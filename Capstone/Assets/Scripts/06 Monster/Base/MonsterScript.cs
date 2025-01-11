using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class MonsterScript : ObjectScript
{
    [SerializeField]
    protected GameObject monsterBullet;
    [SerializeField]
    protected GameObject attackPoint;

    protected NavMeshAgent monsterNav;
    protected MonsterHPbar HPbar;
    public float MonsterAttack { get { return Attack; } }

    public virtual void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.CompareTag(ValueDefinition.PLAYER_ATTACK_TAG))
        {
            PlayerAttack playerAttack = _other.gameObject.GetComponent<PlayerAttack>();

            GetHit(playerAttack.skillDamage);
            if (playerAttack != null)
            {
                // 피흡이 세팅된 경우 적용
                if (PlayManager.IsDrain)
                {
                    PlayManager.Drain(playerAttack.skillDamage);
                }
                // 상태이상인 경우 적용
                if ((int)playerAttack.StatusEffect >= 3 && (int)playerAttack.StatusEffect <= 6 && !IsDebuffed)
                {
                    StartCoroutine(ApplyCCType(playerAttack.StatusEffect));
                }
            }
        }
    }

    public virtual void Update()
    {
        CheckMonsterState();
        MonsterAction();
    }

    public virtual void Awake()
    {
        HPbar = GetComponentInChildren<MonsterHPbar>();

        monsterNav = GetComponent<NavMeshAgent>();
        monsterNav.radius = 1.5f;
        monsterNav.speed = Speed;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        GameManager.RegisterMonster(this);

        state = EMonsterState.PATROL;
        IsDie = false;
        HPbar.SetComps();   // 활성화 될 때마다 다시 MaxHP로
        monsterNav.isStopped = false;
        monsterNav.autoTraverseOffMeshLink = true; // 링크 자동 탐색 활성화
        GetComponent<CapsuleCollider>().enabled = true;
    }

    private void OnDisable()
    {
        GameManager.UnregisterMonster(this);
    }
}
