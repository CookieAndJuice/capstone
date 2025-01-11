using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas playerUICanvas;

    [SerializeField]
    private DialogueUI dialogue;
    public bool IsDialogueOpened { get { return dialogue.IsOpened; } }
    public void OpenDialogue(NPCScript _npc) { dialogue.OpenDialogue(_npc); }
    public void CloseDialogue() { dialogue.CloseDialogue(); }

    [SerializeField]
    private SupporterUI support;
    public void ToggleSupporterUI(bool _toggle) { support.ToggleSupporterUI(_toggle); }

    [SerializeField]
    private HuntingUI huntingInfo;
    public void SetBattleInfo() { huntingInfo.SetBattleInfo(); }
    public void SetBattleInfo(Room _room) { huntingInfo.SetBattleInfo(_room); }

    private HPbar hpBar;
    public void SetMaxHP(float _hp) { hpBar.SetMaxHP(_hp); }
    public void SetCurHP(float _hp) { hpBar.SetCurHP(_hp); }

    [SerializeField]
    private FadeinFadeout battleUI;
    public void ShowBattleUI() { battleUI.ShowBattleUI(); }

    [SerializeField]
    private TimerScript timer;
    public void StartTimer(Room _room) { timer.StartTimer(_room); }

    public void SetManager()
    {
        hpBar = playerUICanvas.GetComponentInChildren<HPbar>();
        hpBar.SetComps();
    }
}
