using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSkill : PlayerAttack
{
    [SerializeField]
    private GameObject fire;
    [SerializeField]
    private GameObject explosion;

    private MonsterScript monster;

    public void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.CompareTag(ValueDefinition.MONSTER_TAG))
        {
            monster = _other.gameObject.GetComponent<MonsterScript>();

            if (monster != null)
            {
                StartCoroutine(FireExplosion());
            }
        }
    }

    IEnumerator FireExplosion()
    {
        GameObject fireInstance = Instantiate(fire, monster.transform.position, Quaternion.identity, monster.transform);

        ParticleSystem fireParticle = fireInstance.GetComponent<ParticleSystem>();
        var mainModule = fireParticle.main;
        float realDuration = mainModule.duration / mainModule.simulationSpeed;
        yield return new WaitForSeconds(realDuration);
        
        Destroy(fireInstance);
    }
}
