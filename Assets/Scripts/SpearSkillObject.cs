using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSkillObject : SkillObject
{
    
    // [SerializeField] protected ParticleSystem MonserHitEffect;
    // [SerializeField] protected float finalDamage;
    // public float FinalDamage{
    //     get { return finalDamage; }
    //     set { finalDamage = value; }
    // }
    public int RemainTargetNum = 1;
    //
    //
    //
    //
    //
    //
    void Start()
    {
        Destroy(this.gameObject, 2f);
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().velocity = PlayerStatus.Instance.transform.forward * 5;
        }
        else
        {
            gameObject.AddComponent<Rigidbody>().velocity = PlayerStatus.Instance.transform.forward * 5;
        }
        transform.LookAt(transform.position + PlayerStatus.Instance.transform.forward);
        transform.Rotate(0,-90,-90);
        transform.position += Vector3.up * 2;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            other.gameObject.GetComponent<MonsterStatus>().OnDamaged(FinalDamage, DamageType.Piercing);
            ParticleSystem temp = Instantiate(MonserHitEffect, other.transform);
            temp.transform.SetParent(null, true);
            Destroy(temp.gameObject, 10f);
            RemainTargetNum -= 1;
            if (RemainTargetNum < 1) Destroy(gameObject, 0.1f);
        }
    }
}
