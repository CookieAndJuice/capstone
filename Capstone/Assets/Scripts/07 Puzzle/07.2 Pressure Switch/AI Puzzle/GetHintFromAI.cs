using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHintFromAI : MonoBehaviour
{
    void Start()
    {
        
    }

    public void GetHint()
    {
        string imagePath = "";
        imagePath = CameraCapture.capture();
        ChatGPTAPI.gptEvent(imagePath);


    }
}
