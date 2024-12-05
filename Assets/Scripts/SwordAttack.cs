using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SwordAttack : WeaponAttack
{
    // public ItemData Weapon;
    // public SkillData skill1;
    // public SkillData skill2;
    // public ParticleSystem Basic;
    // public GameObject BasicEffectObject;
    // public SkillData FlipBurst;
    // public float cur_AttackPower = 0; // 스킬에 의한 배수
    // public float weaponbasicpower = 1; // 무기 상수
    // public bool CanFlipBurst;
    Coroutine basiccor;
    float AddPower = 0;
    float Drain = 0f;
    bool Skill1BonusBarrior = false;
    float BonusDamage = 1f;
    float DrainStamina = 0;
    bool CanUltimate = false;
    public SkillData Ultimate;
    float FinalDamage => 
            (weaponbasicpower + AddPower + PlayerStatus.Instance.Value_AdditionalWeaponPower) 
            * cur_AttackPower 
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
    protected override void Start()
    {
        base.Start();
        transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
    }
    public override bool OnAttack(int atk)
    {
        base.OnAttack(atk);
        if (basiccor != null) StopCoroutine(basiccor);
        if (CanUltimate) StartCoroutine(UltimateEffect(Ultimate.Effect));
        switch (atk)
        {
            case 0:
                cur_AttackPower = weaponbasicpower;
                basiccor = StartCoroutine(BasicEffect(Basic));
                return true;
            case 1:
                cur_AttackPower = skill1.SkillAttackPower;
                StartCoroutine(Skill1Effect1(skill1.Effect));
                StartCoroutine(Skill1Effect2(skill1.Effect));
                StartCoroutine(Skill1Effect3(skill1.Effect));
                return true;
            case 2:
                cur_AttackPower = skill2.SkillAttackPower;
                StartCoroutine(Skill2Effect(skill2.Effect));
                return true;
            case 3:
                if (CanFlipBurst)
                {
                    cur_AttackPower = FlipBurst.SkillAttackPower;
                    StartCoroutine(FlipBurstEffect(FlipBurst.Effect));
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                return false;
        }
        
    }
    public override void AttackFalse(int atk)
    {
        // Debug.Log("스워드어택 false 하는거 없음 이제");
        // transform.GetChild(atk).GetComponent<BoxCollider>().gameObject.SetActive(false);

    }
    IEnumerator BasicEffect(ParticleSystem part)
    {
        yield return new WaitForSeconds(0.4f);
        transform.GetChild(0).gameObject.GetComponent<BoxCollider>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        ParticleSystem temp = Instantiate(part, PlayerStatus.Instance.transform);
        temp.transform.localScale *= PlayerStatus.Instance.Value_AttackRange / PlayerStatus.Instance.transform.localScale.x;
        temp.transform.SetParent(PlayerStatus.Instance.transform);
        temp.transform.position = gameObject.transform.position + PlayerStatus.Instance.transform.forward;
        Destroy(temp.gameObject, 5f);
        yield return new WaitForSeconds(0.3f);
        transform.GetChild(0).gameObject.GetComponent<BoxCollider>().enabled = false;
    }
    IEnumerator Skill1Effect1(ParticleSystem part)
    {
        Debug.Log("Eff1");
        transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        ParticleSystem temp = Instantiate(part, gameObject.transform);
        temp.transform.localScale *= PlayerStatus.Instance.Value_AttackRange / 2f;
        Destroy(temp.gameObject, 5f);
        yield return new WaitForSeconds(0.14f);
        transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
    }
    IEnumerator Skill1Effect2(ParticleSystem part)
    {
        Debug.Log("Eff2");
        if(Skill1BonusBarrior) PlayerStatus.Instance.UpBarrior(0.1f);
        yield return new WaitForSeconds(0.3f);
        transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        ParticleSystem temp = Instantiate(part, gameObject.transform);
        temp.transform.localScale *= PlayerStatus.Instance.Value_AttackRange / 2f;
        Destroy(temp.gameObject, 5f);
        yield return new WaitForSeconds(0.4f);
        transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
    }
    IEnumerator Skill1Effect3(ParticleSystem part)
    {
        Debug.Log("Eff3");
        yield return new WaitForSeconds(0.8f);
        transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
        ParticleSystem temp = Instantiate(part, gameObject.transform);
        temp.transform.localScale *= PlayerStatus.Instance.Value_AttackRange / 2f;
        Destroy(temp.gameObject, 5f);
        yield return new WaitForSeconds(0.2f);
        transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
        cur_AttackPower = 1;
    }
    IEnumerator Skill2Effect(ParticleSystem part)
    {
        GameObject temp = Instantiate(skill2.SkillObject);
        temp.transform.localScale = new Vector3(1, 1, 1) * PlayerStatus.Instance.Value_AttackRange;
        temp.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        temp.GetComponent<SwordSkillObject>().FinalDamage = FinalDamage;

        ParticleSystem temp2 = Instantiate(part, temp.transform);
        Destroy(temp2.gameObject, 5f);

        yield return new WaitForSeconds(0.2f);
        cur_AttackPower = 1;
        DestroyImmediate(temp);
    }
    IEnumerator FlipBurstEffect(ParticleSystem part)
    {
        GameObject temp = Instantiate(FlipBurst.SkillObject);
        temp.transform.localScale = new Vector3(1, 1, 1) * PlayerStatus.Instance.Value_AttackRange;
        temp.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        temp.GetComponent<SwordSkillObject>().FinalDamage = FinalDamage;

        ParticleSystem temp2 = Instantiate(part);
        temp2.transform.localScale *= PlayerStatus.Instance.Value_AttackRange;
        temp2.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        Destroy(temp2.gameObject, 5f);

        yield return new WaitForSeconds(1f);
        cur_AttackPower = 1;
        DestroyImmediate(temp);

    }
    IEnumerator UltimateEffect(ParticleSystem part)
    {
        GameObject temp = Instantiate(Ultimate.SkillObject);
        temp.transform.localScale = new Vector3(1, 1, 1) * PlayerStatus.Instance.Value_AttackRange;
        temp.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        temp.GetComponent<SwordSkillObject>().FinalDamage = FinalDamage;

        ParticleSystem temp2 = Instantiate(part);
        temp2.transform.localScale *= PlayerStatus.Instance.Value_AttackRange;
        temp2.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        Destroy(temp2.gameObject, 5f);

        yield return new WaitForSeconds(0.75f);
        cur_AttackPower = 1;
        DestroyImmediate(temp);
    }
    public override void Reset(){
        
        AddPower = 0;
        Drain = 0f;
        CanFlipBurst = false;
        Skill1BonusBarrior = false;
        BonusDamage = 1f;
        DrainStamina = 0f;
        CanUltimate = false;
    }
    public override void LevelSet(int i)
    {
        switch (i)
        {
            case 9:
                CanUltimate = true;
                goto case 8;
            case 8:
                AddPower += 10;
                goto case 7;
            case 7:
                Drain += 1.8f;
                goto case 6;
            case 6:
                DrainStamina = 1f;
                goto case 5;
            case 5:
                BonusDamage = 1.2f;
                goto case 4;
            case 4:
                Skill1BonusBarrior = true;
                goto case 3;
            case 3:
                CanFlipBurst = true;
                goto case 2;
            case 2:
                Drain += 0.2f;
                goto case 1;
            case 1:
                AddPower += 2;
                break;
        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            PlayerStatus.Instance.HitPoint += Drain;
            PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] += DrainStamina;
            other.gameObject.GetComponent<MonsterStatus>().OnDamaged(FinalDamage);
            Debug.Log(FinalDamage);
        }
    }
}
