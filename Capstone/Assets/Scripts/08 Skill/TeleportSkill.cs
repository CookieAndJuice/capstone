using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSkill : PlayerAttack
{
    [SerializeField]
    private GameObject HitEffect;
    private MonsterScript monster;

    public void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.CompareTag(ValueDefinition.MONSTER_TAG))
        {
            monster = _other.gameObject.GetComponent<MonsterScript>();

            if (monster != null)
            {
                StartCoroutine(HitMonster());
            }
        }
    }

    IEnumerator HitMonster()
    {
        GameObject HitInstance = Instantiate(HitEffect, monster.transform.position, Quaternion.identity, monster.transform);

        ParticleSystem HitParticle = GetComponent<ParticleSystem>();
        var mainModule = HitParticle.main;
        float realDuration = mainModule.duration / mainModule.simulationSpeed;
        yield return new WaitForSeconds(realDuration);

        Destroy(HitInstance);
    }
}
