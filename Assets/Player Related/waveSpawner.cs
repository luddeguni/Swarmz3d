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

    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private int currentWaveIndex = 0;
    private float waveTimer = 0f;
    private bool waveActive = false;
    private bool isSpawningWave = false;

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (waveActive)
        {
            // Clean up dead monsters
            spawnedMonsters.RemoveAll(monster => monster == null);

            if (spawnedMonsters.Count == 0)
            {
                waveActive = false;
                waveTimer = timeBetweenWaves;
            }
        }
        else if (!isSpawningWave)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                isSpawningWave = true;
                StartCoroutine(StartWave());
            }
        }
    }

    private IEnumerator StartWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("All waves completed!");
            yield break;
        }

        WavePreset currentWave = waves[currentWaveIndex];
        Debug.Log($"Starting Wave {currentWaveIndex + 1}");

        spawnedMonsters.Clear();

        foreach (var spawnInfo in currentWave.monstersToSpawn)
        {
            for (int i = 0; i < spawnInfo.amount; i++)
            {
                Vector3 spawnPos = GetRandomPositionAroundPlayer();
                GameObject monster = Instantiate(spawnInfo.monsterPrefab, spawnPos, Quaternion.identity);
                spawnedMonsters.Add(monster);

                yield return new WaitForSeconds(spawnDelayBetweenMonsters);
            }
        }

        currentWaveIndex++;
        waveActive = true;
        isSpawningWave = false;
    }

    private Vector3 GetRandomPositionAroundPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(
            player.position.x + randomCircle.x,
            player.position.y,
            player.position.z + randomCircle.y
        );
    }
}
