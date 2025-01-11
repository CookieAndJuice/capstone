using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HuntingUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI monsterNum;
    private int totalMonsterNum;
    private Room curRoom;

    public void SetBattleInfo() // ���Ͱ� �׾��� ���� ����
    {
        monsterNum.text = $"{PlayManager.MonsterNum} / {totalMonsterNum}";

        // ���� ��
        if (PlayManager.MonsterNum == totalMonsterNum)
        {
            PlayManager.FinishBattle(curRoom);
            this.gameObject.SetActive(false);
        }
    }

    public void SetBattleInfo(Room _room) // �濡 ó�� �������� ���� ����
    {
        curRoom = _room;
        this.gameObject.SetActive(true);
        monsterNum.text = $"{PlayManager.MonsterNum} / {totalMonsterNum}";
    }

    private void OnEnable()
    {
        totalMonsterNum = PlayManager.TotalMonsterCount;
        // totalMonsterNum = 1;
    }
}
