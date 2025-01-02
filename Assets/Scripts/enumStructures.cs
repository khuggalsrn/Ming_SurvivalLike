using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Aug
{
    WeaponSlot1Lv, WeaponSlot2Lv, WeaponSlot3Lv, WeaponSlot4Lv, LastDamage, 
    Speed, Defense, AddiHp, HpRecovery, GainItemRange, 
    AttackRange, StaminaRecovery, SwordWeapon, FirearmsWeapon, SpearWeapon,
    GloveWeapon, 
}
public enum Stats
{
    None, LastDamage, Speed, Defense, AddiHp,
    HpRecoveryRate, GainItemRange, AttackRange, StaminaRecovery, ExpGainRate,
    CurHpRecover, BarriorGuard, FlipBurstCoolReduceValue, BuffDamage, CurBarriorSet,
    MonsterNumLimit, RangeMultiplier
}

public enum Item
{
    Reroll,
}
public enum SkillType
{
    Normal, Skill1, Skill2, FlipBurst, UltimateAttack, SubWeapon
}
public enum DamageType
{
    Slashing, Piercing, Blunt, Explode
}
public enum Debuff
{
    Lacerated, Exposed,
}