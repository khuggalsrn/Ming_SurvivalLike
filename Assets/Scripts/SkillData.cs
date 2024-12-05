using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillname;           // 스킬 이름
    public float SkillAttackPower;         // 공격력, 무기 공격력의 배수
    public float Cooltime;         // 스킬 쿨타임
    public ParticleSystem Effect;               // 스킬 이펙트
    public string animationname;            // 스킬 애니메이션
    public GameObject SkillObject;            // 스킬 애니메이션

}