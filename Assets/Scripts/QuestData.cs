using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questName; // 퀘스트 이름
    public int questID; // 퀘스트 번호
    public float questAreaRadius; // 동그란 Trigger Collider의 Radius크기 (퀘스트 영역)
    public GameObject generatedObject; // 퀘스트 오브젝트 (보스 등 신호를 보낼 오브젝트)
    public List<GameObject> Rewards; // 퀘스트 보상
}
