using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public partial class PlayerController : ObjectScript
{
    private PlayerCamera playerCam;     // 플레이어 카메라
    private Rigidbody playerRB;         // 플레이어 리지드바디      
    public Rigidbody PlayerRB { get { return playerRB; } }

    public static event Action OnPlayerDeath; // 플레이어의 죽음을 알리는 이벤트

    public GameObject leftArm;
    public GameObject rightArm;
    private MeshRenderer[] playerParts;

    private bool isDead;

    [SerializeField]
    private Material[] emotionMaterials;

    // 입력 관련
    private PlayerInput.PlayerActions PlayerInput { get { return GameManager.PlayerInputs; } }  // Input System Player 입력
    public Vector3 MoveInput { get { return PlayerInput.Movement.ReadValue<Vector2>(); } }      // wasd 입력
    public Vector2 MouseDelta { get { return PlayerInput.Look.ReadValue<Vector2>(); } }         // 마우스 좌표
    public bool AttackTrigger { get { return PlayerInput.Attack.triggered; } }                  // 공격
    public bool JumpTrigger { get { return PlayerInput.Jump.triggered; } }                      // 점프
    public bool RunTrigger { get { return PlayerInput.Run.triggered; } }                                                                                            // 달리기
    public bool InteractTrigger { get { return PlayerInput.Interact.triggered; } }              // 상호작용
    public bool SupportUITrigger { get { return PlayerInput.SupportUI.triggered; } }            // AI 대화창

    public void SetEmotionColor(EEmotion _emotion)
    {
        foreach (var part in playerParts)
        {
            part.material = emotionMaterials[(int)_emotion];
        }
    }

    public void PlayerDie()
    {
        if (isDead) return; // 중복 처리 방지
        isDead = true;

        // 사망 이벤트 호출
        OnPlayerDeath?.Invoke();

        // 사망 시 비활성화 되는 컴포넌트들
        // PlayerInput.Disable();
        playerRB.isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        this.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void UpdateInputs()
    {
        PlayerInteract();       // 상호작용 물체 탐색
        PlayerAttack();
        PlayerJump();
    }

    private void Update()
    {
        PlayerMove();           // 플레이어 이동
        UpdateInputs();         // 기타 조작
    }

    private void LateUpdate()
    {
        playerCam.Rotate();     // 카메라 회전
    }

    public void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerCam = GetComponentInChildren<PlayerCamera>();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        playerParts = GetComponentsInChildren<MeshRenderer>();
        PlayManager.SetCurPlayer(this);
        PlayManager.SetPlayerMaxHP(MaxHP);

        foreach(var part in playerParts)
        {
            Debug.Log(part.name);
        }
        CanAttack = true;
    }
}
