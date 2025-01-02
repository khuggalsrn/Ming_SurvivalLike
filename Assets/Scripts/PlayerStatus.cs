using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    // Serialized fields
    [SerializeField] private UserInitStatus initStatus; // 지정된 초기 스탯
    [SerializeField] private GameObject weaponInRightHand;
    [SerializeField] private GameObject weaponInLeftHand;
    [SerializeField] private int curWeaponnum = 0;
    [SerializeField] private int[] curskill => weaponsSkillLv[curWeaponnum].Split('_').Select(int.Parse).ToArray();
    [SerializeField] private List<int> weaponsLv = new List<int>(new int[4]);
    [SerializeField] private string[] weaponsSkillLv = new string[] { "0_0", "0_0", "0_0", "0_0" };
    [SerializeField] private List<string> weaponsDescription = new List<string>();
    [SerializeField] private List<WeaponData> invWeapon;

    // Private fields
    [SerializeField] private int avatarNum;
    private SphereCollider gainItemRange;
    private List<float> flipBurstCooltime = new List<float>(4) { 20f, 20f };
    private float flipBurstMaxCool = 20f;
    private List<float> skillCooltime = new List<float>(4) { 20f, 20f };
    // Additional Stats from Items
    [SerializeField] private List<float> addiStats = new List<float>(new float[Stats.GetValues(typeof(Stats)).Length]);
    [SerializeField] private List<int> statusLv;
    private List<float> statPerLv => GameManager.Instance.UserStat.BasicStatus.floatList; // = new List<float>(9) { 1f, 0.1f, 0.1f, 25f, 50f, 1f, 0.5f, 0.1f, 1f };
    private float basicGainRange;
    private float systemTime = 0f;

    private float Exp;
    private float MaxHp => 100 + Value_AddiHp;
    private float hitPoint;
    private float Barrior;
    private int PlayerLevel = 1;
    private bool BuffOn = false;

    public Coroutine BuffCor = null;
    private List<float> staminas = new List<float>(new float[2]);

    // UI Components
    private Scrollbar HPBar, BarriorBar, ExpBar;
    private List<Scrollbar> StaminaBars = new List<Scrollbar>();
    private List<RawImage> UI_Weapon = new List<RawImage>();
    private List<Text> FlipBurstCoolText = new List<Text>();
    private Text Text_Level, Text_Time;
    private List<RawImage> UI_Skill = new List<RawImage>();
    private List<Text> UI_SkillCoolText = new List<Text>();

    // Singleton implementation
    private static PlayerStatus instance;
    public static PlayerStatus Instance => instance ??= FindObjectOfType<PlayerStatus>();

    // Getter and Setter Properties
    public int AvatarNum
    {
        get { return avatarNum; }
        set { avatarNum = value; }
    }
    public float HitPoint
    {
        get { return hitPoint; }
        set { hitPoint = value; }
    }
    public float FlipBurstMaxCool
    {
        get { return flipBurstMaxCool; }
        set { flipBurstMaxCool = value; }
    }
    public GameObject WeaponInRightHand
    {
        get => weaponInRightHand;
        set => weaponInRightHand = value;
    }

    public GameObject WeaponInLeftHand
    {
        get => weaponInLeftHand;
        set => weaponInLeftHand = value;
    }

    public int CurWeaponnum
    {
        get => curWeaponnum;
        set => curWeaponnum = value;
    }

    public int[] CurSkill
    {
        get => curskill;
    }

    public List<int> WeaponsLv
    {
        get => weaponsLv;
    }
    public string[] WeaponsSkillLv
    {
        get => weaponsSkillLv;
    }
    public List<string> WeaponsDescription
    {
        get => weaponsDescription;
    }
    public List<WeaponData> InvWeapon
    {
        get => invWeapon;
    }
    public List<float> FlipBurstCooltime
    {
        get => flipBurstCooltime;
    }
    public List<float> SkillCooltime
    {
        get => skillCooltime;
    }
    public List<int> StatusLv
    {
        get => statusLv;
    }
    public List<float> StatPerLv
    {
        get => statPerLv;
    }
    public List<float> Staminas
    {
        get => staminas;
    }
    public List<float> AddiStats
    {
        get => addiStats;
    }
    // Calculated Properties
    public float Value_AdditionalWeaponPower => statusLv[0] * statPerLv[0];
    public float Value_LastDamage => (1 + statusLv[1] * statPerLv[1]) * (1 + addiStats[1]);
    public float Value_Speed => 5 + 5 * statusLv[2] * statPerLv[2] + addiStats[2];
    public float Value_Defense => statusLv[3] * statPerLv[3] + addiStats[3];
    public float Value_AddiHp => statusLv[4] * statPerLv[4] + addiStats[4];
    public float Value_HpRecoverySpeed => 1 + statusLv[5] * statPerLv[5] + addiStats[5];
    public float Value_GainItemRange => 1 + statusLv[6] * statPerLv[6] + addiStats[6];
    public float Value_AttackRange => 1 + statusLv[7] * statPerLv[7] + addiStats[7];
    public float Value_StaminaRecoverySpeed => 10f + statusLv[8] * statPerLv[8] + addiStats[8];
    private float CurNeedExp => 40 + Mathf.Pow(PlayerLevel, 1.2f * Mathf.Log(PlayerLevel, 7));

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        initStatus = GameManager.Instance.UserStat;
        InitializeComponents();
        LoadWeaponDescriptions();
        InitializeStats();
        GetComponent<PlayerAttack>().Init();
    }

    private void FixedUpdate()
    {
        UpdateStats();
        UpdateUI();
        
        if(curskill[0] != 0) UI_Skill[0].texture = invWeapon[curWeaponnum].skill1.SkillImageTexture;
        if(curskill[1] != 0) UI_Skill[1].texture = invWeapon[curWeaponnum].skill2.SkillImageTexture;
        
    }

    private void InitializeComponents()
    {
        weaponInRightHand = GameObject.FindWithTag("RightWeapon");
        weaponInLeftHand = GameObject.FindWithTag("LeftWeapon");
        gainItemRange = transform.GetChild(0).GetComponent<SphereCollider>();
        basicGainRange = gainItemRange.radius;

        HPBar = GameObject.Find("HPScrollbar").GetComponent<Scrollbar>();
        BarriorBar = GameObject.Find("BarriorScrollbar").GetComponent<Scrollbar>();
        ExpBar = GameObject.Find("ExpBar").GetComponent<Scrollbar>();

        for (int i = 0; i < 2; i++)
        {
            StaminaBars.Add(GameObject.Find($"StaScrollbar{i}").GetComponent<Scrollbar>());
        }
        for (int i = 0; i < 2; i++)
        {
            GameObject SkillUI = GameObject.Find($"Skill{i+1}");
            UI_Skill.Add(SkillUI.GetComponent<RawImage>());
            UI_SkillCoolText.Add(SkillUI.transform.GetChild(0).GetComponent<Text>());
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject weaponUI = GameObject.Find($"WeaponImage{i}");
            UI_Weapon.Add(weaponUI.GetComponent<RawImage>());
            if (i < 2) FlipBurstCoolText.Add(weaponUI.transform.GetChild(0).GetComponent<Text>());
        }

        Text_Level = GameObject.Find("Text_Level").GetComponent<Text>();
        Text_Time = GameObject.Find("Text_Time_BackGround").transform.GetChild(0).GetComponent<Text>();
    }

    private void LoadWeaponDescriptions()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("WeaponsDescription");
        if (textAsset != null)
        {
            string[] lines = textAsset.text.Split('\n');
            weaponsDescription.AddRange(lines);
        }
    }

    private void InitializeStats()
    {
        statusLv = new List<int>(GameManager.Instance.UserStat.BasicStatus.intList);
        HitPoint = MaxHp / 2; // 기본값
        if (initStatus != null)
        {
            // General Settings
            AvatarNum = initStatus.AvatarNum;

            // Weapon Settings
            invWeapon = new List<WeaponData>();
            WeaponAdd(initStatus.InitWeapon);
            flipBurstCooltime[0] = initStatus.flipBurstCooltimeOfInitWeapon;

            // Stats Settings
            addiStats = new List<float>(initStatus.InitAddiStatusList);
        }
        else
        {
            Debug.LogWarning("UserInitStatus is not assigned. Using default values.");
        }

#if UNITY_EDITOR
        // statusLv[8] = 40;
        // flipBurstCooltime = new List<float> { 0, 0, 0, 0 };
        // statusLv[0] = 8;
        // Exp = 10000;
#endif
    }
    private void UpdateStats()
    {
        if (addiStats[(int)Stats.CurHpRecover] != 0)
        {
            HitPoint += MaxHp * addiStats[(int)Stats.CurHpRecover];
            addiStats[(int)Stats.CurHpRecover] = 0;
        }
        for (int i = 0; i < 2; i++)
        {
            skillCooltime[i] -= Time.fixedDeltaTime;
            flipBurstCooltime[i] -= Time.fixedDeltaTime;
            staminas[i] += staminas[i] < 100 ? Time.fixedDeltaTime * Mathf.Max(Value_StaminaRecoverySpeed, 0) : 0;
        }

        HitPoint = Mathf.Min(HitPoint + Mathf.Max(Value_HpRecoverySpeed, 0) * Time.fixedDeltaTime, MaxHp);

        UpBarrior(200 * addiStats[(int)Stats.CurBarriorSet]);

        gainItemRange.radius = basicGainRange * Value_GainItemRange;

        flipBurstMaxCool = 20 + addiStats[(int)Stats.FlipBurstCoolReduceValue];

        systemTime += Time.fixedDeltaTime;
    }

    private void UpdateUI()
    {
        HPBar.size = HitPoint / MaxHp;
        BarriorBar.size = Barrior / MaxHp;
        ExpBar.size = Exp / CurNeedExp;

        for (int i = 0; i < StaminaBars.Count; i++)
        {
            UI_SkillCoolText[i].text = Mathf.Max(0, Mathf.FloorToInt(skillCooltime[i])).ToString();
            StaminaBars[i].size = staminas[i] / 100f;
            FlipBurstCoolText[i].text = Mathf.Max(0, Mathf.FloorToInt(flipBurstCooltime[i])).ToString();
        }

        for (int i = 0; i < invWeapon.Count; i++)
        {
            UI_Weapon[i].texture = invWeapon[i].WeaponImage;
            UI_Weapon[i].gameObject.transform.localScale = Vector3.one * 0.5f;
        }

        UI_Weapon[curWeaponnum].gameObject.transform.localScale = Vector3.one * 1f;

        Text_Level.text = $"Lv : {PlayerLevel}";
        TimeSpan timeSpan = TimeSpan.FromSeconds(systemTime);
        Text_Time.text = $"{timeSpan.Minutes:D2} : {timeSpan.Seconds:D2}";
    }

    public void GetExpPoint(float exp)
    {
        Exp += exp * (1 + addiStats[(int)Stats.ExpGainRate]);
        if (Exp >= CurNeedExp && PlayerLevel < 50)
        {
            Exp -= CurNeedExp;
            LevelUp();
        }
    }
    public void GetGold(int gold)
    {
        GameManager.Instance.InGameGold += gold;
    }
    private void LevelUp()
    {
        PlayerLevel++;
        GameManager.Instance.LvUp();
    }
    private IEnumerator CollectItem(Transform itemTransform, System.Action onComplete)
    {
        Vector3 startPosition = itemTransform.position;

        float elapsed = 0f;
        while (elapsed < 0.75f)
        {
            if (itemTransform == null) yield break; // Transform이 삭제되었는지 확인
            elapsed += Time.deltaTime;
            float t = elapsed / 0.75f;

            // 아이템을 플레이어로 이동 (부드럽게 날아옴)
            itemTransform.position = Vector3.Lerp(startPosition, transform.position, t);
            yield return null;
        }

        // 이동 완료 후, 습득 처리
        onComplete?.Invoke();
    }
    public void OnDamaged(float dmg)
    {
        if (addiStats[(int)Stats.BarriorGuard] > 0)
        {
            dmg = dmg * 100f / (100f + Value_Defense);
            if (Barrior > 0)
            {
                float remainingDmg = dmg - Barrior;
                Barrior = Mathf.Max(0, Barrior - dmg);
                dmg = remainingDmg > 0 ? remainingDmg : 0;
            }

            HitPoint -= dmg;
        }
        else
        {
            if (Barrior > 0)
            {
                float remainingDmg = dmg - Barrior;
                Barrior = Mathf.Max(0, Barrior - dmg);
                dmg = remainingDmg > 0 ? remainingDmg : 0;
            }

            HitPoint -= dmg * 100f / (100f + Value_Defense);
        }
        if (HitPoint <= 0) Die();
    }

    private void Die()
    {
        GameManager.Instance.GameOver();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exp"))
        {
            float expValue = float.Parse(other.name.Substring(9));
            StartCoroutine(CollectItem(other.transform, () =>
            {
                GetExpPoint(expValue);
                Destroy(other.gameObject);
            }));
        }
        if (other.CompareTag("Gold"))
        {
            int goldValue = int.Parse(other.name.Substring(4));
            StartCoroutine(CollectItem(other.transform, () =>
            {
                GetGold(goldValue);
                Destroy(other.gameObject);
            }));
        }
    }

    public string GetWeaponsDescription(int i)
    {
        return weaponsDescription[i];
    }

    public void UpBarrior(float amount)
    {
        Barrior += MaxHp * amount;
    }
    public IEnumerator BuffSet(int i, float value, float time)
    {
        if (BuffOn)
        {
            addiStats[i] -= value;
        }
        BuffOn = true;
        Debug.Log("Buff");
        if (i == 1)
        {
            addiStats[i] += value;
            yield return new WaitForSeconds(time);
            addiStats[i] -= value;
            BuffOn = false;
        }
        else if (i == 2)
        {
            yield return null;
        }
        yield return null;
    }
    public void WeaponAdd(WeaponData weaponData)
    {
        if (invWeapon.Count == 4) return;
        invWeapon.Add(weaponData);
        GameObject _curAddWeapon = Instantiate(weaponData.Item, weaponInRightHand.transform);
        // if (invWeapon.Count == 3 || invWeapon.Count == 4) 
        //     _curAddWeapon.SetActive(true);
        // else
        _curAddWeapon.SetActive(false);
        weaponInRightHand.transform.GetChild(invWeapon.Count - 1).GetComponent<WeaponAttack>().Reset();
        weaponInRightHand.transform.GetChild(invWeapon.Count - 1).GetComponent<WeaponAttack>().LevelSet(WeaponsLv[invWeapon.Count - 1]);
        if (invWeapon.Count == 3 || invWeapon.Count == 4)
        {
            weaponInRightHand.transform.GetChild(invWeapon.Count - 1).GetComponent<WeaponAttack>().IsSubWeapon = true;
            _curAddWeapon.SetActive(true);
        }
        WeaponTransformSetting(_curAddWeapon);
    }
    void WeaponTransformSetting(GameObject weapon)
    {
        switch (AvatarNum)
        {
            case 0:
                break;
            case 1:
                switch (invWeapon[invWeapon.Count - 1].WeaponLabel)
                {
                    case 0:
                        break;
                    case 1:
                        weapon.transform.SetLocalPositionAndRotation(new Vector3(-0.1f, 0.046f, 0.018f), Quaternion.Euler(-27.919f, 13.869f, 193.252f));
                        break;
                }
                break;
            case 2:
                switch (invWeapon[invWeapon.Count - 1].WeaponLabel)
                {
                    case 0:
                        break;
                    case 1:
                        weapon.transform.SetLocalPositionAndRotation(new Vector3(-0.1f, 0.046f, 0.018f), Quaternion.Euler(-27.919f, 13.869f, 193.252f));
                        break;
                };
                break;
        }
    }
}
