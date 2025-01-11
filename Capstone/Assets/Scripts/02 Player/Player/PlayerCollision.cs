using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    private void OnCollisionEnter(Collision _other)
    {
        switch (_other.gameObject.tag)
        {
            case ValueDefinition.GROUND_TAG:
                IsGround = true;
                break;
            case ValueDefinition.MONSTER_ATTACK_TAG:    // 몬스터 공격에 피격
                Debug.Log("collider");
                MonsterAttack monsterAttack = _other.gameObject.GetComponent<MonsterAttack>();
                if (monsterAttack != null) GetHit(monsterAttack.attack);
                // 피격 파티클? 추가
                Destroy(_other.gameObject);
                break;
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        switch (_other.gameObject.tag)
        {
            case ValueDefinition.MONSTER_ATTACK_TAG:    // 몬스터 공격에 피격
                Debug.Log("trigger");
                MonsterAttack monsterAttack = _other.gameObject.GetComponent<MonsterAttack>();
                if (monsterAttack != null) GetHit(monsterAttack.attack);
                // 피격 파티클? 추가
                Destroy(_other.gameObject);
                break;
        }
    }
}
