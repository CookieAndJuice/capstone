using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Inst;

    // ���� ���� ���� �Լ�
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
            if (room.RoomNumber == 0) // 0�� ���� ���� ��
            {
                Player.transform.position = new Vector3((room.CenterCell.X * 4 + 3), 1f, (room.CenterCell.Y * -4 + 3));
                Player.GetComponent<Rigidbody>().useGravity = true;
                break;
            }
    }

    // �÷��̾�
    private static PlayerController Player { get; set; }
    public static Transform PlayerTransform { get { return Player.transform; } }
    public static Rigidbody PlayerRigidBody { get { return Player.PlayerRB; } }
    public static float MaxHP { get { return Player.MaxHP; } }                                                                              // �÷��̾� �ִ� ü��
    public static float PlayerAttack { get { return Player.Attack; } }                                                                      // �÷��̾� ���ݷ�
    public static void SetCurPlayer(PlayerController _player) { Player = _player; }                                                         // �÷��̾� ���
    public static bool CheckIsPlayer(ObjectScript _object) { return _object == Player; }                                                    // �÷��̾����� Ȯ��
    public static Vector3 PlayerPos { get { if (IsPlayerSet) return Player.transform.position; return ValueDefinition.NULL_VECTOR; } }      // �÷��̾� ��ġ
    public static Vector2 PlayerPos2 { get { if (IsPlayerSet) return Player.Position2; return ValueDefinition.NULL_VECTOR; } }              // �÷��̾� ��� ��ġ
    public static bool IsPlayerSet { get { return Player != null; } }                                                                       // �÷��̾� ��� ����
    public static float GetDistToPlayer(Vector2 _pos) { if (!IsPlayerSet) return -1; return (PlayerPos2 - _pos).magnitude; }                // �÷��̾���� �Ÿ�
    public static void PlayerHit(float _hp) { Player.GetHit(_hp); }                                                                         // �÷��̾� �ǰ�
    public static void StopPlayerInteract() { Player.StopInteract(); }                                                                      // ��ȣ�ۿ� ����
    public static void StopPlayerInteract(InteractScript _interact) { Player.StopInteract(_interact); }
    public static void SetEmotionColor(EEmotion _emotion) { Player.SetEmotionColor(_emotion); }

    // ���� ����
    public static void PrepareSkill(string _spell, EEmotion _emotion) { Player.PrepareSkill(_spell, _emotion); }
    public static bool IsDrain { get { return Player.IsDrain; } }
    public static void Drain(float _hp) { Player.Drain(_hp); }

    // ���� ����
    public static int CurMonsterNum;                    // ��ȯ�� �̷���� ���� ��
    public static int MonsterNum;                       // ��ɴ��� ���� ��

    // UI
    private UIManager playerUI;
    private static UIManager UIManager { get { return Inst.playerUI; } }
    public static bool IsDialogueOpened { get { return UIManager.IsDialogueOpened; } }   // ��ȭâ ���ȴ���
    public static void OpenDialogue(NPCScript _npc) { UIManager.OpenDialogue(_npc); }    // ��ȭâ ����
    public static void CloseDialogue() { UIManager.CloseDialogue(); }                    // ��ȭâ �ݱ�
    public static void SetBattleInfo() { UIManager.SetBattleInfo(); }
    public static void SetBattleInfo(Room _room) { UIManager.SetBattleInfo(_room); }                    // ���� �� ���� �� ���� 
    public static void ShowBattleUI() { UIManager.ShowBattleUI(); }
    public static void StartTimer(Room _room) { UIManager.StartTimer(_room); }

    public static void ToggleSupporterUI(bool _toggle) { UIManager.ToggleSupporterUI(_toggle); }
    public static void SetPlayerMaxHP(float _hp) { UIManager.SetMaxHP(_hp); }    // ü�¹� �ִ� ü��
    public static void SetPlayerCurHP(float _hp) { UIManager.SetCurHP(_hp); }    // ü�¹� ���� ü��


    // ��
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
