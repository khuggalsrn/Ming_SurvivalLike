using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSkillObject : MonoBehaviour
{
    [SerializeField] ParticleSystem MineEffect;
    [SerializeField] float cur_attackPower;
    [SerializeField] float mineDamage;
    GameObject mine;
    public float cur_AttackPower{
        get { return cur_attackPower; }
        set { cur_attackPower = value;}
    }
    public float MineDamage{
        get { return mineDamage; }
        set { mineDamage = value;}
    }
    //
    //
    //
    //
    //
    //
    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Monster")){
            transform.GetChild(0).GetComponent<MeshCollider>().enabled = true;
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Monster"))
        {
            other.gameObject.GetComponent<MonsterStatus>().OnDamaged(MineDamage);
            ParticleSystem temp = Instantiate(MineEffect, other.transform);
            Destroy(temp.gameObject, 5f);
        }
    }
}
