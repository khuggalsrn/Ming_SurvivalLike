using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    Vector3 PlayerInitPosition = new Vector3(-40, -15, 45);
    [SerializeField] List<WeaponData> weapons = new List<WeaponData>();
    [SerializeField] List<ItemData> coreItems = new List<ItemData>();
    [SerializeField] List<ItemData> normalItems = new List<ItemData>();
    [SerializeField] List<DebuffData> debuffs = new List<DebuffData>();
    public int InGameGold = 0;
    public UserInitStatus UserStat;
    public List<GameObject> Avatars;
    public IReadOnlyList<WeaponData> Weapons => weapons;
    public IReadOnlyList<ItemData> CoreItems => coreItems;
    public IReadOnlyList<ItemData> NormalItems => normalItems;
    public IReadOnlyList<DebuffData> Debuffs => debuffs;
    private int monsterNumLimit => 150 + Mathf.FloorToInt(PlayerStatus.Instance.AddiStats[(int)Stats.MonsterNumLimit]);
    [SerializeField] private GameObject player;
    public int MonsterNumLimit
    {
        get { return monsterNumLimit; }
    }
    [SerializeField] private int curMonsterNum = 0;
    public int CurMonsterNum
    {
        get { return curMonsterNum; }
        set { curMonsterNum = value; }
    }
    static private GameManager instance = null; // 싱글톤
    public static GameManager Instance
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
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    IEnumerator PlayerCharacterMake()
    {
        yield return new WaitForEndOfFrame();
        player = Instantiate(Avatars[UserStat.AvatarNum]);
        player.transform.position = PlayerInitPosition;
        player.AddComponent<PlayerStatus>();
        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerAttack>();
        
        int Length = UserData.Instance.StatusLv.Count;

        for (int t = 0; t < Length; t++)
        {
            int i = t;
            UserStat.InitAddiStatusList[i] = UserData.Instance.StatusLv[i] * UserData.Instance.StatusValuePerLv.floatList[i];
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
        Debug.Log("on" + scene.name);
        if (scene.name.Substring(0, 3) != "Map") return;

        StartCoroutine(PlayerCharacterMake());
        
        Debug.Log("onplayerload");
        InGameGold = 0;

    }
    
    public void LvUp()
    {
        UI_Manager.Instance.UI_LevelUp(UI_Manager.Instance.CanLvUpAug());
    }
    
    public void GameStart(string mapname)
    {
        SceneManager.LoadScene($"Map_{mapname}");
        Time.timeScale = 1;
    }
    public void GameOver()
    {
        GameEnd(false);
    }
    public void GameClear()
    {
        GameEnd(true);
    }
    void GameEnd(bool isClear)
    {
        int tempgold = -InGameGold; // 음수로해야 획득임
        InGameGold = 0;
        UserData.Instance.SpendGold(tempgold); // 인게임에서 얻은 골드 획득
        Time.timeScale = 0;
        UI_Manager.Instance.F_GameEnd(isClear);
    }
}
