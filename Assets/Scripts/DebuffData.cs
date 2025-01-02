using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDebuff", menuName = "Debuff/Debuff Data")]
public class DebuffData : ScriptableObject
{
    public Debuff debuff;
    public float AddiDamageRatio;
}
