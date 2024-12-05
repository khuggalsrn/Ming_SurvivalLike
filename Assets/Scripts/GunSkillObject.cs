using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSkillObject : MonoBehaviour
{
    [SerializeField] float cur_attackPower;
    [SerializeField] float mineDamage;
    GameObject mine5;
    GameObject mine6;
    public float cur_AttackPower{
        get { return cur_attackPower; }
        set { cur_attackPower = value;}
    }
    public float MineDamage{
        get { return mineDamage; }
        set { mineDamage = value;}
    }    
    public GameObject Mine5{
        get { return mine5; }
        set { mine5 = value;}
    }
    public GameObject Mine6{
        get { return mine6; }
        set { mine6 = value;}
    }
    void Start(){
        cur_AttackPower = 6;
        Mine5 = gameObject;
        cur_AttackPower = 15;
        Mine6 = gameObject;
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
            other.gameObject.GetComponent<MonsterStatus>().OnDamaged(MineDamage * cur_AttackPower);
        }
    }
}
