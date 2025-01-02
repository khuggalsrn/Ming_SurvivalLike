using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTower : MonoBehaviour
{
    public QuestData questData;
    private bool isQuestActive = false; // 퀘스트 진행 상태
    private string questName => questData.questName; // 퀘스트 이름
    private int questID => questData.questID; // 퀘스트 번호
    private float questAreaRadius => questData.questAreaRadius; // 동그란 Trigger Collider의 Radius크기 (퀘스트 영역)
    private GameObject generatedObject => questData.generatedObject; // 퀘스트 오브젝트 (보스 등 신호를 보낼 오브젝트)
    private List<GameObject> Rewards => questData.Rewards; // 퀘스트 오브젝트 (보스 등 신호를 보낼 오브젝트)
    private float rotationSpeed = 120f; // 회전 속도 (초당 각도)

     void Start()
    {
        if (GetComponent<SphereCollider>() != null)
        {
            // Trigger Collider 확인
            GetComponent<SphereCollider>().radius = questAreaRadius;
        }
    }
    void Update()
    {
        // y축 기준으로 계속 회전
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isQuestActive)
        {
            Debug.Log($"퀘스트 '{questName}' 시작!");
            isQuestActive = true;
            
            // 퀘스트 진행 로직 시작
            StartQuest();
        }
    }

    private void StartQuest()
    {
        // 퀘스트 실행 시 새로운 오브젝트 생성
        GameObject temp = Instantiate(generatedObject, transform.position, Quaternion.identity);
        var questSignal = temp.AddComponent<QuestSignal>();

        // 신호 이벤트 등록
        questSignal.OnQuestSignal += HandleQuestSignal;
    }

    private void HandleQuestSignal(bool success)
    {
        if(!isQuestActive) return;
        isQuestActive = false;
        if (success)
        {
            Debug.Log($"퀘스트 {questID} 성공!");
            CreateRewardBox();
        }
        else
        {
            Debug.Log($"퀘스트 {questID} 실패.");
            EndQuest();
        }

    }

    private void CreateRewardBox()
    {
        for(int i = 0; i < Rewards.Count; i++){
            Instantiate(Rewards[i], transform.position + Quaternion.Euler(0, 360f / Rewards.Count * i, 0) * Vector3.forward * 3.0f, Quaternion.identity);//원으로 둥글게
        }
        EndQuest();

    }

    private void EndQuest()
    {
        Destroy(this.gameObject,0.2f);
    }
}
