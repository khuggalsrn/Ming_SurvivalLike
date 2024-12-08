using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    [System.Serializable]
    class MonsterList
    {
        public GameObject monsterPrefab; // 몬스터 프리팹
        public bool spawnOnce = false; // 이 몬스터는 1번만 스폰되는지 여부
    }
    [SerializeField] SpawnerInfo Spawner;
    List<MonsterList> monsterPrefabs; // 스폰할 몬스터 정보 배열
    private int currentMonsterIndex = 0; // 현재 스폰할 몬스터 인덱스
    private float spawnTimer = 0f; // 스폰 타이머
    private float groupTimer = 0f; // 그룹 전환 타이머    
    private bool hasSpawnedOnce = false; // 현재 몬스터가 한 번만 스폰되었는지 확인
    private int poolSize; // 몬스터 미리 생성
    private Queue<GameObject> monsterPool = new Queue<GameObject>(); // 미리 생성된 몬스터
    [SerializeField] bool iswaiting = true;
    //
    //
    //
    //
    //
    //
    IEnumerator Initialize(float time){
        iswaiting = true;
        yield return new WaitForSeconds(time);
        iswaiting = false;
        monsterPrefabs = new List<MonsterList>(Spawner.SpawnPrefabs.Count);
        poolSize = Mathf.FloorToInt(Spawner.spawnRate * Spawner.spawnInterval);
        if (Spawner.spawnAreaMin == Vector2.zero && Spawner.spawnAreaMax == Vector2.zero)
        {
            Spawner.spawnAreaMin = new Vector2(transform.position.x - Spawner.spawnRate / 2, transform.position.z - Spawner.spawnRate / 2);
            Spawner.spawnAreaMax = new Vector2(transform.position.x + Spawner.spawnRate / 2, transform.position.z + Spawner.spawnRate / 2);
        }
        // 미리 몬스터 생성
        for (int i = 0; i < Spawner.SpawnPrefabs.Count; i++)
        {
            MonsterList temp = new MonsterList { monsterPrefab = Spawner.SpawnPrefabs[i], spawnOnce = false };
            monsterPrefabs.Add(temp);
            if (Spawner.SpawnOnces.Contains(i)) monsterPrefabs[i].spawnOnce = true;
            var currentMonsterInfo = monsterPrefabs[i];
            for (int j = 0; j < poolSize; j++)
            {
                GameObject monster = Instantiate(currentMonsterInfo.monsterPrefab, transform.position, Quaternion.identity);
                monster.SetActive(false);
                monsterPool.Enqueue(monster);
                if (currentMonsterInfo.spawnOnce) break;
            }
        }
        SpawnMonster();//1초에 1마리 2초동안이면 2마리 소환되어야하는데 소환이 안되는 경우가 있음. 첫 몬스터 소환
    }
    void Start()
    {
        StartCoroutine(Initialize(Spawner.InitialSpawnDelay));
    }
    void FixedUpdate()
    {
        if (iswaiting) return;
        // 그룹 전환 타이머 업데이트
        groupTimer += 0.02f;
        spawnTimer += 0.02f;
        if (groupTimer < Spawner.spawnInterval)
        {
            // 몬스터 생성 타이머 업데이트
            if (spawnTimer >= 1f / Spawner.spawnRate)
            {
                spawnTimer = 0f;
                SpawnMonster();
            }
        }
        else
        {
            StartCoroutine(WaitandNext(5f));
        }
    }
    IEnumerator WaitandNext(float time)
    {
        iswaiting = true;
        groupTimer = 0f;
        spawnTimer = 0f;

        yield return new WaitForSeconds(time);
        iswaiting = false;
        currentMonsterIndex++;
        hasSpawnedOnce = false; // 새로운 그룹으로 넘어가면 다시 초기화
        SpawnMonster();
        if (currentMonsterIndex >= monsterPrefabs.Count)
        {
            Destroy(this);//다 만들면 삭제
        }

    }
    void SpawnMonster()
    {
        if (monsterPrefabs.Count == 0 || currentMonsterIndex >= monsterPrefabs.Count) return;

        var currentMonsterInfo = monsterPrefabs[currentMonsterIndex];

        // 스폰이 1회 제한된 경우, 이미 생성되었으면 스킵
        if (currentMonsterInfo.spawnOnce && hasSpawnedOnce) return;
        // 랜덤한 위치 찾기
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning("유효한 스폰 위치를 찾을 수 없습니다.");
            return; // 유효한 위치를 찾지 못한 경우
        }

        // 몬스터 생성
        GetMonster(spawnPosition, Quaternion.identity);
        if (currentMonsterInfo.spawnOnce)
        {
            hasSpawnedOnce = true;
        }
    }
    public GameObject GetMonster(Vector3 position, Quaternion rotation)
    {
        if (monsterPool.Count > 0)
        {
            GameObject monster = monsterPool.Dequeue();
            monster.transform.position = position;
            monster.transform.rotation = rotation;
            monster.SetActive(true);
            return monster;
        }
        else
        {
            Debug.LogWarning("Object Pool is empty!");
            return null;
        }
    }
    Vector3 GetValidSpawnPosition()
    {
        int maxAttempts = 10; // 위치를 찾기 위한 최대 시도 횟수
        for (int i = 0; i < maxAttempts; i++)
        {
            // 스폰 영역에서 랜덤 위치 계산
            Vector3 randomPosition = new Vector3(
                Random.Range(Spawner.spawnAreaMin.x, Spawner.spawnAreaMax.x),
                0, // Y 값은 NavMesh.SamplePosition에서 계산
                Random.Range(Spawner.spawnAreaMin.y, Spawner.spawnAreaMax.y)
            );
            randomPosition += this.transform.position;
            
            // NavMesh.SamplePosition으로 유효한 위치 확인
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                // 플레이어와의 안전 거리 확인 -> 미사용
                // if (Vector3.Distance(hit.position, player.position) >= safeDistance)

                return hit.position;

            }
            return randomPosition;
        }

        // 유효한 위치를 찾지 못한 경우
        return Vector3.zero;
    }
}
