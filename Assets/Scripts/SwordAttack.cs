using System.Collections;
using System.Linq;
using UnityEngine;

public class SwordAttack : WeaponAttack
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
    [SerializeField] Mesh UltWeapon;
    [SerializeField] Material UltWeaponMat;
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
    float Drain = 0f;
    public override float FinalDamage =>
            (weaponbasicpower + AddPower + PlayerStatus.Instance.Value_AdditionalWeaponPower)
            * cur_AttackPower
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
    protected override void Start()
    {
        base.Start();
        transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
        Type = DamageType.Slashing;
    }
    private void OnDisable()
    {
        if (basiccor != null) StopCoroutine(basiccor);
        PlayerStatus.Instance.GetComponent<PlayerAttack>().AttackFalse();
    }
    public override bool OnAttack(int atk)
    {
        base.OnAttack(atk);
        float Scale = PlayerStatus.Instance.Value_AttackRange * AddiRange;
        if (basiccor != null) StopCoroutine(basiccor);
        if (CanUltimate) StartCoroutine(UltimateEffect(Weapon.Ultimate.Effect, Scale));
        switch (atk)
        {
            case 0:
                cur_AttackPower = weaponbasicpower;
                basiccor = StartCoroutine(BasicEffect(Weapon.Basic, Scale / PlayerStatus.Instance.transform.localScale.x));
                return true;
            case 1:
                cur_AttackPower = Weapon.skill1.SkillAttackPower;
                StartCoroutine(Skill1Effect(Weapon.skill1.Effect, Scale));
                return true;
            case 2:
                cur_AttackPower = Weapon.skill2.SkillAttackPower;
                StartCoroutine(Skill2Effect(Weapon.skill2.Effect, Scale));
                return true;
            case 3:
                cur_AttackPower = Weapon.FlipBurst.SkillAttackPower;
                StartCoroutine(FlipBurstEffect(Weapon.FlipBurst.Effect, Scale));
                return true;
            default:
                return false;
        }

    }
    IEnumerator BasicEffect(ParticleSystem part, float scale)
    {
        yield return StartCoroutine(SkillManager.Instance.SwordBasicEffect(part, scale, transform, animator.GetFloat("SwordAniSpeed")));
        basiccor = null;
    }
    IEnumerator Skill1Effect(ParticleSystem part, float scale)
    {
        yield return StartCoroutine(SkillManager.Instance.SwordSkill1Effect1(part, scale, transform, animator.GetFloat("SwordAniSpeed")));
        if (Skill1BonusBarrior) PlayerStatus.Instance.UpBarrior(0.1f);
        yield return StartCoroutine(SkillManager.Instance.SwordSkill1Effect2(part, scale, transform, animator.GetFloat("SwordAniSpeed")));
        if (AddiSkill1AttackCount == 1)
            yield return StartCoroutine(SkillManager.Instance.SwordSkill1Effect3(part, scale, transform, animator.GetFloat("SwordAniSpeed")));
        cur_AttackPower = 1;
        TurnStaminaAfterSkill();
    }
    IEnumerator Skill2Effect(ParticleSystem part, float scale)
    {
        float[] Abillity = new float[4] { FinalDamage * Skill2_is_upgraded_zero, FinalDamage * Skill2BleedingMultiplier, 1, BleedingDrain };
        yield return StartCoroutine(SkillManager.Instance.SwordSkill2Effect(part, scale, Weapon, Abillity, animator.GetFloat("SwordAniSpeed")));
        cur_AttackPower = 1;
        TurnStaminaAfterSkill();
    }
    IEnumerator FlipBurstEffect(ParticleSystem part, float scale)
    {
        float[] Abillity = new float[4] { FinalDamage, FinalDamage, FlipAttackCount + AddiFlipAttackCount, BleedingDrain };
        yield return StartCoroutine(SkillManager.Instance.SwordFlipBurstEffect(part, scale, Weapon, Abillity, animator.GetFloat("SwordAniSpeed")));
        cur_AttackPower = 1;
        TurnStaminaAfterSkill();
    }
    IEnumerator UltimateEffect(ParticleSystem part, float scale)
    {
        float[] Abillity = new float[4] { FinalDamage, FinalDamage, 1f, BleedingDrain };
        yield return StartCoroutine(SkillManager.Instance.SwordUltimateEffect(part, scale, Weapon, Abillity, animator.GetFloat("SwordAniSpeed")));
        cur_AttackPower = 1;
    }
    IEnumerator DestroyParticleSystem(ParticleSystem part, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (part != null)
        {
            part.Stop();
            Destroy(part.gameObject);
        }
    }
    public override void AttackFalse(int atk)
    {
        // Debug.Log("스워드어택 false 하는거 없음 이제");
        // transform.GetChild(atk).GetComponent<BoxCollider>().gameObject.SetActive(false);

    }
    public override float SubWeaponAttack(int num)
    {
        Debug.Log("Sub" + num);
        Debug.Log(PlayerStatus.Instance.WeaponsLv[num]);
        float dmg = (weaponbasicpower + AddPower + PlayerStatus.Instance.WeaponsLv[num] * PlayerStatus.Instance.StatPerLv[0])
            * cur_AttackPower
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
        GameObject temp = Instantiate(Weapon.SubWeapon, PlayerStatus.Instance.transform);
        float[] Abillity = new float[4] { dmg, 0, 0, Drain };
        temp.transform.position += Vector3.up * 0.2f;
        temp.transform.localScale *= PlayerStatus.Instance.Value_AttackRange;
        temp.GetComponent<SwordSkillObject>().InitializeParam = Abillity;
        temp.GetComponent<SwordSkillObject>().FinalDamage = dmg;
        temp.GetComponent<SwordSkillObject>().Drain = Drain;
        return Weapon.SubWeaponCooltime - ReduceSubWeaponCooltime;
    }
    public override void Reset()
    {
        AddiRange = 1f;
        AddPower = 0;
        BleedingDrain = 0f;
        animator.SetFloat("SwordAniSpeed", 1f);
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
        Drain = 0f;
    }
    public override void LevelSet(int i)
    {
        var p = PlayerStatus.Instance;
        if (isSubWeapon)
        {
            switch (i)
            {
                case 4:
                    Drain = 0.025f;
                    goto case 3;
                case 3:
                    AddiRange += 0.4f;
                    ReduceSubWeaponCooltime += 0.2f;
                    AddPower += 2f;
                    goto case 2;
                case 2:
                    ReduceSubWeaponCooltime += 0.2f;
                    goto case 1;
                case 1:
                    AddiRange += 0.4f;
                    break;
                default:
                    break;
            }
        }
        else
            switch (i)
            {
                case 9:
                    LvMax = true;
                    goto case 8;
                case 8:
                    animator.SetFloat("SwordAniSpeed", 1.5f);
                    goto case 7;
                case 7:
                    UltimateMode();
                    AddiRange += 0.15f;
                    goto case 6;
                case 6:
                    BleedingDrain = 0.5f;
                    goto case 5;
                case 5:
                    p.WeaponsSkillLv[p.CurWeaponnum] = "1_2";
                    Skill2BleedingMultiplier = 3f;
                    Skill2_is_upgraded_zero = 0f;
                    goto case 4;
                case 4:
                    AddiSkill1AttackCount = 1;
                    AddiFlipAttackCount = 1;
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
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            if (cur_AttackPower == Weapon.skill1.SkillAttackPower)
            {
                float[] Abillity = new float[4] { FinalDamage + (FinalDamage * 0.5f * AddiSkill1AttackCount), FinalDamage, Skill1AttackCount + AddiSkill1AttackCount, BleedingDrain };
                StartCoroutine(SkillManager.Instance.ApplyDamageOfSkill1(other.gameObject, Abillity));
            }
            else
            {
                ApplyDamage(other.gameObject);
                ApplyDebuff(other.gameObject, 1);
            }
        }
    }
    protected override void ApplyDamage(GameObject target)
    {
        PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] += DrainStamina;
        target.gameObject.GetComponent<MonsterStatus>().OnDamaged(FinalDamage, Type);
    }
    void ApplyDebuff(GameObject target, int time, float value = 1f)
    {

        if (target.gameObject.GetComponent<MonsterStatus>().GetDebuff(
        Debuff.Lacerated, DamageType.Slashing, time, FinalDamage * value * GameManager.Instance.Debuffs[(int)Debuff.Lacerated].AddiDamageRatio))
        {
            SkillManager.Instance.BuffAfterBleeding(value * GameManager.Instance.Debuffs[(int)Debuff.Lacerated].AddiDamageRatio * BleedingDrain, LvMax);
        }
    }
    public override void UltimateMode()
    {
        CanUltimate = true;
        GetComponent<MeshFilter>().mesh = UltWeapon;
        GetComponent<MeshRenderer>().material = UltWeaponMat;
        transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0.2f), Quaternion.Euler(-40, 90, 90));
    }
    void TurnStaminaAfterSkill()
    {
        PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] += TurnStamina;
    }
}
