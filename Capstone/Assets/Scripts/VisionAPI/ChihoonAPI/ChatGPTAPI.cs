using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;
using System.Collections;

public class ChatGPTAPI : MonoBehaviour
{
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
    private const string ApiKey = ""; // Replace with your OpenAI API key

    [SerializeField] private Text textArea;
    public static Action<string> gptEvent;
    
    private string prompt = "Act as a kind companion in a chat room and reply to the questions. " +
        "Don't break character. Don't ever mention that you are an AI model.";

    private void Start()
    {
        gptEvent += DescribeImage;
    }

    private void DescribeImage(string base64Path)
    {
        StartCoroutine(SendRequestCoroutine(base64Path));
    }

    private IEnumerator SendRequestCoroutine(string base64Path)
    {
        Debug.Log("Describe");

        string json = $@"
        {{
            ""model"": ""gpt-4o-mini"",
            ""messages"": [
                {{
                    ""role"": ""user"",
                    ""content"": [
                        {{
                            ""type"": ""text"",
                            ""text"": ""{prompt} 다음 이미지를 설명해 줘.""
                        }},
                        {{
                            ""type"": ""image_url"",
                            ""image_url"": {{
                                ""url"": ""data:image/png;base64,{base64Path}""
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
                string response = request.downloadHandler.text;

                Debug.Log("Response: " + response);

                ChatGPTResponse chatGPTResponse = JsonUtility.FromJson<ChatGPTResponse>(response);

                textArea.text = chatGPTResponse.choices[0].message.content;
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

    }
}