using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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
    // public bool CanFlipBurst;
    [SerializeField] ParticleSystem MobHit;
    [SerializeField] GameObject ShotGun;
    GameObject Pistol;
    GameObject Sniper;
    enum Arms { Pistol, Sniper, ShotGun }
    Coroutine basiccor;
    float AddPower = 0;
    float timer3 = 0f;
    bool TimeBullet = false;
    public GunSkillObject Mines;
    [SerializeField] float MineTimer = 0f;
    [SerializeField] GameObject Mine;
    float BonusDamage = 1f;
    float BossDrainStamina = 0;
    bool StopBullet = false;
    bool TankMine = false;
    Animator animator;
    float MineDamage => (weaponbasicpower + AddPower + PlayerStatus.Instance.Value_AdditionalWeaponPower)
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
    [SerializeField]
    float FinalDamage =>
            (weaponbasicpower + AddPower + PlayerStatus.Instance.Value_AdditionalWeaponPower)
            * cur_AttackPower
            * PlayerStatus.Instance.Value_LastDamage
            * BonusDamage;
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
        ShotGun.name = ShotGun.name.Substring(0, 14);
        Pistol = transform.Find("Weapon_Pistol").gameObject;
        Sniper = transform.Find("Weapon_Sniper").gameObject;
        animator = PlayerStatus.Instance.GetComponent<Animator>();
        Reset();
    }
    private void OnEnable()
    {
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(true);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        ShotGun.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if (basiccor != null) StopCoroutine(basiccor);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(false);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        ShotGun.gameObject.SetActive(false);
    }
    void FixedUpdate()
    {
        MineTimer += 0.02f;
        if (!StopBullet) timer3 += 0.02f;
        if (timer3 > 10f)
        {
            StopBullet = true;
            timer3 = 0f;
        }
        if (MineTimer > 30f && Mine)
        {
            MineTimer = 0f;
            GameObject temp = Instantiate(Mine, PlayerStatus.Instance.transform);
            temp.transform.SetParent(null, true);
            if (TankMine)
            {
                temp.GetComponent<GunSkillObject>().MineDamage = 15 * this.MineDamage;
            }
            else
            {
                temp.GetComponent<GunSkillObject>().MineDamage = 6 * this.MineDamage;
            }
        }
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
                cur_AttackPower = skill1.SkillAttackPower;
                StartCoroutine(Skill1Effect());
                return true;
            case 2:
                cur_AttackPower = skill2.SkillAttackPower;
                StartCoroutine(Skill2Effect());
                return true;
            case 3:
                if (CanFlipBurst)
                {
                    cur_AttackPower = FlipBurst.SkillAttackPower;
                    // StartCoroutine(FlipBurstEffect(FlipBurst.Effect));
                    return true;
                }
                else
                {
                    return false;
                }
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
        ParticleSystem part = Instantiate(Basic);
        part.transform.rotation = PlayerStatus.Instance.transform.rotation;
        part.transform.Rotate(90, 0, 0);
        part.transform.localScale *= PlayerStatus.Instance.Value_AttackRange / PlayerStatus.Instance.transform.localScale.x;
        part.transform.position = Pos;
        if (Long) part.transform.position += PlayerStatus.Instance.transform.forward;
        DestroyParticleSystem(part, 5f);
        GameObject temp = Instantiate(BasicEffectObject);
        temp.transform.rotation = PlayerStatus.Instance.transform.rotation;
        temp.transform.Rotate(0, 90, 0);
        temp.transform.localScale *= PlayerStatus.Instance.Value_AttackRange / PlayerStatus.Instance.transform.localScale.x;
        temp.transform.position = Pos;
        if (Long) temp.transform.position += PlayerStatus.Instance.transform.forward;
        Destroy(temp, 0.1f);
    }
    IEnumerator BasicEffect()
    {
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        ShotGun.SetActive(false);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.225f / 1.2f * animator.GetFloat("AniSpeed"));
        Fire(Pistol.transform.position);
        yield return new WaitForSeconds(0.65f / 1.2f * animator.GetFloat("AniSpeed"));
        basiccor = StartCoroutine(BasicEffect2());
    }
    IEnumerator BasicEffect2()
    {
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        cur_AttackPower = 2 * weaponbasicpower;
        ShotGun.SetActive(true);
        yield return new WaitForSeconds(0.15f / 1.2f * animator.GetFloat("AniSpeed"));
        Fire(ShotGun.transform.position);
        yield return new WaitForSeconds(0.75f / 1.2f * animator.GetFloat("AniSpeed"));
        ShotGun.SetActive(false);
        cur_AttackPower = weaponbasicpower;
        basiccor = StartCoroutine(BasicEffect3());
    }
    IEnumerator BasicEffect3()
    {
        cur_AttackPower = 3 * weaponbasicpower;
        yield return new WaitForSeconds(0.6f / 1.2f * animator.GetFloat("AniSpeed"));
        ShotGun.SetActive(false);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(false);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.4f / 1.2f * animator.GetFloat("AniSpeed"));
        Fire(Sniper.transform.position, true);
        yield return new WaitForSeconds(0.6f / 1.2f * animator.GetFloat("AniSpeed"));
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(true);
        cur_AttackPower = weaponbasicpower;
    }
    IEnumerator Skill1Effect()
    {
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        cur_AttackPower = 2 * skill1.SkillAttackPower;
        ShotGun.SetActive(true);
        yield return new WaitForSeconds(0.25f / 1.2f * animator.GetFloat("AniSpeed"));
        Fire(ShotGun.transform.position);
        yield return new WaitForSeconds(0.55f / 1.2f * animator.GetFloat("AniSpeed"));
        Fire(ShotGun.transform.position);
        yield return new WaitForSeconds(0.50f / 1.2f * animator.GetFloat("AniSpeed"));
        Fire(ShotGun.transform.position);
        yield return new WaitForSeconds(0.5f / 1.2f * animator.GetFloat("AniSpeed"));
        ShotGun.SetActive(false);
        cur_AttackPower = weaponbasicpower;
    }
    IEnumerator Skill2Effect()
    {
        cur_AttackPower = 3 * skill2.SkillAttackPower;
        ShotGun.SetActive(false);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(false);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f / 1.2f * animator.GetFloat("AniSpeed"));
        Fire(Sniper.transform.position, true);
        yield return new WaitForSeconds(0.965f / 1.2f * animator.GetFloat("AniSpeed"));
        Fire(Sniper.transform.position, true);
        yield return new WaitForSeconds(1f);
        transform.GetChild((int)Arms.Sniper).gameObject.SetActive(false);
        transform.GetChild((int)Arms.Pistol).gameObject.SetActive(true);
        cur_AttackPower = weaponbasicpower;

    }
    IEnumerator FlipBurstEffect(ParticleSystem part)
    {
        GameObject temp = Instantiate(FlipBurst.SkillObject);
        temp.transform.localScale = new Vector3(1, 1, 1) * PlayerStatus.Instance.Value_AttackRange;
        temp.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        temp.GetComponent<GunSkillObject>().MineDamage = FinalDamage;

        ParticleSystem temp2 = Instantiate(part);
        temp2.transform.localScale *= PlayerStatus.Instance.Value_AttackRange;
        temp2.transform.SetLocalPositionAndRotation(PlayerStatus.Instance.transform.position, PlayerStatus.Instance.transform.rotation);
        Destroy(temp2.gameObject, 5f);

        yield return new WaitForSeconds(1f);
        cur_AttackPower = 1;
        DestroyImmediate(temp);

    }
    void Fire(Vector3 pos, bool Long = false)
    {
        // 총알이 발사되는 위치와 방향
        Vector3 firePosition = pos;
        // 마우스 위치로부터 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Ray가 특정 레이어(groundLayer)에 닿았는지 확인
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground", "Monster", "Object")))
        {

            // 캐릭터가 마우스 커서를 바라보도록 회전
            Vector3 lookDirection = hit.point - PlayerStatus.Instance.transform.position; // 캐릭터와 마우스 위치 간의 벡터
            lookDirection.y = 0f; // 높이 차이를 무시

            Vector3 fireDirection = lookDirection;
            ShotEffect(firePosition, Long);

            Debug.DrawRay(firePosition, fireDirection * 10f, Color.red, 1f);

            // RaycastAll로 충돌한 모든 객체 확인
            RaycastHit[] hits = Physics.SphereCastAll(firePosition, 0.5f, fireDirection, 15f * PlayerStatus.Instance.Value_AttackRange, LayerMask.GetMask("Monster"));

            if (hits.Length > 0)
            {
                // 충돌한 객체를 거리순으로 정렬
                System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

                // 가장 가까운 객체 가져오기
                RaycastHit closestHit = hits[0];

                // 충돌한 Monster 처리
                GameObject hitMonster = closestHit.collider.gameObject;
                Debug.Log($"Hit Monster: {hitMonster.name}");
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
                hitMonster.GetComponent<MonsterStatus>().OnDamaged(FinalDamage); // 데미지 처리 함수 호출

            }
            else
            {
                Debug.Log("No monster hit.");
            }
        }
    }
    public override void Reset()
    {
        AddPower = 0;
        timer3 = -180000f;
        StopBullet = false;
        CanFlipBurst = false;
        TankMine = false;
        MineTimer = 0f;
        Mine = null;
        BonusDamage = 1f;
        animator.SetFloat("AniSpeed", 1.2f);
        BossDrainStamina = 0;
    }
    public override void LevelSet(int i)
    {
        switch (i)
        {
            case 9:
                BonusDamage = 1.5f;
                animator.SetFloat("AniSpeed", 1.8f);
                goto case 8;
            case 8:
                StopBullet = true;
                goto case 7;
            case 7:
                AddPower += 10;
                goto case 6;
            case 6:
                BossDrainStamina += 5f;
                goto case 5;
            case 5:
                TankMine = true;
                Mine = Mines.gameObject;
                goto case 4;
            case 4:
                Mine = Mines.gameObject;
                goto case 3;
            case 3:
                CanFlipBurst = true;
                goto case 2;
            case 2:
                timer3 = 0f;
                goto case 1;
            case 1:
                AddPower += 2;
                break;
        }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            other.gameObject.GetComponent<MonsterStatus>().OnDamaged(FinalDamage);
            Debug.Log(FinalDamage);
        }
    }
}
