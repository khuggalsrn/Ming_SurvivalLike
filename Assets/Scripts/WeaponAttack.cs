using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] protected WeaponData Weapon;
    [SerializeField] protected SkillData skill1;
    [SerializeField] protected SkillData skill2;
    [SerializeField] protected ParticleSystem Basic;
    [SerializeField] protected GameObject BasicEffectObject;
    [SerializeField] protected SkillData FlipBurst;
    [SerializeField] protected float cur_AttackPower = 0; // 스킬에 의한 배수
    [SerializeField] protected float weaponbasicpower; // 무기 상수
    [SerializeField] protected bool canFlipBurst = false;
    public bool CanFlipBurst
    {
        get { return canFlipBurst; }
        set { canFlipBurst = value; }
    }
    protected virtual void Start()
    {
        weaponbasicpower = Weapon.WeaponPower;
    }
    public virtual bool OnAttack(int atk)
    {
        return false;
    }
    public virtual void AttackFalse(int atk)
    {
    }
    public virtual void OnEffect(ParticleSystem part)
    {
    }
    public virtual void Reset()
    {
    }
    public virtual void LevelSet(int i)
    {
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
    }
}
