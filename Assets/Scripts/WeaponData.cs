using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string itemName;           // 아이템 이름
    // public float BasicAttackPower;         // 기본 공격력
    public int WeaponLabel; //무기 번호
    public float WeaponPower;         // 기본 공격력
    public GameObject Item;          // 실제 무기 오브젝트
    public ParticleSystem Basic;
    public GameObject BasicEffectObject;
    public int skillLv;              //해금된 스킬 몇개? 0,1,2
    public SkillData skill1;         // 스킬 1
    public float skill1Stamina => skill1.NeedStamina;
    public SkillData skill2;         // 스킬 2
    public float skill2Stamina => skill2.NeedStamina;
    public SkillData FlipBurst;         // 플립버스트
    public SkillData Ultimate;         // 플립버스트
    public Texture2D WeaponImage;
    public GameObject SubWeapon;
    public float SubWeaponCooltime;
}
