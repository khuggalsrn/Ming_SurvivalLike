using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    GameObject WeaponInRightHand => PlayerStatus.Instance.WeaponInRightHand;
    GameObject WeaponInLeftHand => PlayerStatus.Instance.WeaponInLeftHand;
    [SerializeField] GameObject CurWeaponObject;
    [SerializeField] WeaponData CurWeaponData;
    [SerializeField] int CurWeaponnum;
    [SerializeField] bool isAttacking = false;
    [SerializeField] int curattacknum = -1;
    List<WeaponData> InvWeapon => PlayerStatus.Instance.InvWeapon;
    Animator animator;
    int[] CurSkill => PlayerStatus.Instance.CurSkill;
    float cur_AttackPower = 0; // 무기 상수, 스킬에 의한 배수
    float Skill1Stamina = 40f;
    float Skill2Stamina = 95f;
    float Skill1MaxCooltime;
    float Skill2MaxCooltime;
    float Skill1Cooltime => PlayerStatus.Instance.SkillCooltime[0];
    float Skill2Cooltime => PlayerStatus.Instance.SkillCooltime[1];
    float SubWeapon_Attacktimer1 = 0;
    float SubWeapon_Attacktimer2 = 0;
    float MainWeapon1FinalDamage => WeaponInRightHand.transform.GetChild(0).GetComponent<WeaponAttack>().FinalDamage;
    float MainWeapon2FinalDamage => WeaponInRightHand.transform.GetChild(1).GetComponent<WeaponAttack>().FinalDamage;
    WeaponAttack SubWeapon1 => WeaponInRightHand.transform.GetChild(2).GetComponent<WeaponAttack>();
    WeaponAttack SubWeapon2 => WeaponInRightHand.transform.GetChild(3).GetComponent<WeaponAttack>();

    public void Init()
    {
        animator = GetComponent<Animator>();

        CurWeaponnum = 0;
        WeaponInRightHand.transform.GetChild(CurWeaponnum).gameObject.SetActive(true);

        CurWeaponData = InvWeapon[CurWeaponnum];
        cur_AttackPower = CurWeaponData.WeaponPower;
        CurWeaponObject = WeaponInRightHand.transform.GetChild(CurWeaponnum).gameObject;
        Skill1Stamina = CurWeaponData.skill1Stamina;
        Skill2Stamina = CurWeaponData.skill2Stamina;
        Skill1MaxCooltime = CurWeaponData.skill1.Cooltime;
        Skill2MaxCooltime = CurWeaponData.skill2.Cooltime;
        PlayerSestting(CurWeaponnum);
        PlayerStatus.Instance.CurWeaponnum = CurWeaponnum;
    }
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            Time.timeScale = 3f;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Time.timeScale = 10f;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0f;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameManager.Instance.GameClear();
        }
