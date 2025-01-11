using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IInteractable))]
public class InteractScript : MonoBehaviour                 // 상호작용이 가능한 오브젝트에 넣는 스크립트
{
    private Vector2 Position2 { get { return new(transform.position.x, transform.position.z); } }

    [SerializeField]
    private float canInteractDist = 2.5f;                                             // 상호작용 가능 거리
    private float canInteractAngle = 60f;

    private IInteractable m_interactable;                                             // 오브젝트 내 IInteractable을 상속한 오브젝트

    private float InteractAngle { get { return canInteractAngle; } }
    public bool CanInteract { get { return DistToPlayer <= canInteractDist && CheckInteractable; } }  // 상호작용 가능한지


    public float DistToPlayer { get { return PlayManager.GetDistToPlayer(Position2); } }           // 플레이어와의 거리
    public Transform InteractTransform { get { return transform; } }                               // 상호작용 대상의 위치

    public bool CheckInteractable { get { return m_interactable.CanInteract; } }

    public void AbleInteract()                 // 조작 허용
    {
        if (!CanInteract) { return; }
        ShowToggleUI();
    }
    public void DisableInteract()              // 조작 비허용
    {
        HideToggleUI();
    }
    private void ShowToggleUI()                 // 조작 UI 띄우기
    {
       // PlayManager.ShowInteractInfo(m_interactable.InfoTxt);
    }
    private void HideToggleUI()                 // 조작 UI 숨기기
    {
       // PlayManager.HideInteractInfo();
    }

    public void StartInteract()                // 상호작용 시작
    {
        m_interactable.StartInteract();
        HideToggleUI();
    }
    public void StopInteract()                  // 상호작용 중단
    {
        m_interactable.StopInteract();
        ShowToggleUI();
    }

    private void Awake()
    {
        m_interactable = GetComponent<IInteractable>();
        m_interactable.SetInteractScript(this);
    }
}
