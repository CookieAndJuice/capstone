using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SupporterFunc : MonoBehaviour
{
    [SerializeField]
    private GameObject chatFrame;
    [SerializeField]
    private Image skillBook;
    [SerializeField]
    private Image note;

    private GridLayoutGroup supporterBtns;

    private void SetBtns()
    {
        for(int i = 0; i < supporterBtns.transform.childCount; i++)
        {
            Transform childTransform = supporterBtns.transform.GetChild(i);
            Button supporterBtn = childTransform.GetComponent<Button>();

            if (supporterBtn != null)
            {
                int index = i - 1;  // Text Á¦¿Ü
                supporterBtn.onClick.AddListener(() => OnButtonClick(index));
            }
        }
    }
    private void OnButtonClick(int index)
    {
        switch (index)
        {
            case 0:
                chatFrame.SetActive(!chatFrame.activeSelf);
                break;
            case 1:
                CheckWall();
                break;
            case 2:
                skillBook.gameObject.SetActive(!skillBook.gameObject.activeSelf);
                break;
            case 3:
                note.gameObject.SetActive(!note.gameObject.activeSelf);
                break;
            default:
                break;
        }
    }

    private void CheckWall()
    {

    }

    void Start()
    {
        supporterBtns = GetComponent<GridLayoutGroup>();
        SetBtns();
    }
}
