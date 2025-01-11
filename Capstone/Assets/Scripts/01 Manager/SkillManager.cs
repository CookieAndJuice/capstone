using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillManager : MonoBehaviour
{

    private string[] spells = { ValueDefinition.SPELL1, ValueDefinition.SPELL2, ValueDefinition.SPELL3 };

    public PlayerAttack[] skills = new PlayerAttack[(int)ESkill.LAST];

    private void SetSkillSpell()
    {
        for (var i = 0; i < skills.Length; i++)
            skills[i].Spell = spells[i];
    }

    public void SetManager()
    {
        SetSkillSpell();
    }
}
