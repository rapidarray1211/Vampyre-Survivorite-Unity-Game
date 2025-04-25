using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public Transform player;

    public float spawnRadius = 14f;
    public float minSpawnDistance = 11f;
    public float initialSpawnRate = 3f;
    public float minSpawnRate = 0.2f;
    public float spawnRateReduction = 0.3f;
    public float spawnRateReductionInterval = 15f;

    public float difficultyIncreaseRate = 10f;
    public float enemySpeedMultiplier = 0.5f;
    public float enemyHealthMultiplier = 0.1f;
    public float spawnIncreaseInterval = 30f;
    public int maxBaseSpawnCount = 10;

    private float timeSinceStart = 0f;
    private float difficultyMultiplier = 1f;
    private int baseSpawnCount = 1;
    private float spawnRate;
    private int listCount = 0;

    private void Start()
    {
        spawnRate = initialSpawnRate;
        StartCoroutine(SpawnEnemyLoop());
    }

    private IEnumerator SpawnEnemyLoop()
    {
        while (true)
        {
            Debug.Log("Spawning!");
            yield return new WaitForSeconds(spawnRate);
            SpawnEnemy();
        }
    }


    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        difficultyMultiplier = 1f + (timeSinceStart / difficultyIncreaseRate) * enemyHealthMultiplier;

        baseSpawnCount = Mathf.Min(1 + Mathf.FloorToInt(timeSinceStart / spawnIncreaseInterval), maxBaseSpawnCount);

        int spawnReductionSteps = (int)(timeSinceStart / spawnRateReductionInterval);
        if (spawnReductionSteps > (int)((timeSinceStart - Time.deltaTime) / spawnRateReductionInterval))
        {
            spawnRate -= spawnRateReduction;
            spawnRate = Mathf.Max(spawnRate, minSpawnRate);
        }

        int newWave = Mathf.FloorToInt(timeSinceStart / 60f);
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0) return;

        for (int i = 0; i < baseSpawnCount; i++)
        {
            Vector2 spawnPosition;
            do
            {
                Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
                spawnPosition = (Vector2)player.position + randomPosition;
            }
            while (Vector2.Distance(spawnPosition, player.position) < minSpawnDistance);
            Debug.Log("Spawning!");
            GameObject newEnemy = Instantiate(enemyPrefabs[listCount], spawnPosition, Quaternion.identity);
            listCount++;
            if (listCount > enemyPrefabs.Count - 1) {
                listCount = 0;
            }
            Enemy enemyScript = newEnemy.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.InitializeStats(difficultyMultiplier);
            }
        }
    }
}
