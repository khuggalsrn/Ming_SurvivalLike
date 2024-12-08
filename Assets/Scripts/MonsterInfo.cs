using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewMonster", menuName = "Monster/Monster Data")]
public class MonsterInfo : ScriptableObject
{
    public List<GameObject> items;
    public float MaxHp;
    public float AttackPower;
    public float MoveSpeed;
}
