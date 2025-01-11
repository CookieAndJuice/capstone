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
            case ValueDefinition.MONSTER_ATTACK_TAG:    // ���� ���ݿ� �ǰ�
                Debug.Log("collider");
                MonsterAttack monsterAttack = _other.gameObject.GetComponent<MonsterAttack>();
                if (monsterAttack != null) GetHit(monsterAttack.attack);
                // �ǰ� ��ƼŬ? �߰�
                Destroy(_other.gameObject);
                break;
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        switch (_other.gameObject.tag)
        {
            case ValueDefinition.MONSTER_ATTACK_TAG:    // ���� ���ݿ� �ǰ�
                Debug.Log("trigger");
                MonsterAttack monsterAttack = _other.gameObject.GetComponent<MonsterAttack>();
                if (monsterAttack != null) GetHit(monsterAttack.attack);
                // �ǰ� ��ƼŬ? �߰�
                Destroy(_other.gameObject);
                break;
        }
    }
}
