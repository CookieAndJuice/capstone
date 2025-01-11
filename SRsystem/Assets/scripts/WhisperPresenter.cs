using UnityEngine;

public class WhisperPresenter : MonoBehaviour
{
    [SerializeField] private WhisperModel whisperModel; // Reference to WhisperModel component

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

    private async void ProcessVoiceInput()
    {
        var result = await whisperModel.StopRecording(); // Stop recording and start transcription
        Debug.Log($"{result}"); // Output the result
    }
}