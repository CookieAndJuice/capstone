using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    private float timerDuration;
    public TextMeshProUGUI timerText; // UI�� ǥ���� Ÿ�̸� �ؽ�Ʈ

    private float currentTime;
    private bool isTimerRunning = false;

    private Room curRoom;

    [SerializeField]
    private GameObject explosionEffect; // �÷��̾� ���� ����Ʈ

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

        timerDuration = Random.Range(90, 121);  // 1�� 30�ʺ��� 2�б��� ����

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
            timerText.text = $"{minutes:00}: {seconds:00}"; // ��:�� �������� ǥ��
        }
    }



    private void OnTimerEnd()
    {
        // ���Ͱ� ��� ��ġ���� ���� ä�� �ð��� ������ �÷��̾� ���
        if (!curRoom.IsBattleFinished)
            StartCoroutine(PlayerExplosion());
    }

    IEnumerator PlayerExplosion()
    {
        GameObject explosion = Instantiate(explosionEffect, PlayManager.PlayerPos, Quaternion.identity, PlayManager.PlayerTransform);
        yield return new WaitForSeconds(6.5f);  // ��ƼŬ ����ð�
        PlayManager.PlayerHit(PlayManager.MaxHP);
        Destroy(explosion);
    }
}
