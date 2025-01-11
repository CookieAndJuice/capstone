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

        while (true) // 계속 실행
        {
            if (PlayManager.CurMonsterNum < totalMonsterCount)
            {
                GameObject monster = GameManager.GetPooledObject();

                if (monster != null)
                {
                    // 몬스터 활성화
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
        // 2D 상에서 반경 내의 랜덤한 위치 생성 (x, z 축 사용)
        Vector2 randomPos2D = Random.insideUnitCircle * spawnRadius;
        Vector3 randomPos = new Vector3(randomPos2D.x, 0, randomPos2D.y);

        return transform.position + randomPos;
    }

    private void Start()
    {
        StartCoroutine(MonsterSpawn());
    }

}
