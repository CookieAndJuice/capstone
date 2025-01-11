using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MonsterScript
{
    [SerializeField]
    private float duration = 3f; // cc기 지속시간
    [SerializeField]
    private float dottedDamage = 5f;

    public bool IsDebuffed { get; private set; }
    public bool IsDotted { get; private set; }  // 도트 데미지 여부

    public override void GetHit(float _damage)
    {
        curHP -= _damage;
        HPbar.gameObject.SetActive(true);
        HPbar.SetHPValue(curHP);

        if (curHP <= 0)
        {
            state = EMonsterState.DIE;
            // 죽음 상태 즉시 처리
            MonsterAction();
        }
    }

    protected IEnumerator TempAttack()
    {
        this.transform.LookAt(PlayManager.PlayerPos);

        IsAttack = true;

        GameObject bullet = Instantiate(monsterBullet, attackPoint.transform.position, attackPoint.transform.rotation);

        MonsterAttack monsterAttack = bullet.GetComponent<MonsterAttack>();
        if (monsterAttack != null) monsterAttack.attack = this.Attack;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) rb.velocity = attackPoint.transform.forward * 15;

        yield return new WaitForSeconds(1.0f);

        IsAttack = false;
    }

    // 지속시간 동안 상태이상 적용
    protected IEnumerator ApplyCCType(EStatusEffect _ccType)
    {
        IsDebuffed = true; 

        if(_ccType == EStatusEffect.SLOW) // 둔화
        {
            monsterNav.speed *= 0.5f;
        }
        else if (_ccType == EStatusEffect.DOT_DAMAGE) // 도트뎀
        {
            IsDotted = true;
            StartCoroutine(DottedDamage());
        }
        else if (_ccType == EStatusEffect.STUN) // 스턴
        {
            this.monsterNav.isStopped = true;
            StopCoroutine(TempAttack());
            IsAttack = false;
        }
        else if(_ccType == EStatusEffect.NERF_STAT) // 공격력 & 방어력 감소
        {
            this.Attack *= 0.7f;
            this.Defense *= 0.7f;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        IsDebuffed = false;
        RemoveCC(_ccType);
    }

    // 스탯 원상 복구
    protected void RemoveCC(EStatusEffect _ccType)
    {
        switch(_ccType)
        {
            case EStatusEffect.SLOW:
                monsterNav.speed = Speed;
                break;
            case EStatusEffect.DOT_DAMAGE:
                IsDotted = false;
                break;
            case EStatusEffect.STUN:
                monsterNav.isStopped = false;
                StartCoroutine(TempAttack());
                IsAttack = true;
                break;
            case EStatusEffect.NERF_STAT:
                this.Attack /= 0.7f;
                this.Defense /= 0.7f;
                break;
            default:
                return;
        }
    }

    protected IEnumerator DottedDamage()
    {
        while(IsDotted)
        {
            curHP -= dottedDamage;
            HPbar.SetHPValue(curHP);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
