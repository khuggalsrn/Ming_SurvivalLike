using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : WeaponAttack
{
    // public WeaponData Weapon;
    // public SkillData skill1;
    // public SkillData skill2;
    // public ParticleSystem Basic;
    // public GameObject BasicEffectObject;
    // public SkillData FlipBurst;
    // public float cur_AttackPower = 0; // 스킬에 의한 배수
    // public float weaponbasicpower = 1; // 무기 상수
    // public float TurnStamina;
    // public AttackElement Element;
    // public virtual float FinalDamage;
    Coroutine basiccor;
    Animator animator => PlayerStatus.Instance.GetComponent<Animator>();
    
    public override bool IsSubWeapon
    {
        get { return isSubWeapon; }
        set { isSubWeapon = value; }
    }
    private bool isSubWeapon = false;
    float AddiRange = 1f;
    readonly int Skill1AttackCount = 2;
    int AddiSkill1AttackCount = 0;
    readonly int FlipAttackCount = 3;
    int AddiFlipAttackCount = 0;
    float Skill2BleedingMultiplier = 1f;
    float Skill2_is_upgraded_zero = 1f;
    float AddPower = 0;
    float BleedingDrain = 0f;
    bool LvMax = false;
    bool Skill1BonusBarrior = false;
    float BonusDamage = 1f;
    float DrainStamina = 0;
    bool CanUltimate = false;

    float ReduceSubWeaponCooltime = 0f;
    int AddiObject = 0;
    int AddiTarget = 0;
    public override float FinalDamage =>
            (weaponbasicpower + AddPower + PlayerStatus.Instance.Value_AdditionalWeaponPower)
            * cur_AttackPower
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
    IEnumerator DestroyParticleSystem(ParticleSystem part, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (part != null)
        {
            part.Stop();
            Destroy(part.gameObject);
        }
    }

    protected override void Start()
    {
        base.Start();
        transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
        Type = DamageType.Piercing;
    }
    private void OnDisable()
    {
        if (basiccor != null) StopCoroutine(basiccor);
        PlayerStatus.Instance.GetComponent<PlayerAttack>().AttackFalse();
    }
    public override float SubWeaponAttack(int num)
    {
        cur_AttackPower = 1;
        StartCoroutine(Sub_SpearShot(num));
        return Weapon.SubWeaponCooltime - ReduceSubWeaponCooltime;
    }
    IEnumerator Sub_SpearShot(int num)
    {
        for (int i = 0; i < 1 + AddiObject; i++)
        {
            float dmg = (weaponbasicpower + AddPower + PlayerStatus.Instance.WeaponsLv[num] * PlayerStatus.Instance.StatPerLv[0])
            * cur_AttackPower
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
            GameObject temp = Instantiate(Weapon.SubWeapon, PlayerStatus.Instance.transform);
            temp.transform.SetParent(null, true);
            temp.transform.localScale *= PlayerStatus.Instance.Value_AttackRange;
            temp.GetComponent<SpearSkillObject>().FinalDamage = dmg;
            temp.GetComponent<SpearSkillObject>().RemainTargetNum += AddiTarget;
            yield return new WaitForSeconds(0.1f);
        }
    }
    public override void Reset()
    {
        AddiRange = 1f;
        AddPower = 0;
        BleedingDrain = 0f;
        animator.SetFloat("SpearAniSpeed", 1f);
        AddiSkill1AttackCount = 0;
        Skill1BonusBarrior = false;
        Skill2BleedingMultiplier = 1f;
        Skill2_is_upgraded_zero = 1f;
        AddiFlipAttackCount = 0;
        TurnStamina = 0;
        BonusDamage = 1f;
        DrainStamina = 0f;
        CanUltimate = false;
        LvMax = false;

        ReduceSubWeaponCooltime = 0f;
        AddiObject = 0;
        AddiTarget = 0;
    }
    public override void LevelSet(int i)
    {
        var p = PlayerStatus.Instance;
        if (isSubWeapon)
        {
            switch (i)
            {
                case 4:
                    AddiTarget += 5;
                    goto case 3;
                case 3:
                    AddiObject += 1;
                    ReduceSubWeaponCooltime += 0.2f;
                    AddPower += 2f;
                    goto case 2;
                case 2:
                    AddiTarget += 1;
                    ReduceSubWeaponCooltime += 0.2f;
                    goto case 1;
                case 1:
                    AddiObject += 1;
                    break;
                default:
                    break;
            }
        }
        else
            switch (i)
            {
                case 9:
                    
                    goto case 8;
                case 8:
                    goto case 7;
                case 7:
                    UltimateMode();
                    goto case 6;
                case 6:
                    goto case 5;
                case 5:
                    goto case 4;
                case 4:
                    goto case 3;
                case 3:
                    AddiRange += 0.15f;
                    goto case 2;
                case 2:
                    if (p.WeaponsSkillLv[p.CurWeaponnum].Split("_").Select(int.Parse).ToArray().Sum() < 2)
                    {
                        p.WeaponsSkillLv[p.CurWeaponnum] = "1_1";
                    }
                    AddPower += 2f;
                    break;
                case 1:
                    if (p.WeaponsSkillLv[p.CurWeaponnum].Split("_").Select(int.Parse).ToArray().Sum() < 1)
                    {
                        p.WeaponsSkillLv[p.CurWeaponnum] = Random.Range(0, 2) == 0 ? "0_1" : "1_0";
                    }
                    break;
                default:
                    break;
            }
    }

}
