using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHintColor : MonoBehaviour
{
    private List<Color> materialColors;

    void Awake()
    {
        materialColors = new List<Color>();

        materialColors.Add(new Color(1.0f, 68 / 255f, 68 / 255f));
        materialColors.Add(new Color(1.0f, 176 / 255f, 0.0f));
        materialColors.Add(new Color(75 / 255f, 1.0f, 241 / 255f));
        materialColors.Add(new Color(0.0f, 0.0f, 0.0f));
    }

    public void GetRandomColors(ref List<Color> colors)
    {
        int[] indices = new int[4];
        ShuffleArray(indices);

        for (int i = 0; i < indices.Length; i++)
        {
            colors.Add(materialColors[i]);
        }
    }

    private void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            var randomIndex = Random.Range(i, array.Length);

            var temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
