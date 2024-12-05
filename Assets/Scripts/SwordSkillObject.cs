using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillObject : MonoBehaviour
{
    public enum skill{
        Skill1, Skill2, FlipBurst, Ultimate
    }
    [SerializeField] ParticleSystem MonserHitEffect;
    [SerializeField] skill Objectnum;
    float finalDamage;
    public float FinalDamage{
        get { return finalDamage; }
        set { finalDamage = value; }
    }
    void Start(){
        if (Objectnum == skill.Skill2){
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (Objectnum == skill.FlipBurst){
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (Objectnum == skill.FlipBurst){
            transform.GetChild(2).gameObject.SetActive(true);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Monster"))
        {
            other.gameObject.GetComponent<MonsterStatus>().OnDamaged(FinalDamage);
            ParticleSystem temp = Instantiate(MonserHitEffect, other.transform);
            Destroy(temp.gameObject, 5f);

        }
    }
}
