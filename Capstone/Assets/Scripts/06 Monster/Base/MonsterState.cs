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
        //            Vector3 randomPos = RandomNavSphere(transform.position, PATROLRadius, 1 << 0);  // Walkable ���������� ����ǰ� ��
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
        //        if (IsDie) return; // �ߺ� ó�� ����
        //        IsDie = true;

        //        monsterNav.isStopped = true;
        //        GetComponent<CapsuleCollider>().enabled = false;

        //        // Die Animation

        //        PlayManager.MonsterNum++;   // ��ġ�� ���� �� ����
        //        PlayManager.SetBattleInfo();
        //        GameManager.ReturnObjectToPool(this.gameObject); // Ǯ�� ��ȯ
                
        //        break;
        //}
    }

    protected Vector3 RandomNavSphere(Vector3 _origin, float _distance, int _layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * _distance;  // �ݰ� �� ������ ���� ����
        randomDirection += _origin;  // ���� ��ġ �������� ������ �߰�

        NavMeshHit navHit;
        // NavMesh ���� ��ȿ�� ��ġ ã��
        NavMesh.SamplePosition(randomDirection, out navHit, _distance, _layermask);

        return navHit.position;  // ��ȿ�� ��ġ ��ȯ
    }

    protected void MaintainDistance()
    {
        Collider[] nearbyMonsters = Physics.OverlapSphere(transform.position, minMonsterDistance);
        foreach (Collider collider in nearbyMonsters)
        {
            if (collider.gameObject != this.gameObject && collider.CompareTag("Monster"))
            {
                // �ٸ� ���Ϳ��� �Ÿ� ���
                Vector3 directionAway = transform.position - collider.transform.position;
                float distance = directionAway.magnitude;

                if (distance < minMonsterDistance)
                {
                    // ��������ٸ� �ݴ� �������� �̵��ϰų� NavMesh�� ����
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