#endif

        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking)
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !isAttacking)
        {
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Q) && CurSkill[0] > 0 && !isAttacking && Skill1Cooltime <= 0f && PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] >= Skill1Stamina)
        {
            isAttacking = true;
            animator.SetTrigger(CurWeaponData.skill1.animationname);
            cur_AttackPower *= CurWeaponData.skill1.SkillAttackPower;
            curattacknum = (int)SkillType.Skill1;
        }
        else if (Input.GetKeyDown(KeyCode.E) && CurSkill[1] > 0 && !isAttacking && Skill2Cooltime <= 0f && PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] >= Skill2Stamina)
        {
            isAttacking = true;
            animator.SetTrigger(CurWeaponData.skill2.animationname);
            cur_AttackPower *= CurWeaponData.skill2.SkillAttackPower;
            curattacknum = (int)SkillType.Skill2;
        }
    }
    void FixedUpdate()
    {
        if (!isAttacking && curattacknum == -1 && CurWeaponObject.activeSelf)
        {
            curattacknum = 0;
            animator.SetTrigger(CurWeaponData.itemName);
            CurWeaponObject.GetComponent<WeaponAttack>().OnAttack((int)SkillType.Normal);
        }
        if (InvWeapon.Count > 2)
        {
            SubWeapon_Attacktimer1 -= Time.fixedDeltaTime;
            if (SubWeapon_Attacktimer1 <= 0)
            {
                SubWeapon_Attacktimer1 = SubWeapon1.SubWeaponAttack(2);
            }
        }
        if (InvWeapon.Count > 3)
        {
            SubWeapon_Attacktimer2 -= Time.fixedDeltaTime;
            if (SubWeapon_Attacktimer2 <= 0)
            {
                SubWeapon_Attacktimer2 = SubWeapon2.SubWeaponAttack(3);
            }
        }



    }
    public void AttackTrue()
    {
        float consumemp = curattacknum == 2 ? Skill2Stamina : Skill1Stamina;
        float cool = curattacknum == 2 ? Skill2MaxCooltime : Skill1MaxCooltime;
        PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] -= consumemp;
        PlayerStatus.Instance.SkillCooltime[curattacknum - 1] = cool;
        CurWeaponObject?.GetComponent<WeaponAttack>().OnAttack(curattacknum);
        isAttacking = true;
    }
    public void AttackFalse()
    {
        isAttacking = false;
        CurWeaponObject?.GetComponent<WeaponAttack>().AttackFalse(curattacknum);
        cur_AttackPower = CurWeaponData?.WeaponPower ?? 1f;
        curattacknum = -1;
    }

    public void Fire(string data)
    {
        string[] parameters = data.Split(',');
        int weapon = int.Parse(parameters[0]);
        int Piercing = int.Parse(parameters[1]);
        if (CurWeaponObject.GetComponent<GunAttack>() == null) return;
        CurWeaponObject.GetComponent<GunAttack>().Fire(weapon, Piercing);
    }
    void SwitchWeapon(int weaponIndex)
    {
        // 무기 인덱스가 유효한지 확인
        if (CurWeaponnum != weaponIndex && weaponIndex < InvWeapon.Count)
        {
            WeaponInRightHand.transform.GetChild(CurWeaponnum).gameObject.SetActive(false);

            CurWeaponData = InvWeapon[weaponIndex];
            cur_AttackPower = CurWeaponData.WeaponPower;
            CurWeaponObject = WeaponInRightHand.transform.GetChild(weaponIndex).gameObject;
            Skill1Stamina = CurWeaponData.skill1Stamina;
            Skill2Stamina = CurWeaponData.skill2Stamina;
            Skill1MaxCooltime = CurWeaponData.skill1.Cooltime;
            Skill2MaxCooltime = CurWeaponData.skill2.Cooltime;
            PlayerStatus.Instance.SkillCooltime[0] = Skill1MaxCooltime / 4f;
            PlayerStatus.Instance.SkillCooltime[1] = Skill2MaxCooltime / 4f;
            PlayerSestting(weaponIndex);

            CurWeaponnum = weaponIndex;
            PlayerStatus.Instance.CurWeaponnum = CurWeaponnum;
            WeaponInRightHand.transform.GetChild(weaponIndex).gameObject.SetActive(true);

            WeaponSwapBuff();
            CallFlipBurst();

        }
        else
        {
            Debug.LogWarning($"무기 인덱스 {weaponIndex}가 유효하지 않습니다.");
        }
    }
    void PlayerSestting(int weaponIndex)
    {
        PlayerStatus.Instance.WeaponsLv[CurWeaponnum] = PlayerStatus.Instance.StatusLv[0];
        PlayerStatus.Instance.StatusLv[0] = PlayerStatus.Instance.WeaponsLv[weaponIndex];
        PlayerStatus.Instance.WeaponsSkillLv[CurWeaponnum] = $"{PlayerStatus.Instance.CurSkill[0]}_{PlayerStatus.Instance.CurSkill[1]}";
        Debug.Log($"무기가 변경되었습니다: {CurWeaponData.itemName}");
    }
    void WeaponSwapBuff()
    {
        // StartCoroutine(PlayerStatus.Instance.BuffSet(1, 1.5f, 3f));
    }
    void CallFlipBurst()
    {
        if (PlayerStatus.Instance.FlipBurstCooltime[CurWeaponnum] <= 0f)
        {
            isAttacking = true;
            animator.SetTrigger(CurWeaponData.FlipBurst?.animationname);
            if (CurWeaponObject.GetComponent<WeaponAttack>().OnAttack((int)SkillType.FlipBurst + CurWeaponnum))
                PlayerStatus.Instance.FlipBurstCooltime[CurWeaponnum] = PlayerStatus.Instance.FlipBurstMaxCool;
#if UNITY_EDITOR
            PlayerStatus.Instance.FlipBurstCooltime[CurWeaponnum] = 2f;
#endif

        }
    }
}
