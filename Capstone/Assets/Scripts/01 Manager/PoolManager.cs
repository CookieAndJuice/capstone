using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField]
    private int objectNum = 10; // Ǯ ũ�� ����

    private List<GameObject> poolObjects = new List<GameObject>();
    public List<GameObject> PoolObjects { get { return poolObjects; } }

    public GameObject[] poolObject;

    public void SetManager()
    {
        for (int i = 0; i < objectNum; i++)
        {
            // �迭���� ������ ������Ʈ ����
            int randomIndex = Random.Range(0, poolObject.Length);
            GameObject obj = Instantiate(poolObject[randomIndex]);

            obj.SetActive(false); // ��Ȱ��ȭ�� ���·� �ʱ�ȭ
            poolObjects.Add(obj); // ����Ʈ�� �߰�
        }
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject poolObject in poolObjects)
        {
            if (poolObject.activeSelf == false) // ��Ȱ��ȭ�� ������Ʈ ��ȯ
            {
                return poolObject;
            }
        }
        return null; // ��Ȱ��ȭ�� ������Ʈ�� ���� ���
    }

    public void ReturnObjectToPool(GameObject _obj)
    {
        if (_obj.activeSelf)
        {
            _obj.SetActive(false); // �ݵ�� ��Ȱ��ȭ
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

