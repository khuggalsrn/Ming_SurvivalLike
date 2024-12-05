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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerStatus.Instance.CurSkillLv < 2)
            {
                PlayerStatus.Instance.CurSkillLv += 1;
                PlayerStatus.Instance.WeaponsSkillLv[
                PlayerStatus.Instance.CurWeaponnum] += 1;
                Destroy(gameObject);
            }
        }
    }
}
