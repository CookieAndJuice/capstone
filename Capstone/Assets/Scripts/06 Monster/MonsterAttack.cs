using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 1.2f;
    public float attack;

    private void Start()
    {
        Destroy(this.gameObject, lifeTime);
    }
}
