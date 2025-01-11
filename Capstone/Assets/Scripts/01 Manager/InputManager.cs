using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EControlMode    // 조작 모드
{
    FIRST_PERSON,           // 1인칭
    UI_CONTROL              // UI 조작
}

public class InputManager : MonoBehaviour
{
    private PlayerInput inputSystem;                                                                  // 전체 Input System
    public PlayerInput.PlayerActions PlayerInputs { get { return inputSystem.Player; } }              // 플레이어 조작 Input System
    public PlayerInput.UIControlActions UIControlInputs { get { return inputSystem.UIControl; } }     // UI 조작 Input System

    public EControlMode CurControlMode { get; private set; } = EControlMode.UI_CONTROL;     // 현재 조작 모드

    private static float m_mouseSensitive = 1;                                              // 마우스 민감도
    public static float MouseSensitive { get { return m_mouseSensitive; } }


    public void SetMouseSensitive(float _sensitive)                                         // 마우스 민감도 설정
    {
        m_mouseSensitive = _sensitive;
        // PlayManager.SetCameraSensitive(_sensitive);
    }


    public void SetManager()
    {
        inputSystem = new();   
    }

    public void SetControlMode(EControlMode _mode)                                          // 조작 모드 설정
    {
        CurControlMode = _mode;
        switch (_mode)
        {
            case EControlMode.FIRST_PERSON:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                PlayerInputs.Enable();
                UIControlInputs.Disable();
                break;
            case EControlMode.UI_CONTROL:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                UIControlInputs.Enable();
                PlayerInputs.Disable();
                break;
        }
    }

    private void Update()
    {
        if(CurControlMode == EControlMode.FIRST_PERSON)
        {
            if(PlayerInputs.SupportUI.triggered)
            {
                PlayManager.ToggleSupporterUI(true);
                SetControlMode(EControlMode.UI_CONTROL);
            }
        }
        else if(CurControlMode == EControlMode.UI_CONTROL)
        {
            if (UIControlInputs.SupportUI.triggered)
            {
                PlayManager.ToggleSupporterUI(false);
                SetControlMode(EControlMode.FIRST_PERSON);
            }
            else if(UIControlInputs.Interact.triggered)
            {
                PlayManager.StopPlayerInteract();
                SetControlMode(EControlMode.FIRST_PERSON);
            }
        }
    }
}
