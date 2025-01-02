using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Text YourGold;
    private UserData userData => UserData.Instance;
    [SerializeField] private Button[] ItemButtons;
    [SerializeField] private Button[] StatButtons;
    [SerializeField] private Text[] ItemShopCosts;
    [SerializeField] private Text[] StatShopCosts;
    public ListData ItemCost;
    private List<int> itemCost => ItemCost.intList;
    public ListData StatCost;
    public List<int> statCost => StatCost.intList;
    GameObject curText = null;
    void Start()
    {
        YourGold.text = userData.Gold.ToString();
        ItemButtons = transform.GetChild(1).GetComponentsInChildren<Button>();
        for (int i = 0; i < ItemButtons.Length; i++)
        {
            if (i == ItemButtons.Length) return;
            int t = i;
            ItemButtons[t].onClick.AddListener(() => UpgradeItem(t, itemCost[t]));
        }

        StatButtons = transform.GetChild(2).GetComponentsInChildren<Button>();
        for (int i = 0; i < StatButtons.Length; i+=2)
        {
            if (i == StatButtons.Length) return;
            int t = i;
            StatButtons[t].onClick.AddListener(() => StatScriptTextOn(StatButtons[t].transform.GetChild(2).gameObject));
            StatButtons[t+1].onClick.AddListener(() => UpgradeStat((t)/2, statCost[(t)/2]));
        }

        ItemShopCosts = transform.GetChild(1).GetComponentsInChildren<Text>();
        for (int i = 0; i < itemCost.Count; i++)
        {
            if (i == ItemShopCosts.Length) return;
            int t = i;
            ItemShopCosts[t].text = itemCost[i].ToString();
        }
        
        StatShopCosts = transform.GetChild(2).GetComponentsInChildren<Text>();
        for (int i = 0; i < statCost.Count; i++)
        {
            if (i == StatShopCosts.Length) return;
            int t = i;
            StatShopCosts[t].text = statCost[i].ToString();
        }

    }
    public void StatScriptTextOn(GameObject Text){
        if (curText != null) curText.SetActive(false);
        curText = Text;
        curText.SetActive(true);
    }
    // 특정 스탯을 업그레이드
    public void UpgradeStat(int statIndex, int cost)
    {
        bool success = userData.UpgradeStat(statIndex, cost);
        if (success)
        {
            Debug.Log($"스탯 {statIndex} 업그레이드 성공! 현재 레벨: {userData.StatusLv[statIndex]}");
            YourGold.text = userData.Gold.ToString();
        }
        else
        {
            Debug.Log("골드 부족 또는 잘못된 스탯 인덱스");
        }
    }
    // 특정 아이템을 업그레이드
    public void UpgradeItem(int itemIndex, int cost)
    {
        bool success = userData.UpgradeItem(itemIndex, cost);
        if (success)
        {
            Debug.Log($"아이템 {itemIndex} 업그레이드 성공! 현재 레벨: {userData.ItemsLv[itemIndex]}");
            YourGold.text = userData.Gold.ToString();
        }
        else
        {
            Debug.Log($"{itemIndex} 골드 부족 또는 잘못된 아이템 인덱스");
        }
    }
}
