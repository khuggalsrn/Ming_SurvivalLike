using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    // Serialized fields
    [SerializeField] private GameObject weaponInRightHand;
    [SerializeField] private GameObject weaponInLeftHand;
    [SerializeField] private int curWeaponnum;
    [SerializeField] private int curskillLv;
    [SerializeField] private List<int> weaponsLv = new List<int> { 0, 0, 0, 0 };
    [SerializeField] private List<int> weaponsSkillLv = new List<int> { 0, 0, 0, 0 };
    [SerializeField] private List<int> weaponsLabel;
    [SerializeField] private List<string> weaponsDescription = new List<string>();
    [SerializeField] private List<WeaponData> invWeapon;

    // Private fields
    private SphereCollider gainItemRange;
    private List<float> flipBurstCooltime = new List<float> { 20f, 20f, 20f, 20f };
    private List<int> statusLv = new List<int>();
    private List<float> statPerLv = new List<float> { 1f, 0.1f, 0.1f, 25f, 50f, 1f, 0.5f, 0.1f, 1f };
    private float basicGainRange;
    private float systemTime = 0f;

    private float Exp;
    private float MaxHp => 100 + Value_AddiHp;
    private float hitPoint;
    public float HitPoint{
        get{ return hitPoint; }
        set{ hitPoint = value;}
    }
    private float Barrior;
    private int PlayerLevel = 1;

    private List<float> staminas = new List<float> { 0f, 0f, 0f, 0f };

    // UI Components
    private Scrollbar HPBar, BarriorBar, ExpBar;
    private List<Scrollbar> StaminaBars = new List<Scrollbar>();
    private List<RawImage> UI_Weapon = new List<RawImage>();
    private List<Text> FlipBurstCoolText = new List<Text>();
    private Text Text_Level, Text_Time;

    // Singleton implementation
    private static PlayerStatus instance;
    public static PlayerStatus Instance => instance ??= FindObjectOfType<PlayerStatus>();

    // Getter and Setter Properties
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

    public int CurSkillLv
    {
        get => curskillLv;
        set => curskillLv = value;
    }

    public List<int> WeaponsLv{
        get => weaponsLv;
    }
    public List<int> WeaponsSkillLv {
        get => weaponsSkillLv;
    }
    public List<int> WeaponsLabel {
        get => weaponsLabel;
    }
    public List<string> WeaponsDescription {
        get => weaponsDescription;
    }
    public List<WeaponData> InvWeapon {
        get => invWeapon;
    } 
    public List<float> FlipBurstCooltime {
        get => flipBurstCooltime;
    } 
    public List<int> StatusLv {
        get => statusLv;
    } 
    public List<float> StatPerLv{
        get => statPerLv;
    } 
    public List<float> Staminas{
        get => staminas;    
    } 

    // Calculated Properties
    public float Value_AdditionalWeaponPower => statusLv[0] * statPerLv[0];
    public float Value_LastDamage => 1 + statusLv[1] * statPerLv[1];
    public float Value_Speed => 5 + 5 * statusLv[2] * statPerLv[2];
    public float Value_Defense => statusLv[3] * statPerLv[3];
    public float Value_AddiHp => statusLv[4] * statPerLv[4];
    public float Value_HpRecoverySpeed => 1 + statusLv[5] * statPerLv[5];
    public float Value_GainItemRange => 1 + statusLv[6] * statPerLv[6];
    public float Value_AttackRange => 1 + statusLv[7] * statPerLv[7];
    public float Value_StaminaRecoverySpeed => 2f + statusLv[8] * statPerLv[8];
    private float CurNeedExp => 40 + Mathf.Pow(PlayerLevel, 1.2f * Mathf.Log(PlayerLevel, 7));

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeComponents();
        LoadWeaponDescriptions();
        InitializeStats();
    }

    private void FixedUpdate()
    {
        UpdateStats();
        UpdateUI();
    }

    private void InitializeComponents()
    {
        gainItemRange = transform.GetChild(0).GetComponent<SphereCollider>();
        basicGainRange = gainItemRange.radius;

        HPBar = GameObject.Find("HPScrollbar").GetComponent<Scrollbar>();
        BarriorBar = GameObject.Find("BarriorScrollbar").GetComponent<Scrollbar>();
        ExpBar = GameObject.Find("ExpBar").GetComponent<Scrollbar>();

        for (int i = 0; i < 4; i++)
        {
            StaminaBars.Add(GameObject.Find($"StaScrollbar{i}").GetComponent<Scrollbar>());
            GameObject weaponUI = GameObject.Find($"WeaponImage{i}");
            UI_Weapon.Add(weaponUI.GetComponent<RawImage>());
            FlipBurstCoolText.Add(weaponUI.transform.GetChild(0).GetComponent<Text>());
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
        HitPoint = MaxHp / 2;

#if UNITY_EDITOR
        statusLv[8] = 40;
        flipBurstCooltime = new List<float> { 0, 0, 0, 0 };
#endif
    }

    private void UpdateStats()
    {
        for (int i = 0; i < 4; i++)
        {
            flipBurstCooltime[i] -= Time.fixedDeltaTime;
            staminas[i] += staminas[i] < 100 ? Time.fixedDeltaTime * Value_StaminaRecoverySpeed : 0;
        }

        HitPoint = Mathf.Min(HitPoint + Value_HpRecoverySpeed * Time.fixedDeltaTime, MaxHp);
        gainItemRange.radius = basicGainRange * Value_GainItemRange;

        systemTime += Time.fixedDeltaTime;
    }

    private void UpdateUI()
    {
        HPBar.size = HitPoint / MaxHp;
        BarriorBar.size = Barrior / MaxHp;
        ExpBar.size = Exp / CurNeedExp;

        for (int i = 0; i < StaminaBars.Count; i++)
        {
            StaminaBars[i].size = staminas[i] / 100f;
            FlipBurstCoolText[i].text = Mathf.Max(0, Mathf.FloorToInt(flipBurstCooltime[i])).ToString();
        }

        for (int i = 0; i < invWeapon.Count; i++)
        {
            UI_Weapon[i].texture = invWeapon[i].WeaponImage;
        }

        UI_Weapon[curWeaponnum].gameObject.transform.localScale = Vector3.one * 2;

        Text_Level.text = $"Lv : {PlayerLevel}";
        TimeSpan timeSpan = TimeSpan.FromSeconds(systemTime);
        Text_Time.text = $"{timeSpan.Minutes:D2} : {timeSpan.Seconds:D2}";
    }

    public void GetExpPoint(float exp)
    {
        Exp += exp;
        while (Exp >= CurNeedExp && PlayerLevel < 50)
        {
            Exp -= CurNeedExp;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        PlayerLevel++;
        DataBase.Instance.LvUp();
    }

    public void OnDamaged(float dmg)
    {
        if (Barrior > 0)
        {
            float remainingDmg = dmg - Barrior;
            Barrior = Mathf.Max(0, Barrior - dmg);
            dmg = remainingDmg > 0 ? remainingDmg : 0;
        }

        HitPoint -= dmg * 100f / (100f + Value_Defense);
        if (HitPoint <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exp"))
        {
            GetExpPoint(float.Parse(other.name.Substring(9)));
            Destroy(other.gameObject);
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
    public void WeaponAdd()
    {
        Instantiate(invWeapon[invWeapon.Count - 1].Item, weaponInRightHand.transform).SetActive(false);
        weaponsLabel[invWeapon.Count - 1] = invWeapon[invWeapon.Count - 1].WeaponLabel;
    }

}
