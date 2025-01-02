using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class UI_Manager : MonoBehaviour
{
    [Header("Option")]
    public Canvas Option;
    // 
    //
    // 인게임
    //
    //
    [Header("InGame")]
    [SerializeField] Canvas UI_AfterLevelUp;
    [SerializeField] List<Button> SelectAugs;
    [SerializeField] List<Text> Stattext;
    [SerializeField] List<Text> Stattextcontent1;
    [SerializeField] List<Text> Stattextcontent2;
    PlayerStatus player => PlayerStatus.Instance;
    [SerializeField] Canvas UI_ItemPickUp;
    [SerializeField] List<Button> SelectItems;
    [SerializeField] List<Text> Itemtext;
    [SerializeField] List<Text> Itemtextcontent1;
    [SerializeField] List<Text> Itemtextcontent2;
    [SerializeField] Canvas CurUI_AfterLevelUp;
    [SerializeField] Canvas CurUI_ItemPickUp;
    private int order = 0;
    static private UI_Manager instance = null; // 싱글톤
    public static UI_Manager Instance
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
    // 3중 택1 UI
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
    IEnumerator UI_Setting()
    {
        yield return new WaitForEndOfFrame();

        SelectAugs.Clear();
        Stattext.Clear();
        Stattextcontent1.Clear();
        Stattextcontent2.Clear();
        SelectItems.Clear();
        Itemtext.Clear();
        Itemtextcontent1.Clear();
        Itemtextcontent2.Clear();
        CurUI_AfterLevelUp = Instantiate(UI_AfterLevelUp);
        CurUI_ItemPickUp = Instantiate(UI_ItemPickUp);
        CurUI_AfterLevelUp.gameObject.SetActive(false);
        CurUI_ItemPickUp.gameObject.SetActive(false);
        for (int j = 0; j < 4; j++) //UI.length == 4
        {
            Debug.Log($"{j} {CurUI_AfterLevelUp.transform.GetChild(j).name}");
            if (j == 0) continue; //BackGround, j == 1> Name, 2>Level, 3>Value

            var a = CurUI_AfterLevelUp.transform.GetChild(j);
            var c = CurUI_ItemPickUp.transform.GetChild(j);

            var b = a.GetComponentsInChildren<Text>();
            var d = c.GetComponentsInChildren<Text>();

            for (int k = 0; k < b.Length; k++)
            {
                switch (j)
                {
                    case 1:
                        Stattext.Add(b[k].GetComponent<Text>());
                        Itemtext.Add(d[k].GetComponent<Text>());
                        break;
                    case 2:
                        Stattextcontent1.Add(b[k].GetComponent<Text>());
                        Itemtextcontent1.Add(d[k].GetComponent<Text>());
                        break;
                    case 3:
                        Stattextcontent2.Add(b[k].GetComponent<Text>());
                        Itemtextcontent2.Add(d[k].GetComponent<Text>());
                        SelectAugs.Add(b[k].GetComponent<Text>().transform.GetChild(0).GetComponent<Button>());
                        SelectItems.Add(d[k].GetComponent<Text>().transform.GetChild(0).GetComponent<Button>());
                        break;
                    default:
                        break;
                }
            }

        }
    }
    void OnEnable()
    {
        // 델리게이트 체인 추가
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneLoaded += OnRobbyScene;
    }
    void OnDisable()
    {
        // // 델리게이트 체인 제거
        // SceneManager.sceneLoaded -= OnSceneLoaded;
        // SceneManager.sceneLoaded -= OnRobbyScene;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Substring(0, 3) != "Map") return;

        StartCoroutine(UI_Setting());

    }
    IReadOnlyList<WeaponData> CanPick_NewWeapons => GameManager.Instance.Weapons;
    // FlipBurst 0 == none, 1 == first FB, 2 == second FB
    public List<Aug> CanLvUpAug()
    {
        List<Aug> CanLvUpAugList = new List<Aug>();
        for (int i = 0; i < PlayerStatus.Instance.InvWeapon.Count; i++)
        {
            if (PlayerStatus.Instance.WeaponsLv[i] < 9) // 아직 10렙 미만
            {
                if (i == 0 || i == 1)
                {
                    if (PlayerStatus.Instance.WeaponsLv[i] < 6) // 아직 7렙미만
                    {
                        CanLvUpAugList.Add((Aug)i);
                    }
                    else if (PlayerStatus.Instance.WeaponsLv[0] + PlayerStatus.Instance.WeaponsLv[1] > 11)
                    { // 둘 다 7렙이상
                        CanLvUpAugList.Add((Aug)i); //진화렙업 가능
                    }
                }
                else if (PlayerStatus.Instance.WeaponsLv[i] < 4) // 보조무기 2개의 경우 5렙 미만
                { // && (i == 2 || i == 3) 필요없는 조건문
                    CanLvUpAugList.Add((Aug)i);
                }
            }
        }
        for (int i = 4; i < PlayerStatus.Instance.StatusLv.Count + 3; i++)
        {
            if (PlayerStatus.Instance.StatusLv[i - 3] < 10) //CurLv < Level(N)
            {
                CanLvUpAugList.Add((Aug)i);
            }
        }
        for (int i = 0; i < CanPick_NewWeapons.Count; i++)
        {
            if (!PlayerStatus.Instance.InvWeapon.Contains(CanPick_NewWeapons[i]))
            {
                CanLvUpAugList.Add((Aug)(CanPick_NewWeapons[i].WeaponLabel + 12));
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
    public void UI_LevelUp(List<Aug> Augs)
    {
        PlayerStatus p = PlayerStatus.Instance;
        // if(!UI_AfterLevelUp)    FindUI();
        if (Augs.Count == 0) return;
        CurUI_AfterLevelUp.gameObject.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < Augs.Count; i++)
        {
            int Aug = (int)Augs[i];
            Stattext[i].text = Augs[i].ToString();
            if (Aug < 4)
            {
                Stattextcontent1[i].text = $"Lv : {PlayerStatus.Instance.WeaponsLv[Aug] + 1} -> Lv : {PlayerStatus.Instance.WeaponsLv[Aug] + 2}";
                Stattextcontent2[i].text = p.GetWeaponsDescription(PlayerStatus.Instance.WeaponsLv[Aug] + 10 * PlayerStatus.Instance.InvWeapon[Aug].WeaponLabel + 1);
                SelectAugs[i].onClick.AddListener(() => StatusUp(Aug, PlayerStatus.Instance.WeaponsLv[Aug] + 1));
            }
            else if (Aug < 12)
            {
                int AugLv = p.StatusLv[(int)Augs[i] - 3];
                Stattextcontent1[i].text = $"Lv : {AugLv + 1} -> Lv : {AugLv + 2}";
                Stattextcontent2[i].text = $"{p.StatPerLv[Aug - 3] * AugLv} -> {p.StatPerLv[Aug - 3] * (AugLv + 1)}";
                SelectAugs[i].onClick.AddListener(() => StatusUp(Aug, AugLv + 1));
            }
            else if (Aug < 16)
            {
                Stattextcontent1[i].text = $"You Can PickUp this Weapon.";
                Stattextcontent2[i].text = $"This Weapon will be {PlayerStatus.Instance.InvWeapon.Count + 1}th Slot.";
                SelectAugs[i].onClick.AddListener(() => StatusUp(Aug, 1));
            }
            else if (Aug < 24)
            {
                Stattextcontent1[i].text = $"You Can Uses This Slot's Weapon's FlipBurst";
                Stattextcontent2[i].text = $"FlipBurst {(Aug - 16) % 2 + 1}th will be READY.";
                SelectAugs[i].onClick.AddListener(() => StatusUp(Aug, 1));
            }
        }
        for (int i = Augs.Count; i < 3; i++)
        {
            Stattext[i].text = "There are no augmentations here";
            Stattextcontent1[i].text = $"that allow you to level up";
            Stattextcontent2[i].text = "There are no augmentations here.\n that allow you to level up";
        }
    }
    void StatusUp(int Aug, int NextLv)
    {
        if (Aug < 4)
        {
            PlayerStatus.Instance.WeaponsLv[Aug] = NextLv;
            if (Aug == PlayerStatus.Instance.CurWeaponnum) PlayerStatus.Instance.StatusLv[0] = NextLv;
            GameObject targetweapon = PlayerStatus.Instance.WeaponInRightHand.transform.GetChild(Aug).gameObject;
            targetweapon.gameObject.SetActive(true);
            targetweapon.GetComponent<WeaponAttack>().Reset();
            targetweapon.GetComponent<WeaponAttack>().LevelSet(NextLv);
            if (Aug != PlayerStatus.Instance.CurWeaponnum && Aug < 2) targetweapon.gameObject.SetActive(false);
        }
        else if (Aug < 12)
        {
            PlayerStatus.Instance.StatusLv[Aug - 3] = NextLv;
        }
        else if (Aug < 16)
        {
            for (int i = 0; i < CanPick_NewWeapons.Count; i++)
            {
                if (CanPick_NewWeapons[i].WeaponLabel + 12 == Aug)
                {
                    PlayerStatus.Instance.WeaponAdd(CanPick_NewWeapons[i]);
                    break;
                }
            }
        }
        foreach (var Btn in SelectAugs)
        {
            Btn.onClick.RemoveAllListeners();
        }
        CurUI_AfterLevelUp.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void MakeItem()
    {
        List<ItemData> Datas;
        if (order == 0 || order == 4)
        {
            Datas = new List<ItemData>(GameManager.Instance.CoreItems);
        }
        else
        {
            Datas = new List<ItemData>(GameManager.Instance.NormalItems);
        }
        for (int i = 0; i < Datas.Count; i++)
        {
            int randomIndex = Random.Range(i, Datas.Count);
            ItemData temp = Datas[i];
            Datas[i] = Datas[randomIndex];
            Datas[randomIndex] = temp;
        }
        order += 1;
        UI_Selection(Datas.Count >= 3 ? Datas.GetRange(0, 3) : Datas);
    }
    void UI_Selection(List<ItemData> datas)
    {
        CurUI_ItemPickUp.gameObject.SetActive(true);
        Time.timeScale = 0;
        for (int i = 0; i < datas.Count; i++)
        {
            int index = i;
            string title = datas[index].itemName;
            Itemtext[index].text = $"{title}";
            Itemtextcontent1[index].text = $"";
            Itemtextcontent2[index].text = $"{datas[index].itemScript}";
            SelectItems[index].onClick.AddListener(() => LootItem(datas[index]));
        }
        for (int i = datas.Count; i < 3; i++)
        {
            Itemtext[i].text = "No item exist.";
            Itemtextcontent1[i].text = $"";
            Itemtextcontent2[i].text = "Select any slot except this one.";
        }
        Debug.LogWarning("UI까진 됐어");
    }
    void LootItem(ItemData item)
    {
        Debug.LogWarning("Button까진 됐어");
        foreach (var effect in item.StatEffects)
        {
            if ((int)effect.Stat == 1) player.AddiStats[1] += effect.value;
            else player.AddiStats[(int)effect.Stat] += effect.value;
        }
        foreach (var Btn in SelectItems)
        {
            Btn.onClick.RemoveAllListeners();
        }
        CurUI_ItemPickUp.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    // 
    //
    // 게임 시작 전
    //
    //
    [Header("MainSceen")]
    [SerializeField] Canvas MainScreen;
    Canvas Cur_MainScreen;
    [Header("Btn_Start")]
    [SerializeField] Button Btn_Start;
    [Header("Btn_Setting")]
    [SerializeField] Button Btn_Settings;
    [Header("GameSelect")]
    [SerializeField] Canvas UI_AvatarSelect;
    [SerializeField] Canvas UI_WeaponSelect;
    [SerializeField] Canvas UI_Shop;
    List<Button> avatarSelectButtons = new List<Button>();
    List<Button> weaponSelectButtons = new List<Button>();
    Canvas CurUI_AvatarSelect;
    Canvas CurUI_WeaponSelect;
    Canvas CurUI_Shop;
    public IReadOnlyList<Button> AvatarSelectButtons => avatarSelectButtons;
    public IReadOnlyList<Button> WeaponSelectButtons => weaponSelectButtons;
    void OnRobbyScene(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name.Substring(0, 4).CompareTo("Game") == 0)
        {
            Cur_MainScreen = Instantiate(MainScreen);
            Btn_Settings = Cur_MainScreen.transform.GetChild(1).GetComponent<Button>();
            Btn_Start = Cur_MainScreen.transform.GetChild(2).GetComponent<Button>();
            Btn_Settings.onClick.AddListener(() => F_AvatarSelectOpen());
            Btn_Settings.onClick.AddListener(() => F_WeaponSelectOpen());
            Btn_Settings.onClick.AddListener(() => UserData.Instance.earseAll());
            Btn_Start.onClick.AddListener(() => GameManager.Instance.GameStart("Site"));
        }
    }
    void F_AvatarSelectOpen()
    {
        if (!CurUI_AvatarSelect)
        {
            CurUI_AvatarSelect = Instantiate(UI_AvatarSelect);
            var a = CurUI_AvatarSelect.transform.GetComponentInChildren<Transform>();
            foreach (Transform item in a)
            {
                if (item.GetComponent<Button>())
                    avatarSelectButtons.Add(item.GetComponent<Button>());
            }
            for (int i = 0; i < GameManager.Instance.Avatars.Count; i++)
            {
                int t = i;
                if (avatarSelectButtons.Count == t) break;
                avatarSelectButtons[i].onClick.AddListener(() => GameManager.Instance.UserStat.AvatarNum = t);
            }
            avatarSelectButtons[avatarSelectButtons.Count - 2].onClick.AddListener(() => F_ShopOpen());
            avatarSelectButtons[avatarSelectButtons.Count - 1].onClick.AddListener(() => CurUI_AvatarSelect.gameObject.SetActive(false));
            avatarSelectButtons[avatarSelectButtons.Count - 1].onClick.AddListener(() => CurUI_WeaponSelect.gameObject.SetActive(false));
            avatarSelectButtons[avatarSelectButtons.Count - 1].onClick.AddListener(() => CurUI_Shop.gameObject.SetActive(false));
        }
        CurUI_AvatarSelect.gameObject.SetActive(true);
    }
    void F_WeaponSelectOpen()
    {
        if (!CurUI_WeaponSelect)
        {
            CurUI_WeaponSelect = Instantiate(UI_WeaponSelect);
            var a = CurUI_WeaponSelect.transform.GetComponentInChildren<Transform>();
            foreach (Transform item in a)
            {
                if (item.GetComponent<Button>())
                    weaponSelectButtons.Add(item.GetComponent<Button>());
            }
            for (int i = 0; i < GameManager.Instance.Weapons.Count; i++)
            {
                int t = i;
                if (weaponSelectButtons.Count == t) break;
                weaponSelectButtons[t].onClick.AddListener(() => GameManager.Instance.UserStat.InitWeapon = GameManager.Instance.Weapons[t]);
            }
        }
        CurUI_WeaponSelect.gameObject.SetActive(true);
    }
    void F_ShopOpen()
    {
        if (!CurUI_Shop)
        {
            CurUI_Shop = Instantiate(UI_Shop);
            CurUI_Shop.gameObject.SetActive(true);
        }
        else
            CurUI_Shop.gameObject.SetActive(!CurUI_Shop.gameObject.activeSelf);
    }

    // 
    //
    // 게임 끝난 후
    //
    //
    [Header("GameOver")]
    [SerializeField] Canvas UI_GameEnd;
    Canvas CurUI_GameEnd;
    [Header("GameOver")]
    [SerializeField] Canvas UI_GameOver;
    Canvas CurUI_GameOver;
    [Header("GameClear")]
    [SerializeField] Canvas UI_GameClear;
    Canvas CurUI_GameClear;
    Button Btn_ReStart;
    Button Btn_Quit;
    public void F_GameEnd(bool isClear)
    {
        CurUI_GameEnd = Instantiate(UI_GameEnd);
        var ChildButtons = CurUI_GameEnd.GetComponentsInChildren<Button>();
        Btn_ReStart = ChildButtons[0];
        Btn_Quit = ChildButtons[1];

        Btn_ReStart.onClick.AddListener(() => LoadScene(SceneManager.GetActiveScene().name));
        Btn_Quit.onClick.AddListener(() => LoadScene("GameSelect"));
        if (isClear)
        {
            F_GameClearUIOpen();
        }
        else
        {
            F_GameOverUIOpen();
        }
    }
    void F_GameOverUIOpen()
    {
        CurUI_GameOver = Instantiate(UI_GameOver);
    }
    void F_GameClearUIOpen()
    {
        CurUI_GameClear = Instantiate(UI_GameClear);
    }
    void LoadScene(string name)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(name);
    }
}