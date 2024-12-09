using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    GameObject WeaponInRightHand;
    GameObject WeaponInLeftHand;
    [SerializeField] GameObject CurWeaponObject;
    [SerializeField] WeaponData CurWeaponData;
    [SerializeField] int CurWeaponnum;
    [SerializeField] bool isAttacking = false;
    [SerializeField] int curattacknum = -1;
    List<WeaponData> InvWeapon => PlayerStatus.Instance.InvWeapon;
    Animator animator;
    int CurSkillnum => PlayerStatus.Instance.CurSkillLv;
    float cur_AttackPower = 0; // 무기 상수, 스킬에 의한 배수
    float Skill1Stamina = 40f;
    float Skill2Stamina = 95f;
    void Start(){
        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();
        WeaponInRightHand = GameObject.FindWithTag("RightWeapon");
        WeaponInLeftHand = GameObject.FindWithTag("LeftWeapon");

        
        CurWeaponnum = 0;
        CurWeaponData = InvWeapon[0];
        cur_AttackPower = CurWeaponData.WeaponPower;
        
        foreach(WeaponData item in InvWeapon){
            Instantiate(item.Item,WeaponInRightHand.transform).SetActive(false);//무기생성
        }
        WeaponInRightHand.transform.GetChild(CurWeaponnum).gameObject.SetActive(true);
        CurWeaponObject = WeaponInRightHand.transform.GetChild(CurWeaponnum).gameObject;
        PlayerStatus.Instance.WeaponsLabel[0] = CurWeaponData.WeaponLabel;
        
        // status.AdditionalWeaponPower = 0; 필요없음
    }
    void Update(){


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            Time.timeScale = 3f;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
        }
#endif

        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking)
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) &&  !isAttacking)
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) &&  !isAttacking)
        {
            SwitchWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) &&  !isAttacking)
        {
            SwitchWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Q) && CurSkillnum > 0 && !isAttacking && PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum]>=Skill1Stamina)
        {
            isAttacking = true;
            animator.SetTrigger(CurWeaponData.skill1.animationname);
            cur_AttackPower *= CurWeaponData.skill1.SkillAttackPower;
            curattacknum = 1;
        }
        else if (Input.GetKeyDown(KeyCode.E) && CurSkillnum > 1 && !isAttacking && PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum]>=Skill2Stamina)
        {
            isAttacking = true;
            animator.SetTrigger(CurWeaponData.skill2.animationname);
            cur_AttackPower *= CurWeaponData.skill2.SkillAttackPower;
            curattacknum = 2;
        }
    }
    void FixedUpdate(){
        if (!isAttacking && curattacknum == -1)
        {
            curattacknum = 0;
            animator.SetTrigger(CurWeaponData.itemName);
            CurWeaponObject.GetComponent<WeaponAttack>().OnAttack(0);
        }
        
    }
    public void AttackTrue(){
        float consumemp = curattacknum == 2? Skill2Stamina : Skill1Stamina;
        PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] -= consumemp;
        CurWeaponObject.GetComponent<WeaponAttack>().OnAttack(curattacknum);
        isAttacking = true;
    }
    public void AttackFalse(){
        isAttacking = false;
        CurWeaponObject.GetComponent<WeaponAttack>().AttackFalse(curattacknum);
        cur_AttackPower = CurWeaponData.WeaponPower;
        curattacknum = -1;
    }
    
    public void Fire(int weapon)
    {
        if(CurWeaponObject.GetComponent<GunAttack>() == null) return;
        if(weapon !=1 ) CurWeaponObject.GetComponent<GunAttack>().Fire(weapon);
        else CurWeaponObject.GetComponent<GunAttack>().Fire(weapon, "Sniper");
    }
    void SwitchWeapon(int weaponIndex)
    {
        // 무기 인덱스가 유효한지 확인
        if (CurWeaponnum != weaponIndex && weaponIndex < InvWeapon.Count)
        {
            WeaponInRightHand.transform.GetChild(CurWeaponnum).gameObject.SetActive(false);
            WeaponInRightHand.transform.GetChild(weaponIndex).gameObject.SetActive(true);

            CurWeaponData = InvWeapon[weaponIndex];
            cur_AttackPower = CurWeaponData.WeaponPower;
            CurWeaponObject = WeaponInRightHand.transform.GetChild(weaponIndex).gameObject;
            
            
            PlayerStatus.Instance.WeaponsLv[CurWeaponnum] = PlayerStatus.Instance.StatusLv[0];
            PlayerStatus.Instance.StatusLv[0] = PlayerStatus.Instance.WeaponsLv[weaponIndex];
            PlayerStatus.Instance.WeaponsSkillLv[CurWeaponnum] = PlayerStatus.Instance.CurSkillLv;
            PlayerStatus.Instance.CurSkillLv = PlayerStatus.Instance.WeaponsSkillLv[weaponIndex];
            CurWeaponnum = weaponIndex;
            PlayerStatus.Instance.CurWeaponnum = CurWeaponnum;
            Debug.Log($"무기가 변경되었습니다: {CurWeaponData.itemName}");
            if(PlayerStatus.Instance.FlipBurstCooltime[CurWeaponnum] <= 0f && CurWeaponObject.GetComponent<WeaponAttack>().CanFlipBurst){
                isAttacking = true;
                animator.SetTrigger(CurWeaponData.FlipBurst.animationname);
                if (CurWeaponObject.GetComponent<WeaponAttack>().OnAttack(3))
                    PlayerStatus.Instance.FlipBurstCooltime[CurWeaponnum] = 20f;
#if UNITY_EDITOR
    PlayerStatus.Instance.FlipBurstCooltime[CurWeaponnum] = 2f;
#endif
        
            }
        }
        else
        {
            Debug.LogWarning($"무기 인덱스 {weaponIndex}가 유효하지 않습니다.");
        }
    }
    
}
