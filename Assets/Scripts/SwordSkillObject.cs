using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillObject : SkillObject
{
    // [SerializeField] protected ParticleSystem MonserHitEffect;
    // [SerializeField] protected float finalDamage;
    // public float FinalDamage{
    //     get { return finalDamage; }
    //     set { finalDamage = value; }
    // }
    [SerializeField] SkillType Objectnum;
    /// <summary>
    /// 0 : FinalDamage / 1 : DebuffDamageMultiplier / 2 : DebuffHitCount / 3 : Drain
    /// </summary>
    public float[] InitializeParam;
    public float DebuffDamageMultiplier = 1f;
    public int DebuffHitCount = 1;
    public float Drain = 0f;
    void Start()
    {
        FinalDamage = InitializeParam[0];
        DebuffDamageMultiplier = InitializeParam[1];
        DebuffHitCount = (int)InitializeParam[2];
        Drain = InitializeParam[3];
        if (Objectnum == SkillType.Skill2)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            Destroy(this.gameObject, 0.6f);
        }
        else if (Objectnum == SkillType.FlipBurst)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            Destroy(this.gameObject, 0.15f);
        }
        else if (Objectnum == SkillType.UltimateAttack)
        {
            transform.GetChild(2).gameObject.SetActive(true);
            Destroy(this.gameObject, 0.15f);
        }
        else if (Objectnum == SkillType.SubWeapon)
        {
            DebuffDamageMultiplier = 0;
            DebuffHitCount = 0;
            transform.GetChild(3).gameObject.SetActive(true);
            Destroy(this.gameObject, 0.5f);
        }
    }
    void FixedUpdate(){
        if (Objectnum == SkillType.SubWeapon)
        {
            transform.Rotate(0, 720 * Time.fixedDeltaTime, 0);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            if (Objectnum == SkillType.SubWeapon) PlayerStatus.Instance.HitPoint += FinalDamage * Drain;
            other.gameObject.GetComponent<MonsterStatus>().OnDamaged(FinalDamage, DamageType.Slashing);
            ApplyDebuff(other.gameObject, DebuffHitCount, DebuffDamageMultiplier);
            ParticleSystem temp = Instantiate(MonserHitEffect, other.transform);
            temp.transform.SetParent(null, true);
            Destroy(temp.gameObject, 5f);

        }
    }
    void ApplyDebuff(GameObject target, int time, float value = 1f)
    {
        if (target.gameObject.GetComponent<MonsterStatus>().GetDebuff(
        Debuff.Lacerated, DamageType.Slashing, time, value * GameManager.Instance.Debuffs[(int)Debuff.Lacerated].AddiDamageRatio))
        {
            PlayerStatus.Instance.HitPoint += value * GameManager.Instance.Debuffs[(int)Debuff.Lacerated].AddiDamageRatio * Drain;
        }
    }
}
