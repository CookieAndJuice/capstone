using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Inst;

    // 게임 시작 관련 함수
    private void StartPlay()
    {
        GameManager.SetControlMode(EControlMode.FIRST_PERSON);
    }

    public static void FreezePlayer()
    {
        Player.GetComponent<Rigidbody>().useGravity = false;
    }
    
    public static void PlayerSpawn()
    {
        foreach(var room in RoomWithWalls)
            if (room.RoomNumber == 0) // 0번 방이 스폰 방
            {
                Player.transform.position = new Vector3((room.CenterCell.X * 4 + 3), 1f, (room.CenterCell.Y * -4 + 3));
                Player.GetComponent<Rigidbody>().useGravity = true;
                break;
            }
    }

    // 플레이어
    private static PlayerController Player { get; set; }
    public static Transform PlayerTransform { get { return Player.transform; } }
    public static Rigidbody PlayerRigidBody { get { return Player.PlayerRB; } }
    public static float MaxHP { get { return Player.MaxHP; } }                                                                              // 플레이어 최대 체력
    public static float PlayerAttack { get { return Player.Attack; } }                                                                      // 플레이어 공격력
    public static void SetCurPlayer(PlayerController _player) { Player = _player; }                                                         // 플레이어 등록
    public static bool CheckIsPlayer(ObjectScript _object) { return _object == Player; }                                                    // 플레이어인지 확인
    public static Vector3 PlayerPos { get { if (IsPlayerSet) return Player.transform.position; return ValueDefinition.NULL_VECTOR; } }      // 플레이어 위치
    public static Vector2 PlayerPos2 { get { if (IsPlayerSet) return Player.Position2; return ValueDefinition.NULL_VECTOR; } }              // 플레이어 평면 위치
    public static bool IsPlayerSet { get { return Player != null; } }                                                                       // 플레이어 등록 여부
    public static float GetDistToPlayer(Vector2 _pos) { if (!IsPlayerSet) return -1; return (PlayerPos2 - _pos).magnitude; }                // 플레이어와의 거리
    public static void PlayerHit(float _hp) { Player.GetHit(_hp); }                                                                         // 플레이어 피격
    public static void StopPlayerInteract() { Player.StopInteract(); }                                                                      // 상호작용 종료
    public static void StopPlayerInteract(InteractScript _interact) { Player.StopInteract(_interact); }
    public static void SetEmotionColor(EEmotion _emotion) { Player.SetEmotionColor(_emotion); }

    // 전투 관련
    public static void PrepareSkill(string _spell, EEmotion _emotion) { Player.PrepareSkill(_spell, _emotion); }
    public static bool IsDrain { get { return Player.IsDrain; } }
    public static void Drain(float _hp) { Player.Drain(_hp); }

    // 몬스터 관련
    public static int CurMonsterNum;                    // 소환이 이루어진 몬스터 수
    public static int MonsterNum;                       // 사냥당한 몬스터 수

    // UI
    private UIManager playerUI;
    private static UIManager UIManager { get { return Inst.playerUI; } }
    public static bool IsDialogueOpened { get { return UIManager.IsDialogueOpened; } }   // 대화창 열렸는지
    public static void OpenDialogue(NPCScript _npc) { UIManager.OpenDialogue(_npc); }    // 대화창 열기
    public static void CloseDialogue() { UIManager.CloseDialogue(); }                    // 대화창 닫기
    public static void SetBattleInfo() { UIManager.SetBattleInfo(); }
    public static void SetBattleInfo(Room _room) { UIManager.SetBattleInfo(_room); }                    // 전투 시 몬스터 수 정보 
    public static void ShowBattleUI() { UIManager.ShowBattleUI(); }
    public static void StartTimer(Room _room) { UIManager.StartTimer(_room); }

    public static void ToggleSupporterUI(bool _toggle) { UIManager.ToggleSupporterUI(_toggle); }
    public static void SetPlayerMaxHP(float _hp) { UIManager.SetMaxHP(_hp); }    // 체력바 최대 체력
    public static void SetPlayerCurHP(float _hp) { UIManager.SetCurHP(_hp); }    // 체력바 현재 체력


    // 맵
    private Main mapMaker;
    public static Main MapMaker { get { return Inst.mapMaker; } }
    public static List<Room> RoomWithWalls { get { return MapMaker.RoomsWithWalls; } }

    private BattleRoomSpawner battleRoomSpawner;
    public static BattleRoomSpawner BattleRoomSpawner { get { return Inst.battleRoomSpawner; } }
    public static int MonsterSpawnerCount { get { return BattleRoomSpawner.MonsterSpawnCount; } set { BattleRoomSpawner.MonsterSpawnCount = value; } }
    public static int TotalMonsterCount { get { return MonsterSpawnerCount * 3; } }

    public static void FinishBattle(Room _room)
    {
        BattleRoomSpawner.FinishBattle(_room);
        RoomManager.Instance.OpenAllDoors();
    }


    private void SetSubManagers()
    {
        playerUI = GetComponent<UIManager>();
        playerUI.SetManager();
        mapMaker = GetComponent<Main>();
        battleRoomSpawner = GetComponent<BattleRoomSpawner>();
    }

    private void Awake()
    {
        if (Inst != null) { Destroy(Inst.gameObject); }
        Inst = this;
        SetSubManagers();
    }

    private void Start()
    {
        StartPlay();
    }
}
