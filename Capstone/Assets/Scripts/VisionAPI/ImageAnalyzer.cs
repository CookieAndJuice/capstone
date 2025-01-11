using UnityEngine;

public class ImageAnalyzer : MonoBehaviour
{
    public OpenAIImageRequest requestSender;  // Reference to the OpenAIImageRequest component

    private void Start()
    {
        // Ensure the requestSender is assigned
        if (requestSender == null)
        {
            Debug.LogError("OpenAIImageRequest is not assigned in the Inspector.");
            return;
        }

        string imagePath = "/Users/hwany/Projects/Unity Projects/capstone/Capstone/Assets/Images/puzzleTest.webp";
        ImageEncoder encoder = new ImageEncoder();
        string base64Image = encoder.EncodeImage(imagePath);

        // Send the image request
        requestSender.SendImageRequest(base64Image);
    }
}