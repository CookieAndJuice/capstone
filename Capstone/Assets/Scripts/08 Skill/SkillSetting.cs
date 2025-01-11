using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerAttack
{
    public int emotionNum;

    public bool IsDrained { get; private set; }

    public void SetEmotion(EEmotion _emotion)
    {
        // 시각효과 적용을 위해 있는데 뭐 더 넣을 듯
        switch (_emotion)
        {
            case EEmotion.EHappy:
                
                break;
            case EEmotion.EAngry:
                
                break;
            case EEmotion.ENeutral:
                
                break;
            case EEmotion.EDisgust:
                
                break;
            case EEmotion.EFear:
                
                break;
            case EEmotion.ESad:
                
                break;
            case EEmotion.ESurprise:
                
                break;
            default: 
                return;
        }
        StatusEffect = (EStatusEffect)_emotion;
        PlayManager.SetEmotionColor(_emotion);
    }

    public void SetSkillType(ESkill _skill)
    {
        Skill = _skill;
    }
}

