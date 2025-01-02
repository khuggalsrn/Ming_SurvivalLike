using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public int Gold { get; private set; } // 골드
    public int ViewGold;
    public List<int> StatusLv {get; private set;} // 스탯 레벨
    public ListData StatusValuePerLv ; // 스탯 레벨당 값 float
    public List<int> ItemsLv {get; private set;}  // 아이템 레벨
    private const string GoldKey = "Gold";
    private const string StatusLvKey = "StatusLv";
    private const string ItemsLvKey = "ItemsLv";
    static private UserData instance = null; // 싱글톤
    public static UserData Instance
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
    void Start(){
        LoadData();
    }
    public bool UpgradeStat(int statIndex, int cost)
    {
        if (statIndex < 0 || statIndex >= StatusLv.Count) return false;

        if (SpendGold(cost)) // 골드 차감 성공 시
        {
            StatusLv[statIndex]++; // 해당 스탯 업그레이드
            SaveData(); // 즉시 저장
            return true;
        }
        return false;
    }
    public bool UpgradeItem(int ItemIndex, int cost)
    {
        if (ItemIndex < 0 || ItemIndex >= ItemsLv.Count) return false;

        if (SpendGold(cost)) // 골드 차감 성공 시
        {
            ItemsLv[ItemIndex]++; // 해당 스탯 업그레이드
            SaveData(); // 즉시 저장
            return true;
        }
        return false;
    }
    // 리스트 저장
    private void SaveList(string key, List<int> list)
    {
        string data = string.Join(",", list);
        PlayerPrefs.SetString(key, data);
    }

    // 리스트 로드
    private List<int> LoadList(string key)
    {
        string data = PlayerPrefs.GetString(key, "");
        if (string.IsNullOrEmpty(data)) return null;

        string[] splitData = data.Split(',');
        List<int> result = new List<int>();
        foreach (string value in splitData)
        {
            if (int.TryParse(value, out int parsed))
                result.Add(parsed);
        }
        return result;
    }
    // 데이터 로드
    private void LoadData()
    {
        Gold = PlayerPrefs.GetInt(GoldKey, 0); // 기본값: 0

        StatusLv = LoadList(StatusLvKey);
        ItemsLv = LoadList(ItemsLvKey);

        if (StatusLv == null) // 기본값 세팅
            StatusLv = new List<int>(new int[16]); // enum InitStatus
        if (ItemsLv == null)
            ItemsLv = new List<int>(new int[3]); // enum Items... etc : reroll item
    }
    private void SaveData()
    {
        PlayerPrefs.SetInt(GoldKey, Gold);
        SaveList(StatusLvKey, StatusLv);
        SaveList(ItemsLvKey, ItemsLv);
        PlayerPrefs.Save();
    }

    // 골드 사용
    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            SaveData(); // 즉시 저장
            ViewGold = Gold;
            return true;
        }
        return false; // 골드 부족
    }
    public void earseAll()
    {
#if UNITY_EDITOR
        SpendGold(0);
        PlayerPrefs.SetInt(GoldKey, 6666);
        SaveList(StatusLvKey, new List<int>() {});
        SaveList(ItemsLvKey, new List<int>() {});
        PlayerPrefs.Save();
        LoadData();
#endif
    }
}
