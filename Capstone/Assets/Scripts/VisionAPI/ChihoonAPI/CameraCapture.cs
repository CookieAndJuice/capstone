using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    [SerializeField] private Camera selectedCamera;       //보여지는 카메라.
    
    public static Func<string> capture;
    private int resWidth;
    private int resHeight;
    string imagePath;
    // Use this for initialization
    void Start()
    {
        resWidth = Screen.width;
        resHeight = Screen.height;
        imagePath = Application.dataPath + "/Resources/ScreenShots/";
        Debug.Log(imagePath);

        capture += ClickScreenShot;
    }

    public string ClickScreenShot()
    {
        DirectoryInfo dir = new DirectoryInfo(imagePath);
        if (!dir.Exists)
        {
            Directory.CreateDirectory(imagePath);
        }
        string name;
        name = imagePath + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        selectedCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        Rect rec = new Rect(0, 0, screenShot.width, screenShot.height);
        selectedCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();

        byte[] screenshotBytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(name, screenshotBytes);

        string base64String = Convert.ToBase64String(screenshotBytes);

        Debug.Log("Camera Screen Captured");

        return base64String;
    }
}
