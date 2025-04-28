using UnityEngine;
using UnityEngine.UI;
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

    [Header("Terrain Settings")]
    public Terrain terrain;

    [Header("Wave UI Settings")]
    public Font waveTextFont;
    public int waveTextFontSize = 40;
    public Color waveTextColor = Color.white;
    public float waveTextFadeDuration = 1f;
    public float waveTextStayDuration = 2f;

    [Header("Wave Presets")]
    public List<WavePreset> waves;

    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private int currentWaveIndex = 0;
    private float waveTimer = 0f;
    private bool waveActive = false;
    private bool waitingNextWave = false;

    private Canvas waveCanvas;
    private Text waveText;

    private void Start()
    {
        CreateWaveTextUI();
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (waveActive)
        {
            // Clean destroyed monsters
            spawnedMonsters.RemoveAll(monster => monster == null);

            if (spawnedMonsters.Count == 0)
            {
                waveActive = false;
                waitingNextWave = true;
                waveTimer = timeBetweenWaves;
            }
        }
        else if (waitingNextWave)
        {
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                waitingNextWave = false;
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

        ShowWaveText($"Wave {currentWaveIndex + 1}");

        spawnedMonsters.Clear(); // Clear previous list

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

        yield return null; // Wait 1 frame to ensure monsters are spawned

        waveActive = true; // Only set active after spawning
        currentWaveIndex++;
    }

    private Vector3 GetRandomPositionAroundPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        float spawnX = player.position.x + randomCircle.x;
        float spawnZ = player.position.z + randomCircle.y;

        // Clamp within terrain bounds
        Vector3 terrainPosition = terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;

        float minX = terrainPosition.x;
        float maxX = terrainPosition.x + terrainData.size.x;
        float minZ = terrainPosition.z;
        float maxZ = terrainPosition.z + terrainData.size.z;

        spawnX = Mathf.Clamp(spawnX, minX, maxX);
        spawnZ = Mathf.Clamp(spawnZ, minZ, maxZ);

        float spawnY = terrain.SampleHeight(new Vector3(spawnX, 0, spawnZ)) + terrainPosition.y;

        return new Vector3(spawnX, spawnY, spawnZ);
    }

    private void CreateWaveTextUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("WaveCanvas");
        waveCanvas = canvasGO.AddComponent<Canvas>();
        waveCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create Text
        GameObject textGO = new GameObject("WaveText");
        textGO.transform.SetParent(canvasGO.transform);
        waveText = textGO.AddComponent<Text>();

        RectTransform rectTransform = waveText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(600, 200);
        rectTransform.localPosition = Vector3.zero;

        waveText.alignment = TextAnchor.MiddleCenter;
        waveText.fontSize = waveTextFontSize;
        waveText.color = new Color(waveTextColor.r, waveTextColor.g, waveTextColor.b, 0); // Start invisible

        if (waveTextFont != null)
            waveText.font = waveTextFont;
        else
            waveText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private void ShowWaveText(string message)
    {
        waveText.text = message;
        StartCoroutine(FadeWaveText());
    }

    private IEnumerator FadeWaveText()
    {
        // Fade In
        float timer = 0f;
        while (timer < waveTextFadeDuration)
        {
            float alpha = timer / waveTextFadeDuration;
            waveText.color = new Color(waveTextColor.r, waveTextColor.g, waveTextColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        waveText.color = new Color(waveTextColor.r, waveTextColor.g, waveTextColor.b, 1f);

        // Stay
        yield return new WaitForSeconds(waveTextStayDuration);

        // Fade Out
        timer = 0f;
        while (timer < waveTextFadeDuration)
        {
            float alpha = 1f - (timer / waveTextFadeDuration);
            waveText.color = new Color(waveTextColor.r, waveTextColor.g, waveTextColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        waveText.color = new Color(waveTextColor.r, waveTextColor.g, waveTextColor.b, 0);
    }
}
