using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinimapUI : MonoBehaviour
{
    private TextMeshProUGUI mapName;

    private void Start()
    {
        mapName = GetComponentInChildren<TextMeshProUGUI>();
        mapName.text = SceneManager.GetActiveScene().name;
    }
}
