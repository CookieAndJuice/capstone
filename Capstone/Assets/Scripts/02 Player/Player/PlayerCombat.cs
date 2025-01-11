using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController
{
    [SerializeField]
    private Transform attackPos;
    [SerializeField]
    private GameObject attackObject;

    [SerializeField]
    private float buffDuration = 10f;

    private PlayerAttack preparedSkill;   // 주문을 말한 후 생성된 스킬 오브젝트

    public ESkill CurSkill { get; private set; }

    public bool CanAttack { get; private set; }
    public bool IsBuffApplied { get; private set; }
    public bool IsDrain { get; private set; }

    public void PlayerAttack()
    {
        // Test Logic
        //if (CanAttack && AttackTrigger)
        //{
        //    PrepareSkill(ValueDefinition.SPELL2, EEmotion.ENeutral);
        //    UseSkill();
        //}

        // Real Logic
        if (CanAttack)
        {
            if (AttackTrigger && !preparedSkill)
            {
                // 평타 
                StartCoroutine(NormalAttack());
            }
            else if (AttackTrigger && preparedSkill)
            {
                // 스킬
                UseSkill();
            }
        }
    }

    IEnumerator NormalAttack()
    {
        CanAttack = false;

        GameObject attack = Instantiate(attackObject, attackPos.position, attackPos.rotation, PlayManager.PlayerTransform);

        attack.transform.SetParent(attackPos);
        attack.transform.localPosition = new Vector3(0, 0, 0.5f); // 약간 앞쪽으로 위치 보정
        attack.transform.localRotation = Quaternion.identity;

        Rigidbody rb = attack.GetComponent<Rigidbody>();
        if (rb)
        {
            attack.transform.SetParent(null);
            rb.isKinematic = false;
            rb.velocity = attackPos.forward * 20;   // 수치 조정 필요
        }
        yield return new WaitForSeconds(AttackSpeed);
        CanAttack = true;
    }

    // 현재 구현 중인 부분
    public void PrepareSkill(string _spell, EEmotion _emotion)
    {
        for (var idx = 0; idx < (int)ESkill.LAST; idx++)
            if (_spell == GameManager.Skills[idx].Spell)
            {
                CurSkill = (ESkill)idx;
                break;
            }

        preparedSkill = Instantiate(GameManager.Skills[(int)CurSkill],
            attackPos.position,
            attackPos.rotation,
            PlayManager.PlayerTransform);
        
        // Debug.Log($"preparedSkill : {preparedSkill.name}");
        
        if (preparedSkill.SkillType != ESkillType.TELEPORT)
        {
            preparedSkill.transform.SetParent(attackPos);
            preparedSkill.transform.localPosition = new Vector3(0, 0, 0.5f); // 약간 앞쪽으로 위치 보정
        }
        else
            preparedSkill.transform.localPosition = Vector3.zero;
       
        preparedSkill.transform.localRotation = Quaternion.identity;
        preparedSkill.SetSkill(CurSkill, _emotion);
    }

    public void UseSkill()
    {
        switch(preparedSkill.SkillType)
        {
            case ESkillType.SHOOT:
                Rigidbody rb = preparedSkill.GetComponent<Rigidbody>();
                if (rb)
                {
                    preparedSkill.transform.SetParent(null);
                    rb.isKinematic = false;
                    rb.velocity = attackPos.forward * 20;   // 수치 조정 필요
                }
                break;
            case ESkillType.TELEPORT:
                PlayManager.PlayerRigidBody.AddForce(PlayManager.PlayerTransform.forward * 150, ForceMode.Impulse);  // 수치 조정필요2
                Destroy(preparedSkill.gameObject);
                break;
            case ESkillType.WIDE:
                WideSkill wideskill = preparedSkill as WideSkill;
                wideskill.ControlSkill();
                Destroy(preparedSkill.gameObject);
                break;
        }
        if ((int)preparedSkill.StatusEffect >= 0 && (int)preparedSkill.StatusEffect <= 2 && !IsBuffApplied)
            StartCoroutine(ApplyBuff(preparedSkill.StatusEffect));

        preparedSkill = null;
        
    }

    IEnumerator ApplyBuff(EStatusEffect _buff)
    {
        IsBuffApplied = true;
        float elapsedTime = 0f;

        if (_buff == EStatusEffect.ATTACK_UP)        // 공격력 증가
        {
            Attack *= 1.5f;
            
        }
        else if (_buff == EStatusEffect.ATTACK_SPEED_UP) // 공속 증가(== 공격 쿨타임 감소)
        {
            AttackSpeed /= 1.5f;
           
        }
        else if (_buff == EStatusEffect.DRAIN)   // 피흡
        {
            IsDrain = true;
        }

        while (elapsedTime < buffDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetOriginalStat(_buff);
    }

    private void SetOriginalStat(EStatusEffect _buff)
    {
        switch (_buff)
        {
            case EStatusEffect.ATTACK_UP:
                Attack /= 1.5f;
                break;
            case EStatusEffect.ATTACK_SPEED_UP:
                AttackSpeed *= 1.5f;
                break;
            case EStatusEffect.DRAIN:
                IsDrain = false;
                break;
            default:
                return;
        }
        IsBuffApplied = false;
    }

    public override void GetHit(float _damage)
    {
        curHP -= _damage;
        PlayManager.SetPlayerCurHP(curHP);

        if (curHP <= 0 && !isDead)
        {
            PlayerDie(); 
        }
    }


    public void Drain(float _hp)
    {
        if (curHP > 0)
        {
            curHP += _hp;
            if (curHP >= PlayManager.MaxHP)
            {
                curHP = PlayManager.MaxHP;
            }
            PlayManager.SetPlayerCurHP(curHP);
        }
    }
}
