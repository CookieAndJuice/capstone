using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity.Sentis;

using static WhisperEx;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class WhisperModel : MonoBehaviour
{
    #region Fields
    private BackendType backendType = BackendType.GPUCompute; // Set backend type (using GPUCompute)
    private IWorker logMelSpectroEngine; // Log Mel Spectrogram engine
    private IWorker encoderEngine; // Encoder engine
    private IWorker decoderEngine; // Decoder engine
    private IWorker SEREngine; // SER engine
    private TensorFloat encodedAudio; // Encoded audio data
    TensorFloat predictedClass;
    public int predictedIndex;
    public string emotion;
    private AudioSource audioSource; // Audio source component
    private AudioClip audioClip; // Recorded audio clip
    private int numSamples; // Number of samples
    private float[] data; // Audio data array
    public string[] emotions = { "happiness", "angry", "disgust", "fear", "neutral", "sadness", "surprise" };

    private int[] whiteSpaceCharacters = new int[256]; // Whitespace character array

    private bool transcribe = false; // Transcription state flag
    private string outputString = string.Empty; // Transcribed output string

    private string[] tokens; // Token array
    private int currentToken = 0; // Current token index
    private int[] outputTokens = new int[MaxTokens]; // Output token array

    [SerializeField] Language speakerLanguage; // Speaker language setting
    private readonly bool isReplay = true; // Replay flag
    private const int MaxRecordingTime = 30; // Maximum recording time (seconds)
    private const int AudioSamplingRate = 16000; // Audio sampling rate
    private const int maxSamples = MaxRecordingTime * AudioSamplingRate; // Maximum number of samples
    #endregion

    #region Unity Methods
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!audioSource) audioSource = GetComponent<AudioSource>(); // Get audio source component if not present
#endif
    }

    private void Start()
    {
        InitializeModelsAndWorkers(); // Initialize models and workers
        GetTokens(); // Load tokens
        SetupWhiteSpaceShifts(); // Set up whitespace characters
    }

    private void OnDestroy()
    {
        DisposeWorkers(); // Dispose of workers
    }
    #endregion

    #region Recording Methods
    public void StartRecording()
    {
        Debug.Log("WhisperModel.StartRecording(): Recording started");

        if (audioClip != null)
        {
            Destroy(audioClip); // Destroy existing audio clip
            audioClip = null;
        }

        audioClip = Microphone.Start(null, false, MaxRecordingTime, AudioSamplingRate); // Start microphone recording
    }

    public async Task<string> StopRecording()
    {
        Debug.Log("WhisperModel.StopRecording(): Recording stopped");
        Microphone.End(null); // Stop microphone recording
        var tcs = new TaskCompletionSource<string>();

        if (audioClip != null)
        {
            SaveRecordedClip(); // Save recorded clip
            

            tcs.SetResult(await RunWhisperAsync()); // Run Whisper model
            PredictEmotion();
            return await tcs.Task;

        }
        else
        {
            Debug.LogWarning("WhisperModel.StopRecording(): No audio clip recorded");
            return (string.Empty); // Return empty transcription and an invalid emotion value if no audio clip recorded // Return empty string if no audio clip recorded
        }
    }
    


    private void PredictEmotion()
    {
        // Debug.Log("WhisperModel.PredictEmotion(): 감정 예측 시작");

        using var input = new TensorFloat(new TensorShape(1, numSamples/6), data); // 감정 예측을 위한 입력 텐서를 생성합니다.
        SEREngine.Execute(input); // SER 모델을 실행하여 감정을 예측합니다.
        var predictedClass = SEREngine.PeekOutput() as TensorFloat; // SER 모델의 ArgMax 적용된 출력 값을 가져옵니다.
        predictedClass.CompleteOperationsAndDownload();
        float[] classValues = predictedClass.ToReadOnlyArray();
        float[] softmaxValues = Softmax(classValues); // Softmax 적용
        predictedIndex = Array.IndexOf(softmaxValues, softmaxValues.Max()); // 가장 높은 확률을 가진 클래스의 인덱스 찾기
        //Debug.Log($"WhisperModel.PredictEmotion(): 예측된 감정 클래스 - {(predictedClass.shape)}");
        // Debug.Log($"WhisperModel.PredictEmotion():{emotions[predictedIndex]}");
        emotion= emotions[predictedIndex];

    }

    private float[] Softmax(float[] logits)
    {
        float maxLogit = logits.Max(); // Overflow 방지를 위해 최대값을 빼줌
        float[] expValues = logits.Select(x => MathF.Exp(x - maxLogit)).ToArray();
        float sumExpValues = expValues.Sum();
        return expValues.Select(x => x / sumExpValues).ToArray();
    }

    private void SaveRecordedClip()
    {
        audioSource.clip = audioClip; // Set recorded clip to audio source
        if (isReplay)
        {
            audioSource.Play(); // Play audio clip if replay flag is true
        }
        LoadAudioClip(); // Load audio clip
    }
    #endregion

    #region Audio Processing Methods
    private void LoadAudioClip()
    {
        LoadAudio(); // Load audio
        EncodeAudio(); // Encode audio
        ResetTranscriptionState(); // Reset transcription state
    }

    private void LoadAudio()
    {
        if (audioClip.frequency != AudioSamplingRate)
        {
            Debug.Log($"WhisperModel.LoadAudio(): The audio clip should have frequency 16kHz. It has frequency {audioClip.frequency / 1000f}kHz");
            return; // Warn if audio clip frequency is not 16kHz
        }

        numSamples = audioClip.samples; // Set number of samples in audio clip
        if (numSamples > (maxSamples) )
        {
            Debug.Log($"WhisperModel.LoadAudio(): The AudioClip is too long. It must be less than 30 seconds. This clip is {numSamples / audioClip.frequency} seconds.");
            return; // Warn if audio clip is too long
        }

        data = new float[numSamples]; // Initialize audio data array
        audioClip.GetData(data, 0); // Load audio data
    }

    private void EncodeAudio()
    {
        using var input = new TensorFloat(new TensorShape(1, numSamples), data); // Create input tensor
        logMelSpectroEngine.Execute(input); // Run log Mel spectrogram engine
        var spectroOutput = logMelSpectroEngine.PeekOutput() as TensorFloat; // Get log Mel spectrogram output
        encoderEngine.Execute(spectroOutput); // Run encoder engine
        encodedAudio = encoderEngine.PeekOutput() as TensorFloat; // Get encoded audio
    }

    private void ResetTranscriptionState()
    {
        transcribe = true; // Set transcription state
        outputString = string.Empty; // Initialize output string
        Array.Fill(outputTokens, 0); // Initialize output token array
        outputTokens[0] = StartOfTranscript; // Set start of transcript token
        outputTokens[1] = (int)speakerLanguage; // Set speaker language token
        outputTokens[2] = Transcribe; // Set transcribe token
        outputTokens[3] = NoTimeStamps; // Set no timestamps token
        currentToken = 3; // Set current token index
    }
    #endregion

    #region Transcription Methods
    private async Task<string> RunWhisperAsync()
    {
        var tcs = new TaskCompletionSource<string>();
        StartCoroutine(WhisperCoroutine((result) => tcs.SetResult(result))); // Start Whisper coroutine
        return await tcs.Task; // Return result
    }

    private IEnumerator WhisperCoroutine(Action<string> onWhisperCompleted)
    {
        while (transcribe && currentToken < outputTokens.Length - 1)
        {
            using var tokensSoFar = new TensorInt(new TensorShape(1, outputTokens.Length), outputTokens); // Create input tokens

            var inputs = new Dictionary<string, Tensor>
            {
                {"input_0", tokensSoFar }, // Set input tokens
                {"input_1", encodedAudio } // Set encoded audio
            };

            decoderEngine.Execute(inputs); // Run decoder engine
            var tokensPredictions = decoderEngine.PeekOutput() as TensorInt; // Get predicted tokens
            tokensPredictions.CompleteOperationsAndDownload(); // Download predicted tokens

            var id = tokensPredictions[currentToken]; // Get current token
            outputTokens[++currentToken] = id; // Add to output tokens

            if (id == EndOfText)
            {
                transcribe = false; // End transcription
                outputString = GetUnicodeText(outputString); // Convert to Unicode text
                onWhisperCompleted?.Invoke(outputString); // Call completion callback
                yield break; // End coroutine
            }
            else if (id >= tokens.Length)
            {
                outputString += $"(time={(id - StartTime) * 0.02f})"; // Add timestamp
            }
            else
            {
                outputString += tokens[id]; // Add token
            }

            yield return null; // Wait until next frame
        }
    }
    #endregion

    #region Initialization Methods
    private void InitializeModelsAndWorkers()
    {
        var logMelSpectroModel = ModelLoader.Load($"{Application.streamingAssetsPath}/{LogMelSpectroModelName}"); // Load log Mel spectrogram model
        var encoderModel = ModelLoader.Load($"{Application.streamingAssetsPath}/{EncoderModelName}"); // Load encoder model
        var decoderModel = ModelLoader.Load($"{Application.streamingAssetsPath}/{DecoderModelName}"); // Load decoder model
        var SERModel = ModelLoader.Load($"{Application.streamingAssetsPath}/{SERModelName}"); // Load decoder model
        var decoderModelWithArgMax = Functional.Compile(
            forward: (tokens, audio) =>
            {
                return Functional.ArgMax(decoderModel.Forward(tokens, audio)[0], 2); // Apply ArgMax to decoder model
            },
            inputDefs: (decoderModel.inputs[0], decoderModel.inputs[1])
        );
 
        logMelSpectroEngine = WorkerFactory.CreateWorker(backendType, logMelSpectroModel); // Create log Mel spectrogram worker
        encoderEngine = WorkerFactory.CreateWorker(backendType, encoderModel); // Create encoder worker
        decoderEngine = WorkerFactory.CreateWorker(backendType, decoderModelWithArgMax); // Create decoder worker
        SEREngine = WorkerFactory.CreateWorker(backendType, SERModel); // Create SER worker
    }

    private void GetTokens()
    {
        var jsonText = File.ReadAllText($"{Application.streamingAssetsPath}/{VocabName}"); // Read vocabulary file
        var vocab = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonText); // Deserialize JSON
        tokens = new string[vocab.Count]; // Initialize token array
        foreach (var item in vocab)
        {
            tokens[item.Value] = item.Key; // Map vocabulary
        }
    }

    private void SetupWhiteSpaceShifts()
    {
        for (int i = 0, n = 0; i < 256; i++)
        {
            if (IsWhiteSpace((char)i))
            {
                whiteSpaceCharacters[n++] = i; // Set whitespace characters
            }
        }
    }

    private void DisposeWorkers()
    {
        logMelSpectroEngine?.Dispose(); // Dispose log Mel spectrogram worker
        encoderEngine?.Dispose(); // Dispose encoder worker
        decoderEngine?.Dispose(); // Dispose decoder worker
        SEREngine?.Dispose(); // Dispose decoder worker
    }
    #endregion

    #region Helper Methods
    private string GetUnicodeText(string text)
    {
        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(ShiftCharacterDown(text)); // Convert string to byte array with ISO-8859-1 encoding
        return Encoding.UTF8.GetString(bytes); // Convert byte array to UTF-8 string
    }

    private string ShiftCharacterDown(string text)
    {
        var outText = new StringBuilder();
        foreach (char letter in text)
        {
            outText.Append(((int)letter <= 256)
                ? letter
                : (char)whiteSpaceCharacters[(int)(letter - 256)]); // Shift character
        }
        return outText.ToString();
    }

    private bool IsWhiteSpace(char c)
    {
        return !((33 <= c && c <= 126) || (161 <= c && c <= 172) || (187 <= c && c <= 255)); // Check if character is whitespace
    }
    #endregion
}