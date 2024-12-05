using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class DataBase : MonoBehaviour
{
    [SerializeField] List<WeaponData> items = new List<WeaponData>();
    [SerializeField] List<SkillData> skills = new List<SkillData>();
    [SerializeField] Canvas UI_AfterLevelUp;
    [SerializeField] List<Button> SelectAugs;
    [SerializeField] List<Text> text;
    [SerializeField] List<Text> textcontent1;
    [SerializeField] List<Text> textcontent2;
    public IReadOnlyList<WeaponData> Items => items;
    public IReadOnlyList<SkillData> Skills => skills;
    static private DataBase instance = null; // 싱글톤
    public static DataBase Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    //
    //
    //
    //
    //
    //
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void OnEnable()
    {
        // 델리게이트 체인 추가
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        // 델리게이트 체인 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Substring(0, 3) != "Map") return;
        UI_AfterLevelUp = Instantiate(UI_AfterLevelUp);

        var a = UI_AfterLevelUp.transform.GetComponentInChildren<Transform>();
        int i = 0;
        foreach (Transform item in a)
        {
            if (i == 4) break;
            var b = item.transform.GetComponentInChildren<Transform>();
            foreach (Transform bitem in b)
            {
                Debug.Log(bitem);
                switch (i)
                {
                    case 1:
                        text.Add(bitem.GetComponent<Text>());
                        break;
                    case 2:
                        textcontent1.Add(bitem.GetComponent<Text>());
                        break;
                    case 3:
                        textcontent2.Add(bitem.GetComponent<Text>());
                        SelectAugs.Add(bitem.GetComponent<Text>().transform.GetChild(0).GetComponent<Button>());
                        break;
                    case 4:
                        break;
                }
            }
            i++;
        }

    }
    enum Aug
    {
        WeaponPower, LastDamage, Speed, Defense, AddiHp,
        HpRecovery, GainItemRange, AttackRange, StaminaRecovery
    }
    public void LvUp()
    {
        UIrr(CanLvUpAug());
    }
    List<Aug> CanLvUpAug()
    {
        List<Aug> CanLvUpAugList = new List<Aug>();
        for (int i = 0; i < PlayerStatus.Instance.StatusLv.Count; i++)
        {
            if (i == 0)
            {
                if (PlayerStatus.Instance.StatusLv[i] < 9)
                { 
                    CanLvUpAugList.Add((Aug)i);
                }
            }
            else if (PlayerStatus.Instance.StatusLv[i] < 4)
            {
                CanLvUpAugList.Add((Aug)i);
            }
        }
        // 리스트 섞기
        for (int i = 0; i < CanLvUpAugList.Count; i++)
        {
            int randomIndex = Random.Range(i, CanLvUpAugList.Count);
            int temp = (int)CanLvUpAugList[i];
            CanLvUpAugList[i] = CanLvUpAugList[randomIndex];
            CanLvUpAugList[randomIndex] = (Aug)temp;
        }

        return CanLvUpAugList.Count >= 3 ? CanLvUpAugList.GetRange(0, 3) : CanLvUpAugList;
    }
    void UIrr(List<Aug> Augs)
    {
        PlayerStatus p = PlayerStatus.Instance;
        // if(!UI_AfterLevelUp)    FindUI();
        if (Augs.Count == 0) return;
        UI_AfterLevelUp.gameObject.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < Augs.Count; i++)
        {
            int AugLv = p.StatusLv[(int)Augs[i]];
            int Aug = (int)Augs[i];
            text[i].text = Augs[i].ToString();
            textcontent1[i].text = $"Lv : {AugLv} -> Lv : {AugLv + 1}";
            if(Aug == 0){
                textcontent2[i].text = p.GetWeaponsDescription(AugLv + 10*PlayerStatus.Instance.WeaponsLabel[PlayerStatus.Instance.CurWeaponnum]+1);
            }
            else {
                textcontent2[i].text = $"{p.StatPerLv[Aug] * AugLv} -> {p.StatPerLv[Aug] * (AugLv + 1)}";
            }
            SelectAugs[i].onClick.AddListener(() => StatusUp(Aug, AugLv + 1));
        }
        for (int i = Augs.Count; i < 3; i++)
        {
            text[i].text = "There are no augmentations here";
            textcontent1[i].text = $"that allow you to level up";
            textcontent2[i].text = $"";
        }
    }
    void StatusUp(int Aug, int NextLv)
    {
        PlayerStatus.Instance.StatusLv[Aug] = NextLv;
        if (Aug == 0) {
            PlayerStatus.Instance.WeaponInRightHand.transform.GetChild(PlayerStatus.Instance.CurWeaponnum).GetComponent<WeaponAttack>().Reset();
            PlayerStatus.Instance.WeaponInRightHand.transform.GetChild(PlayerStatus.Instance.CurWeaponnum).GetComponent<WeaponAttack>().LevelSet(NextLv);
        }
        foreach (var Btn in SelectAugs)
        {
            Btn.onClick.RemoveAllListeners();
        }
        UI_AfterLevelUp.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
