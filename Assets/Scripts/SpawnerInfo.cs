using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewSpawner", menuName = "Spawner/Spawner Data")]
public class SpawnerInfo : ScriptableObject
{
    public List<GameObject> SpawnPrefabs;
    public List<int> SpawnOnces;
    public Vector2 spawnAreaMin; // 스폰 영역 최소값 (X, Z)
    public Vector2 spawnAreaMax; // 스폰 영역 최대값 (X, Z)
    public float spawnRate = 0.5f; // 초당 생성할 몬스터 수
    public float spawnInterval = 20; // 다음 몬스터 그룹으로 넘어가는 시간
    public float spawnRadius = 5f; // NavMesh.SamplePosition 시 허용 반경
    public float InitialSpawnDelay = 0f; // 첫 몬스터 스폰 전에 기다리는 시간
}