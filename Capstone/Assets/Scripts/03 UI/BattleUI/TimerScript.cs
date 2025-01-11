using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    private float timerDuration;
    public TextMeshProUGUI timerText; // UI에 표시할 타이머 텍스트

    private float currentTime;
    private bool isTimerRunning = false;

    private Room curRoom;

    [SerializeField]
    private GameObject explosionEffect; // 플레이어 폭발 이펙트

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime > 0 && CheckCurRoomBattleStatus(curRoom))
            {
                currentTime = 0;
                isTimerRunning = false;
                this.gameObject.SetActive(false);
                return;
            }
            if (currentTime <= 0)
            {
                currentTime = 0;
                isTimerRunning = false;
                OnTimerEnd();
            }
            UpdateTimerDisplay();
        }
    }

    private bool CheckCurRoomBattleStatus(Room _room)
    {
        return _room.IsBattleFinished;
    }

    public void StartTimer(Room _room)
    {
        curRoom = _room;
        gameObject.SetActive(true);

        timerDuration = Random.Range(90, 121);  // 1분 30초부터 2분까지 랜덤

        currentTime = timerDuration;
        isTimerRunning = true;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:00}: {seconds:00}"; // 분:초 형식으로 표시
        }
    }



    private void OnTimerEnd()
    {
        // 몬스터가 모두 퇴치되지 않은 채로 시간이 지나면 플레이어 사망
        if (!curRoom.IsBattleFinished)
            StartCoroutine(PlayerExplosion());
    }

    IEnumerator PlayerExplosion()
    {
        GameObject explosion = Instantiate(explosionEffect, PlayManager.PlayerPos, Quaternion.identity, PlayManager.PlayerTransform);
        yield return new WaitForSeconds(6.5f);  // 파티클 재생시간
        PlayManager.PlayerHit(PlayManager.MaxHP);
        Destroy(explosion);
    }
}
