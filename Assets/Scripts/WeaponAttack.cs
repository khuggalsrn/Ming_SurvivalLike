using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponAttack : MonoBehaviour
{
    [SerializeField] protected WeaponData Weapon;
    [SerializeField] protected float turnStamina = 0;
    [SerializeField] protected float cur_AttackPower = 1; // 스킬에 의한 배수
    [SerializeField] protected float weaponbasicpower = 1; // 무기 상수
    [SerializeField] protected DamageType type;
    public virtual bool IsSubWeapon
    {
        get { return false; }
        set {}
    }
    public virtual float FinalDamage => -1;
    public virtual float TurnStamina
    {
        get { return turnStamina; }
        set { turnStamina = value; }
    }
    public virtual DamageType Type
    {
        get { return type; }
        set { type = value; }
    }
    public virtual void UltimateMode()
    {
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
    public virtual float SubWeaponAttack(int num)
    {
        return 10f;
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
    protected virtual void ApplyDamage(GameObject target)
    {
    }
}
