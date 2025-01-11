using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WideSkill : PlayerAttack
{
    [SerializeField]
    private GameObject SkillEffect;

    [SerializeField]
    private float detectionRadius = 6.0f;

    public void ControlSkill()
    {
        DetectMonstersInArea();
    }

    private void DetectMonstersInArea()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, 1 << 8);
        List<MonsterScript> monstersInRange = new List<MonsterScript>();

        foreach (Collider hitCollider in hitColliders)
        {
            MonsterScript monster = hitCollider.GetComponent<MonsterScript>();

            if (monster != null)
            {
                GameObject skillEffect = Instantiate(SkillEffect, monster.transform.position, Quaternion.identity, monster.transform);
                StartCoroutine(SkillHit(skillEffect));
            }
        }
    }

    IEnumerator SkillHit(GameObject _skillEffect)
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(_skillEffect);
    }
}
