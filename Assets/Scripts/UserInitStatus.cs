using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserInitStatus", menuName = "Player/User Init Status")]
public class UserInitStatus : ScriptableObject
{
    [Header("General Settings")]
    public int AvatarNum;

    [Header("Weapon Settings")]
    public WeaponData InitWeapon;
    public float flipBurstCooltimeOfInitWeapon;

    [Header("Stats Settings")]
    public int InitPlayerLevel;
    public List<float> InitAddiStatusList{
        get {return InitAddiStatusValue.floatList;}
        set {InitAddiStatusValue.floatList = value;}
    }
    public ListData InitAddiStatusValue;
    public ListData BasicStatus;

}