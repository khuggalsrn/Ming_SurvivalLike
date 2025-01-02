using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Skill : MonoBehaviour
{
    [SerializeField] SkillData skill;
    //
    //
    //
    //
    //
    //
    void Update()
    {
        // y축 기준으로 계속 회전
        transform.Rotate(0, 120f * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerStatus p = PlayerStatus.Instance;
        if (other.CompareTag("Player"))
        {
            if (p.CurSkill[0]+p.CurSkill[0] < 2)
            {
                p.WeaponsSkillLv[p.CurWeaponnum] = "1_1";
                Destroy(gameObject);
            }
        }
    }
}
