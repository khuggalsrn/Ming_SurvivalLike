using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

enum Arms { Pistol, Sniper, ShotGun }
public class GunAttack : WeaponAttack
{
    // public ItemData Weapon;
    // public SkillData skill1;
    // public SkillData skill2;
    // public ParticleSystem Basic;
    // public GameObject BasicEffectObject;
    // public SkillData FlipBurst;
    // public float cur_AttackPower = 0; // 스킬에 의한 배수
    // public float weaponbasicpower = 1; // 무기 상수
    // public float TurnStamina;
    // public DamageType type;
    [SerializeField] ParticleSystem MobHit;
    [SerializeField] GameObject ShotGun;
    public override bool IsSubWeapon
    {
        get { return isSubWeapon; }
        set { isSubWeapon = value; }
    }
    private bool isSubWeapon = false;
    GameObject Pistol;
    GameObject Sniper;
    Coroutine basiccor;
    float AddPower = 0;
    float timer3 = 0f;
    bool TimeBullet = false;
    public GameObject Mines;
    [SerializeField] float MineTimer = 0f;
    float BonusDamage = 1f;
    float BossDrainStamina = 0;
    bool StopBullet = false;
    bool TankMine = false;
    float CriticalRate = 0;
    int Piercing = 1;
    float Stimpackduration = 0f;
    Animator animator => PlayerStatus.Instance.GetComponent<Animator>();
    float MineDamage => (weaponbasicpower + AddPower + PlayerStatus.Instance.Value_AdditionalWeaponPower)
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
    public override float FinalDamage =>
            (weaponbasicpower + AddPower + PlayerStatus.Instance.Value_AdditionalWeaponPower)
            * cur_AttackPower
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
    private Coroutine StimpackCoroutine;
    float initialDamageIncrease = 0.25f;
    float initialSpeedIncrease = 5f;
    float speedIncrease = 0f;  // Speed 증가량
    float prevspeedIncrease = 0f;  // 전Speed 증가량
    float animSpeed = 1.2f;
    float AddianimSpeed = 0f;
    bool isLevel6 = false;

    float ReduceSubWeaponCooltime = 0f;
    int AddiObject = 0;
    int AddiTarget = 0;
    //
    //
    //
    //
    //
    //
    protected override void Start()
    {
        base.Start();
        ShotGun = Instantiate(ShotGun, PlayerStatus.Instance.WeaponInLeftHand.transform);
        switch (PlayerStatus.Instance.AvatarNum)
        {
            case 0:
                break;
            case 1:
                ShotGun.transform.SetLocalPositionAndRotation(new Vector3(-0.1f, 0.046f, 0.018f), Quaternion.Euler(-27.919f, 13.869f, 193.252f));
                break;
            case 2:
                ShotGun.transform.SetLocalPositionAndRotation(new Vector3(-0.1f, 0.046f, 0.018f), Quaternion.Euler(-27.919f, 13.869f, 193.252f));
                break;
        }
        // ShotGun.transform.SetLocalPositionAndRotation(new Vector3(-0.037f, -0.059f, 0.019f), Quaternion.Euler(-1.884f, 13.928f, 146.233f));
        ShotGun.name = ShotGun.name.Substring(0, 14);
        Pistol = transform.Find("Weapon_Pistol").gameObject;
        Sniper = transform.Find("Weapon_Sniper").gameObject;
        Reset();
        Type = DamageType.Piercing;
    }
    private void OnEnable()
    {
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(true);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        ShotGun.gameObject.SetActive(false);
        PlayerStatus.Instance.GetComponent<PlayerController>().Dash();
    }

