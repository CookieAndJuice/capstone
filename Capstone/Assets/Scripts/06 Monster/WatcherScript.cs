using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherScript : MonsterScript
{
    public enum ECatState
    {
        PAUSED,
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    private Animator animator;

    private readonly int hashDamage = Animator.StringToHash("IsDamage"); // 피격
    private readonly int hashWalk = Animator.StringToHash("IsWalk");
    private readonly int hashRun = Animator.StringToHash("IsRun");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");  // 공격
    private readonly int hashIdle = Animator.StringToHash("IsIdle");   // 플레이어가 죽었을 때 idle
    private readonly int hashDie = Animator.StringToHash("IsDie");

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public override void Update()
    {
        base.CheckMonsterState();
        MonsterAction();
    }

    public override void OnTriggerEnter(Collider _other)
    {
        base.OnTriggerEnter(_other);
        if (_other.gameObject.CompareTag(ValueDefinition.PLAYER_ATTACK_TAG))
            animator.SetTrigger(hashDamage);
    }

    public override void MonsterAction()
    {
        switch (state)
        {
            case EMonsterState.PAUSED:
                base.monsterNav.isStopped = true;
                StopAllCoroutines();
                return;

            case EMonsterState.PATROL:
                if (!monsterNav.hasPath || monsterNav.remainingDistance < 0.1f)
                {
                    Vector3 randomPos = base.RandomNavSphere(transform.position, base.PATROLRadius, 1 << 0);  // Walkable 영역에서만 적용되게 함
                    monsterNav.SetDestination(randomPos);
                }
                animator.SetBool(hashRun, false);
                break;

            case EMonsterState.TRACE:
                MaintainDistance();
                animator.SetBool(hashRun, true);
                animator.SetBool(hashAttack, false);
                monsterNav.SetDestination(PlayManager.PlayerPos);

                break;

            case EMonsterState.ATTACK:
                if (!IsAttack) StartCoroutine(base.TempAttack());
                animator.SetBool(hashAttack, true);
                break;

            case EMonsterState.DIE:
                if (IsDie) return; // 중복 처리 방지
                base.IsDie = true;

                monsterNav.isStopped = true;
                GetComponent<CapsuleCollider>().enabled = false;
                HPbar.gameObject.SetActive(false);

                animator.SetTrigger(hashDie);

                StartCoroutine(HandleMonsterDeath());
                break;
        }
    }

    IEnumerator HandleMonsterDeath()
    {
        yield return new WaitForSeconds(2.5f);

        PlayManager.MonsterNum++;   // 퇴치한 몬스터 수 증가
        PlayManager.SetBattleInfo();
        GameManager.ReturnObjectToPool(this.gameObject); // 풀로 반환
    }

    public override void ReactToPlayerDeath()
    {
        state = EMonsterState.PATROL;
        MonsterAction();
    }

}
