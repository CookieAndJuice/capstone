using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    // 입력
    private InputManager inputManager;
    public static InputManager InputManager { get { return Inst.inputManager; } }
    public static PlayerInput.PlayerActions PlayerInputs { get { return InputManager.PlayerInputs; } }                                      // 플레이어 Input
    public static PlayerInput.UIControlActions UIControlInputs { get { return InputManager.UIControlInputs; } }                             // UI조작 Input
    public static EControlMode ControlMode { get { return InputManager.CurControlMode; } }                                                  // 조작 모드
    public static float MouseSensitive { get { return InputManager.MouseSensitive; } }                                                      // 마우스 민감도
    public static void SetControlMode(EControlMode _mode) { InputManager.SetControlMode(_mode); }                                           // 조작 모드 변경
    public static void SetMouseSensitive(float _sensitive) { InputManager.SetMouseSensitive(_sensitive); }                                  // 마우스 민감도 설정

    // 스킬
    private SkillManager skillManager;
    public static SkillManager SkillManager { get { return Inst.skillManager; } }
    public static PlayerAttack[] Skills { get { return SkillManager.skills; } }

    // 오브젝트 풀
    private PoolManager poolManager;
    public static PoolManager PoolManager { get { return Inst.poolManager; } }
    public static List<GameObject> PoolObjects { get { return PoolManager.PoolObjects; } }
    public static GameObject GetPooledObject() { return PoolManager.GetPooledObject(); }
    public static void ReturnObjectToPool(GameObject _obj) { PoolManager.ReturnObjectToPool(_obj); }
    public static void InActiveMonsters() { PoolManager.InActiveMonsters(); }

    // 이벤트
    private EventManager eventManager;
    public static EventManager EventManager { get { return Inst.eventManager; } }
    public static void HandlePlayerDeath() { EventManager.HandlePlayerDeath(); }
    public static void RegisterMonster(MonsterScript _monster) { EventManager.RegisterMonster(_monster); }
    public static void UnregisterMonster(MonsterScript _monster) { EventManager.UnregisterMonster(_monster); }
    public static void NotifyMonsters() { EventManager.NotifyMonsters(); }

    private void SetSubManagers()
    {
        inputManager = GetComponent<InputManager>();
        inputManager.SetManager();
        skillManager = GetComponent<SkillManager>();
        skillManager.SetManager();
        eventManager = GetComponent<EventManager>();
        eventManager.SetManager();
        poolManager = GetComponent<PoolManager>();
        poolManager.SetManager();
    }

    private void Awake()
    {
        if (Inst != null) { Destroy(gameObject); return; }
        Inst = this;
        DontDestroyOnLoad(gameObject);
        SetSubManagers();
    }
}
