using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatChange
{
    public Stats Stat; // 
    public float value; // 
}
[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemScript;
    public string itemName;
    public bool IsCoreItem;
    public List<StatChange> StatEffects = new List<StatChange>();
}