    private IEnumerator Stimpack()
    {
        Debug.LogWarning("스팀팩");
        Stimpackduration = 3f; // 3초 동안 변화
        float elapsedTime = 0f;

        speedIncrease = 0f;  // Speed 증가량
        prevspeedIncrease = 0f;  // 전Damage 증가량

        animator.SetFloat("FirearmAniSpeed", (animSpeed + AddianimSpeed) * 1.5f);

        while (elapsedTime < Stimpackduration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / Stimpackduration; // 0에서 1까지의 비율 계산

            // 증가량 선형 감소
            speedIncrease = Mathf.Lerp(initialSpeedIncrease, 0f, t);

            // 실제 값은 증가량을 더해서 계산 (외부 변경값 고려)
            ApplyStatChanges(speedIncrease, ref prevspeedIncrease);
            yield return null; // 다음 프레임까지 대기

        }
        while (isLevel6 && PlayerStatus.Instance.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            yield return null;
        }

        // 최종 적용
        speedIncrease = 0f;
        animator.SetFloat("FirearmAniSpeed", animSpeed + AddianimSpeed);
        ApplyStatChanges(speedIncrease, ref prevspeedIncrease);
        StimpackCoroutine = null;

    }

    private void ApplyStatChanges(float speedIncrease, ref float prevspeedIncrease)
    {
        // 현재 증가량을 더한 값을 적용
        // 외부에서 Damage와 Speed를 변경해도 해당 변경 사항이 반영됩니다.
        float baseSpeed = PlayerStatus.Instance.AddiStats[(int)Stats.Speed] - prevspeedIncrease;   // 증가량 제거
        PlayerStatus.Instance.AddiStats[(int)Stats.Speed] = baseSpeed + speedIncrease;
        prevspeedIncrease = speedIncrease;
    }

