using System;
using System.IO;
using UnityEngine;

public class ImageEncoder : MonoBehaviour
{
    public string EncodeImage(string imagePath)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        return Convert.ToBase64String(imageBytes);
    }
}