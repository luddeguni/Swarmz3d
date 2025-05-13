using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MonsterSpawnInfo
{
    public GameObject monsterPrefab;
    public int amount;
}

[System.Serializable]
public class WavePreset
{
    public List<MonsterSpawnInfo> monstersToSpawn;
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Transform player;
    public float spawnRadius = 10f;
    public float timeBetweenWaves = 5f;
    public float spawnDelayBetweenMonsters = 0.3f;

    [Header("Wave Presets")]
    public List<WavePreset> waves;

    private List<GameObject> spawnedMonsters;

}
