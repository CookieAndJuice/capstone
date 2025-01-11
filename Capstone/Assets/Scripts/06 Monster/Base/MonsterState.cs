using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class MonsterScript
{
    public enum EMonsterState
    {
        PAUSED,
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    [SerializeField]
    protected float PATROLRadius = 5.0f;
    [SerializeField]
    protected float attackDist = 10.0f;
    [SerializeField]
    protected float traceDist = 20.0f;
    [SerializeField]
    protected float minMonsterDistance = 3.0f;

    protected EMonsterState state = EMonsterState.PATROL;

    public bool IsAttack { get; protected set; }
    public bool IsDie { get; protected set; }

    protected void CheckMonsterState()
    {
        if (state == EMonsterState.DIE)
            return; 

        float distance = Vector3.Distance(PlayManager.PlayerPos, transform.position);

        if (GameManager.ControlMode == EControlMode.UI_CONTROL)
        {
            state = EMonsterState.PAUSED;
        }
        else if (GameManager.ControlMode == EControlMode.FIRST_PERSON)
        {
            if (distance <= attackDist)
                state = EMonsterState.ATTACK;
            else if (distance <= traceDist)
                state = EMonsterState.TRACE;
            else
                state = EMonsterState.PATROL;
        }
    }


    public virtual void MonsterAction()
    {
        //switch (state)
        //{
        //    case EMonsterState.PAUSED:
        //        monsterNav.isStopped = true;
        //        StopAllCoroutines();
        //        return;

        //    case EMonsterState.PATROL:
        //        if (!monsterNav.hasPath || monsterNav.remainingDistance < 0.1f)
        //        {
        //            Vector3 randomPos = RandomNavSphere(transform.position, PATROLRadius, 1 << 0);  // Walkable 영역에서만 적용되게 함
        //            monsterNav.SetDestination(randomPos);
        //        }
        //        break;

        //    case EMonsterState.TRACE:
        //        MaintainDistance();
        //        monsterNav.SetDestination(PlayManager.PlayerPos);
        //        break;

        //    case EMonsterState.ATTACK:
        //        if (!IsAttack) StartCoroutine(TempAttack());
        //        break;

        //    case EMonsterState.DIE:
        //        if (IsDie) return; // 중복 처리 방지
        //        IsDie = true;

        //        monsterNav.isStopped = true;
        //        GetComponent<CapsuleCollider>().enabled = false;

        //        // Die Animation

        //        PlayManager.MonsterNum++;   // 퇴치한 몬스터 수 증가
        //        PlayManager.SetBattleInfo();
        //        GameManager.ReturnObjectToPool(this.gameObject); // 풀로 반환
                
        //        break;
        //}
    }

    protected Vector3 RandomNavSphere(Vector3 _origin, float _distance, int _layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * _distance;  // 반경 내 랜덤한 방향 설정
        randomDirection += _origin;  // 현재 위치 기준으로 오프셋 추가

        NavMeshHit navHit;
        // NavMesh 위의 유효한 위치 찾기
        NavMesh.SamplePosition(randomDirection, out navHit, _distance, _layermask);

        return navHit.position;  // 유효한 위치 반환
    }

    protected void MaintainDistance()
    {
        Collider[] nearbyMonsters = Physics.OverlapSphere(transform.position, minMonsterDistance);
        foreach (Collider collider in nearbyMonsters)
        {
            if (collider.gameObject != this.gameObject && collider.CompareTag("Monster"))
            {
                // 다른 몬스터와의 거리 계산
                Vector3 directionAway = transform.position - collider.transform.position;
                float distance = directionAway.magnitude;

                if (distance < minMonsterDistance)
                {
                    // 가까워졌다면 반대 방향으로 이동하거나 NavMesh를 수정
                    Vector3 newDestination = transform.position + directionAway.normalized * minMonsterDistance;
                    monsterNav.SetDestination(newDestination);
                }
            }
        }
    }

    public virtual void ReactToPlayerDeath()
    {
        state = EMonsterState.PATROL;
        MonsterAction();
    }
}
