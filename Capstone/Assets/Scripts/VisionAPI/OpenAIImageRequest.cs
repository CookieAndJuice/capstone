using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIImageRequest : MonoBehaviour
{
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
    private const string ApiKey = ""; // Add the OpenAI API key here

    public void SendImageRequest(string base64Image)
    {
        StartCoroutine(SendRequestCoroutine(base64Image));
    }

    private IEnumerator SendRequestCoroutine(string base64Image)
    {
        string json = $@"
        {{
            ""model"": ""gpt-4o-mini"",
            ""messages"": [
                {{
                    ""role"": ""user"",
                    ""content"": [
                        {{
                            ""type"": ""text"",
                            ""text"": ""What is in this image?""
                        }},
                        {{
                            ""type"": ""image_url"",
                            ""image_url"": {{
                                ""url"": ""data:image/jpeg;base64,{base64Image}""
                            }}
                        }}
                    ]
                }}
            ]
        }}";

        using (UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {ApiKey}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}