using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newLists", menuName = "List/Lists Data")]
public class ListData : ScriptableObject
{
    public List<int> intList;
    public List<float> floatList;
    public List<string> stringList;
}
