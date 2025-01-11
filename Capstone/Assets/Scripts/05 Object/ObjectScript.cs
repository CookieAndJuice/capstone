using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectScript : MonoBehaviour, IHittable
{
    [SerializeField]
    private float maxHP;
    protected float curHP;

    [SerializeField]
    private float attack;
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float defense;
    [SerializeField]
    private float speed;

    public Vector3 Position { get { return transform.position; } }                                  // 좌표
    public Vector2 Position2 { get { return new(transform.position.x, transform.position.z); } }    // 평면 좌표

    public float MaxHP
    {
        get { return maxHP; }                         // 최대 체력
        protected set { maxHP = value; }
    }
    public float Attack
    {
        get { return attack; }                        // 공격력
        protected set { attack = value; }
    }
    public float AttackSpeed                          // 공격 속도
    {
        get { return attackSpeed; }
        protected set { attackSpeed = value; }
    }
    public float Defense                            // 방어력
    {
        get { return defense; }
        protected set { defense = value; }
    }
    public virtual float Speed
    {
        get { return speed; }                      // 현재 속도
        protected set { speed = value; }
    }


    public bool IsDead { get; set; }                          // 죽음 상태
    public virtual bool IsUnstoppable { get; } = true;                  // 히트 상태 가능 여부

    public virtual bool IsPlayer { get { return false; } }
    public virtual bool IsMonster { get { return false; } }

    public virtual void GetHit(float _damage)
    {

    }

    public virtual void OnEnable()
    {
        curHP = maxHP;
    }
}
