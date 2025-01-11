using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEmotion
{
    EHappy,
    EAngry,
    EDisgust,
    EFear,
    ENeutral,
    ESad,
    ESurprise,
    LAST
}

public enum EStatusEffect
{
    // 캐릭터 버프
    ATTACK_UP,
    ATTACK_SPEED_UP,
    DRAIN,      

    // 몬스터 디버프
    SLOW,
    DOT_DAMAGE,
    STUN,
    NERF_STAT,
    LAST
} 

public enum ESkill
{
    TempSkill1,
    TempSkill2,
    TempSkill3,
    LAST
}

public enum ESkillType
{
    NORMAL,
    SHOOT,
    TELEPORT,
    WIDE,
    LAST
}