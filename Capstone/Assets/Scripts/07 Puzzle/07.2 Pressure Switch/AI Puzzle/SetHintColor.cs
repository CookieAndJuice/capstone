using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHintColor : MonoBehaviour
{
    [SerializeField] GameObject[] hintObjects;
    private List<Color> hintColors;

    void Awake()
    {
        hintColors = new List<Color>();
    }

    public void SetColors()
    {
        this.gameObject.GetComponent<RandomHintColor>().GetRandomColors(ref hintColors);

        for (int i = 0; i < 4; i++)
        {
            GameObject hintObject = hintObjects[i];
            foreach (Transform child in hintObject.transform)
            {
                Debug.Log($"child name : {child.name}");
                var childMaterial = child.GetComponent<Material>();
                childMaterial.color = hintColors[i];
            }
        }
    }
}
