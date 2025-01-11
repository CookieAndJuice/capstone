using UnityEngine;

public class WhisperPresenter : MonoBehaviour
{
    [SerializeField] private WhisperModel whisperModel; // Reference to WhisperModel component
    private string[] spells = new string[] { ValueDefinition.SPELL1, ValueDefinition.SPELL2, ValueDefinition.SPELL3 };
    private float accuracy;
    private EEmotion emotionIndex;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!whisperModel) whisperModel = GetComponent<WhisperModel>(); // Get WhisperModel component if not present
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            whisperModel.StartRecording(); // Start recording when the left control key is pressed
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ProcessVoiceInput(); // Process voice input when the left control key is released
        }
    }

    private void CheckSpell(string spellInput)
    {
        // trim the spell
        spellInput = spellInput.Trim().ToLower();

        foreach (var correctSpell in spells)
        {
            var matchingCount = 0;

            for (var i = 0; i < correctSpell.Length; i++)
                if (i < spellInput.Length && spellInput[i] == correctSpell[i])
                    matchingCount++;
            
            accuracy = (float)matchingCount / correctSpell.Length;
            
            if (accuracy > 0.7f)
            {
                spellInput = correctSpell;
                // var emotionIndex = (EEmotion)whisperModel.predictedIndex;
                var emotionString = whisperModel.emotions[(int)emotionIndex];

                PlayManager.PrepareSkill(spellInput, emotionIndex);
                Debug.Log($"Skill: {spellInput}, Emotion: {emotionString}");
                break;
            }
        }
        Debug.Log($"Acc.: {accuracy}");
    } 

    private async void ProcessVoiceInput()
    {
        var result = await whisperModel.StopRecording(); // Stop recording and start transcription
        Debug.Log($"{result}"); // Output the result

        emotionIndex = (EEmotion)whisperModel.predictedIndex;
        Debug.Log(whisperModel.emotions[(int)emotionIndex]);
        PlayManager.SetEmotionColor(emotionIndex);
        CheckSpell(result);
    }
}