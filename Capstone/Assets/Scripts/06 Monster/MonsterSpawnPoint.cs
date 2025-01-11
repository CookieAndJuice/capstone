using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private MonsterScript monster;
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private float spawnRadius;

    IEnumerator MonsterSpawn()
    {
        int totalMonsterCount = PlayManager.TotalMonsterCount;
        // int totalMonsterCount = 1;

        while (true) // ��� ����
        {
            if (PlayManager.CurMonsterNum < totalMonsterCount)
            {
                GameObject monster = GameManager.GetPooledObject();

                if (monster != null)
                {
                    // ���� Ȱ��ȭ
                    monster.transform.position = GetRandomPositionWithinRadius();
                    monster.transform.rotation = Quaternion.identity;
                    monster.SetActive(true);

                    PlayManager.CurMonsterNum++;
                }
            }

            yield return new WaitForSeconds(spawnTime); 
        }
    }

    Vector3 GetRandomPositionWithinRadius()
    {
        // 2D �󿡼� �ݰ� ���� ������ ��ġ ���� (x, z �� ���)
        Vector2 randomPos2D = Random.insideUnitCircle * spawnRadius;
        Vector3 randomPos = new Vector3(randomPos2D.x, 0, randomPos2D.y);

        return transform.position + randomPos;
    }

    private void Start()
    {
        StartCoroutine(MonsterSpawn());
    }

}
