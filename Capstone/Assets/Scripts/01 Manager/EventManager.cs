using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    private HuntingUI huntingUI;
    [SerializeField]
    private TimerScript timerUI;

    [SerializeField]
    private Image DieFrame;
    [SerializeField]
    private PlayerController Player;

    private List<MonsterScript> monsters;

    public void HandlePlayerDeath()    // 플레이어가 죽었을 때 시스템에서 처리할 기능
    {
        DieFrame.gameObject.SetActive(true);
        NotifyMonsters();
    }
    public void PlayerRespawn()
    {
        Player.IsDead = false;

        Player.PlayerRB.isKinematic = false;
        Player.GetComponent<CapsuleCollider>().enabled = true;
        Player.enabled = true;
        GameManager.SetControlMode(EControlMode.FIRST_PERSON);
        PlayManager.PlayerSpawn();

        DieFrame.gameObject.SetActive(false);
        huntingUI.gameObject.SetActive(false);
        timerUI.gameObject.SetActive(false);
    }
 
    public void RegisterMonster(MonsterScript monster)
    {
        if (!monsters.Contains(monster))
        {
            monsters.Add(monster);
        }
    }
    public void UnregisterMonster(MonsterScript monster)
    {
        if (monsters.Contains(monster))
        {
            monsters.Remove(monster);
        }
    }
    public void NotifyMonsters()    // 플레이어가 죽었을 때 몬스터 단에서 처리할 기능
    {
        foreach (var monster in monsters)
        {
            monster.ReactToPlayerDeath();
        }
    }

    public void SetManager()
    {
        monsters = new List<MonsterScript>();
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= HandlePlayerDeath;
    }
}
