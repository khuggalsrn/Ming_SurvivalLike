using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "NewMonster", menuName = "Monster/Monster Data")]
public class MonsterInfo : ScriptableObject
{
    public List<GameObject> items;
    public float MaxHp;
    public float AttackPower;
    public float MoveSpeed;
    public List<float> DefenseForType = new List<float>(new float[Enum.GetValues(typeof(DamageType)).Length]);
}