    private void OnDisable()
    {
        if (basiccor != null) StopCoroutine(basiccor);
        if (StimpackCoroutine != null)
        {
            speedIncrease = 0f;
            animator.SetFloat("FirearmAniSpeed", animSpeed + AddianimSpeed);
            ApplyStatChanges(speedIncrease, ref prevspeedIncrease);
            StimpackCoroutine = null;
        }
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(false);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        ShotGun.gameObject.SetActive(false);
        PlayerStatus.Instance.GetComponent<PlayerAttack>().AttackFalse();
    }
    void FixedUpdate()
    {
        // MineTimer += 0.02f;
        // if (!StopBullet) timer3 += 0.02f;
        // if (timer3 > 10f)
        // {
        //     StopBullet = true;
        //     timer3 = 0f;
        // }
        // if (MineTimer > 30f && Mines)
        // {
        //     MineTimer = 0f;
        //     GameObject temp = Instantiate(Mines, PlayerStatus.Instance.transform);
        //     temp.transform.SetParent(null, true);
        //     if (TankMine)
        //     {
        //         temp.GetComponent<GunSkillObject>().FinalDamage = 15 * this.MineDamage;
        //     }
        //     else
        //     {
        //         temp.GetComponent<GunSkillObject>().FinalDamage = 6 * this.MineDamage;
        //     }
        // }
    }
    public override bool OnAttack(int atk)
    {
        base.OnAttack(atk);
        if (basiccor != null) StopCoroutine(basiccor);
        switch (atk)
        {
            case 0:
                cur_AttackPower = weaponbasicpower;
                basiccor = StartCoroutine(BasicEffect());
                return true;
            case 1:
                cur_AttackPower = Weapon.skill1.SkillAttackPower;
                StartCoroutine(Skill1Effect());
                return true;
            case 2:
                cur_AttackPower = Weapon.skill2.SkillAttackPower;
                StartCoroutine(Skill2Effect());
                return true;
            case 3:
                cur_AttackPower = Weapon.FlipBurst?.SkillAttackPower ?? 1f;
                StartCoroutine(FlipBurstEffect());
                return true;
            case 4:
                cur_AttackPower = Weapon.FlipBurst?.SkillAttackPower ?? 1f;
                StartCoroutine(FlipBurstEffect());
                return true;
            default:
                return false;
        }

    }
    public override void AttackFalse(int atk)
    {
        // Debug.Log("스워드어택 false 하는거 없음 이제");
        // transform.GetChild(atk).GetComponent<BoxCollider>().gameObject.SetActive(false);

    }
    IEnumerator DestroyParticleSystem(ParticleSystem part, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (part != null)
        {
            part.Stop();
            Destroy(part.gameObject);
        }
    }
    void ShotEffect(Vector3 Pos, bool Long)
    {
        ParticleSystem part = Instantiate(Weapon.Basic);
        part.transform.rotation = PlayerStatus.Instance.transform.rotation;
        part.transform.Rotate(90, 0, 0);
        part.transform.localScale *= PlayerStatus.Instance.Value_AttackRange / PlayerStatus.Instance.transform.localScale.x;
        part.transform.position = Pos;
        if (Long) part.transform.position += 2 * PlayerStatus.Instance.transform.forward;
        DestroyParticleSystem(part, 2f);
        GameObject temp = Instantiate(Weapon.BasicEffectObject);
        temp.transform.rotation = PlayerStatus.Instance.transform.rotation;
        temp.transform.Rotate(0, 90, 0);
        temp.transform.localScale *= PlayerStatus.Instance.Value_AttackRange / PlayerStatus.Instance.transform.localScale.x;
        temp.transform.position = Pos;
        if (Long)
        {
            temp.transform.position += 5 * PlayerStatus.Instance.transform.forward;
            temp.transform.localScale = new Vector3(temp.transform.localScale.x * 50, temp.transform.localScale.y * 1, temp.transform.localScale.z * 1);

        }
        Destroy(temp, 0.1f);
    }
    IEnumerator BasicEffect()
    {
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        ShotGun.SetActive(false);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.225f / 1.2f * animator.GetFloat("FirearmAniSpeed"));
        // Fire(Pistol.transform.position);
        yield return new WaitForSeconds(0.65f / 1.2f * animator.GetFloat("FirearmAniSpeed"));
        basiccor = StartCoroutine(BasicEffect2());
    }
    IEnumerator BasicEffect2()
    {
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        cur_AttackPower = 2 * weaponbasicpower;
        ShotGun.SetActive(true);
        yield return new WaitForSeconds(0.15f / 1.2f * animator.GetFloat("FirearmAniSpeed"));
        // Fire(ShotGun.transform.position);
        yield return new WaitForSeconds(0.75f / 1.2f * animator.GetFloat("FirearmAniSpeed"));
        ShotGun.SetActive(false);
        cur_AttackPower = weaponbasicpower;
        basiccor = StartCoroutine(BasicEffect3());
    }
    IEnumerator BasicEffect3()
    {
        cur_AttackPower = 3 * weaponbasicpower;
        yield return new WaitForSeconds(0.6f / 1.2f * animator.GetFloat("FirearmAniSpeed"));
        ShotGun.SetActive(false);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(false);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.4f / 1.2f * animator.GetFloat("FirearmAniSpeed"));
        // Fire(Sniper.transform.position, true);
        yield return new WaitForSeconds(0.6f / 1.2f * animator.GetFloat("FirearmAniSpeed"));
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(true);
        cur_AttackPower = weaponbasicpower;
    }
    IEnumerator Skill1Effect()
    {
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(true);
        ShotGun.SetActive(true);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        cur_AttackPower = 2 * Weapon.skill1.SkillAttackPower;
        // Fire(ShotGun.transform.position);
        yield return new WaitForSeconds(5f / 1.2f * animator.GetFloat("FirearmAniSpeed"));
        // cur_AttackPower = weaponbasicpower;
        TurnStaminaAfterSkill();

        if (StimpackCoroutine != null)
        {
            speedIncrease = 0f;
            animator.SetFloat("FirearmAniSpeed", animSpeed + AddianimSpeed);
            ApplyStatChanges(speedIncrease, ref prevspeedIncrease);
            StimpackCoroutine = null;
        }
        StimpackCoroutine = StartCoroutine(Stimpack());
    }
    IEnumerator Skill2Effect()
    {
        cur_AttackPower = 3 * Weapon.skill2.SkillAttackPower;
        ShotGun.SetActive(false);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(false);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(true);
        // yield return new WaitForSeconds(5.9f / 1.2f * animator.GetFloat("FirearmAniSpeed"));
        // transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        // transform.GetChild((int)Arms.Pistol).gameObject.SetActive(true);
        // cur_AttackPower = weaponbasicpower;
        yield return null;
        TurnStaminaAfterSkill();
        if (StimpackCoroutine != null)
        {
            speedIncrease = 0f;
            animator.SetFloat("FirearmAniSpeed", animSpeed + AddianimSpeed);
            ApplyStatChanges(speedIncrease, ref prevspeedIncrease);
            StimpackCoroutine = null;
        }
        StimpackCoroutine = StartCoroutine(Stimpack());
    }
    IEnumerator FlipBurstEffect()
    {
        if (StimpackCoroutine != null)
        {
            speedIncrease = 0f;
            animator.SetFloat("FirearmAniSpeed", animSpeed + AddianimSpeed);
            ApplyStatChanges(speedIncrease, ref prevspeedIncrease);
            StimpackCoroutine = null;
        }
        StimpackCoroutine = StartCoroutine(Stimpack());
        yield return new WaitForSeconds(0);
        // (string tag, Vector3 position, float range)
        float range = 50f;
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Monster");
        GameObject highestHPMonster = null;
        float highestHP = float.MinValue;

        foreach (GameObject obj in taggedObjects)
        {
            // 거리 계산
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance > range) continue;

            // Status 컴포넌트 가져오기
            MonsterStatus status = obj.GetComponent<MonsterStatus>();
            if (status != null && status.HitPoint > highestHP)
            {
                highestHPMonster = obj;
                highestHP = status.HitPoint;
            }
        }
        SkillManager.Instance.LockOn(highestHPMonster ?? null);
    }
    public void Fire(int weapon, int piercing = 0)
    {
        piercing += Piercing;
        Vector3 pos;
        switch (weapon)
        {
            case 0:
                pos = Pistol.transform.position;
                break;
            case 1:
                pos = Sniper.transform.position;
                break;
            case 2:
                pos = ShotGun.transform.position;
                break;
            default:
                pos = Pistol.transform.position;
                break;
        }

        // 총알이 발사되는 위치와 방향
        Vector3 firePosition = pos;
        // 마우스 위치로부터 Ray 생성

        Vector3 lookDirection = PlayerStatus.Instance.transform.forward;
        lookDirection.y = 0f; // 높이 차이를 무시

        Vector3 fireDirection = lookDirection;
        if (weapon == 1)
            ShotEffect(firePosition, true);
        else
            ShotEffect(firePosition, false);

        // Debug.DrawRay(firePosition, fireDirection * 10f, Color.red, 1f);

        // RaycastAll로 충돌한 모든 객체 확인
        RaycastHit[] hits = Physics.SphereCastAll(firePosition, 0.5f, fireDirection, 15f * PlayerStatus.Instance.Value_AttackRange, LayerMask.GetMask("Monster"));

        if (hits.Length > 0)
        {
            // 충돌한 객체를 거리순으로 정렬
            System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

            // 가까운 객체 가져오기
            for (int i = 0; i < piercing; i++)
            {
                int t = i;
                if (hits.Length == t) break;
                RaycastHit closestHit = hits[t];

                // 충돌한 Monster 처리
                GameObject hitMonster = closestHit.collider.gameObject;
                ParticleSystem temp2 = Instantiate(MobHit, hitMonster.transform);
                Destroy(temp2.gameObject, 5f);
                if (StopBullet)
                {
                    hitMonster.GetComponent<NavMeshAgent>().speed = 0;
                }
                if (hitMonster.name.Substring(0, 4).Equals("BOSS"))
                {
                    PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] += BossDrainStamina;
                }
                else
                {
                    if (TimeBullet)
                    {
                        hitMonster.GetComponent<MonsterController>().MoveSpeed = 0;
                        hitMonster.GetComponent<NavMeshAgent>().speed = 0;
                    }
                }
                ApplyDamage(hitMonster); // 데미지 처리 함수 호출
            }


        }
    }
    public override float SubWeaponAttack(int num)
    {
        StartCoroutine(Sub_FireShot(num));
        return Weapon.SubWeaponCooltime - ReduceSubWeaponCooltime;
    }
    IEnumerator Sub_FireShot(int num)
    {
        for (int i = 0; i < 1 + AddiObject; i++)
        {
            float dmg = (weaponbasicpower + AddPower + PlayerStatus.Instance.WeaponsLv[num] * PlayerStatus.Instance.StatPerLv[0])
            * cur_AttackPower
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
            GameObject temp = Instantiate(Weapon.SubWeapon, PlayerStatus.Instance.transform);
            temp.transform.SetParent(null, true);
            temp.transform.localScale *= PlayerStatus.Instance.Value_AttackRange;
            temp.GetComponent<GunSkillObject>().FinalDamage = dmg;
            temp.GetComponent<GunSkillObject>().RemainTargetNum += AddiTarget;
            yield return new WaitForSeconds(0.1f);
        }
    }
    public override void Reset()
    {
        AddPower = 0;
        StopBullet = false;
        // TankMine = false;
        // MineTimer = -180000f;
        BonusDamage = 1f;
        BossDrainStamina = 0;
        TurnStamina = 0;
        Piercing = 1;
        CriticalRate = 0;
        PlayerStatus.Instance.AddiStats[(int)Stats.RangeMultiplier] = 0f;
        animSpeed = 1.2f;
        AddianimSpeed = 0f;
        animator.SetFloat("FirearmAniSpeed", animSpeed + AddianimSpeed);
        isLevel6 = false;

        ReduceSubWeaponCooltime = 0f;
        AddiObject = 0;
        AddiTarget = 0;
    }
    public override void LevelSet(int i)
    {
        var p = PlayerStatus.Instance;
        if (isSubWeapon)
        {
            switch (i)
            {
                case 4:
                    AddiTarget += 1;
                    AddiObject += 1;
                    goto case 3;
                case 3:
                    AddPower += 2f;
                    ReduceSubWeaponCooltime += 0.2f;
                    goto case 2;
                case 2:
                    AddiTarget += 1;
                    AddPower += 2f;
                    goto case 1;
                case 1:
                    AddiObject += 1;
                    AddPower += 2f;
                    break;
                default:
                    break;
            }
        }
        else
            switch (i)
            {
                case 9:
                    p.AddiStats[(int)Stats.RangeMultiplier] += 0.2f;
                    goto case 8;
                case 8:
                    //록온 대상 주변 적들이 록온 추가피해량 적용을 받습니다.
                    goto case 7;
                case 7:
                    CriticalRate = 0.2f;
                    goto case 6;
                case 6:
                    //캐릭터 주위 떠다니는 총 2개
                    goto case 5;
                case 5:
                    Stimpackduration += 2f;
                    isLevel6 = true;
                    goto case 4;
                case 4:
                    Piercing += 1;
                    AddPower += 4;
                    goto case 3;
                case 3:
                    AddianimSpeed = 0.5f;
                    goto case 2;
                case 2:
                    if (p.WeaponsSkillLv[p.CurWeaponnum].Split("_").Select(int.Parse).ToArray().Sum() < 2)
                    {
                        p.WeaponsSkillLv[p.CurWeaponnum] = "1_1";
                    }
                    AddPower += 4;
                    break;
                case 1:
                    Piercing += 1;
                    if (p.WeaponsSkillLv[p.CurWeaponnum].Split("_").Select(int.Parse).ToArray().Sum() < 1)
                    {
                        p.WeaponsSkillLv[p.CurWeaponnum] = Random.Range(0, 2) == 0 ? "0_1" : "1_0";
                    }
                    break;
                default:
                    break;
            }
        animator.SetFloat("FirearmAniSpeed", animSpeed + AddianimSpeed);
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            ApplyDamage(other.gameObject);
            other.gameObject.transform.position -= 5 * other.gameObject.transform.forward;
        }
    }
    protected override void ApplyDamage(GameObject target)
    {
        target.gameObject.GetComponent<MonsterStatus>().OnDamaged(FinalDamage, Type);
    }
    public override void UltimateMode()
    {
        BonusDamage = 1.5f;
        animator.SetFloat("FirearmAniSpeed", 1.8f);
    }
    void TurnStaminaAfterSkill()
    {
        PlayerStatus.Instance.Staminas[PlayerStatus.Instance.CurWeaponnum] += TurnStamina;
    }
}
