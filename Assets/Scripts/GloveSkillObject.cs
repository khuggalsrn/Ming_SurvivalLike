using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveSkillObject : SkillObject
{
    
    // [SerializeField] protected ParticleSystem MonserHitEffect;
    // [SerializeField] protected float finalDamage;
    // public float FinalDamage{
    //     get { return finalDamage; }
    //     set { finalDamage = value; }
    // }
    //
    //
    //
    //
    //
    //
    void Start()
    {
        Destroy(this.gameObject, 0.5f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            other.gameObject.GetComponent<MonsterStatus>().OnDamaged(FinalDamage, DamageType.Blunt);
            ParticleSystem temp = Instantiate(MonserHitEffect, other.transform);
            temp.transform.SetParent(null, true);
            Destroy(temp.gameObject, 10f);
        }
    }
}
