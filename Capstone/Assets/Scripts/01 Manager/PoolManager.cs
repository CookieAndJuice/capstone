using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField]
    private int objectNum = 10; // 풀 크기 설정

    private List<GameObject> poolObjects = new List<GameObject>();
    public List<GameObject> PoolObjects { get { return poolObjects; } }

    public GameObject[] poolObject;

    public void SetManager()
    {
        for (int i = 0; i < objectNum; i++)
        {
            // 배열에서 랜덤한 오브젝트 선택
            int randomIndex = Random.Range(0, poolObject.Length);
            GameObject obj = Instantiate(poolObject[randomIndex]);

            obj.SetActive(false); // 비활성화된 상태로 초기화
            poolObjects.Add(obj); // 리스트에 추가
        }
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject poolObject in poolObjects)
        {
            if (poolObject.activeSelf == false) // 비활성화된 오브젝트 반환
            {
                return poolObject;
            }
        }
        return null; // 비활성화된 오브젝트가 없는 경우
    }

    public void ReturnObjectToPool(GameObject _obj)
    {
        if (_obj.activeSelf)
        {
            _obj.SetActive(false); // 반드시 비활성화
            PoolObjects.Add(_obj);
        }
    }

    public void InActiveMonsters()
    {
        foreach (GameObject poolObject in poolObjects)
        {
            if (poolObject.activeSelf == true) 
            {
                poolObject.SetActive(false);
            }
        }
    }
}

