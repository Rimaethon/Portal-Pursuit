using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnMethod
    {
        FIXED,
        RANDOM
    }

    public GameObject prefab;
    public SpawnMethod spawnMethod = SpawnMethod.FIXED;
    public float spawnRate = 5.0f;
    public Vector3 spawnAreaSize = Vector3.zero;
    public bool showSpawnArea = true;
    private float nextSpawnTime = Mathf.NegativeInfinity;

    private void Update()
    {
        TestSpawn();
    }

    private void OnDrawGizmos()
    {
        if (showSpawnArea)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, spawnAreaSize);
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(transform.position, spawnAreaSize);
        }
    }

    private void TestSpawn()
    {
        if (Time.timeSinceLevelLoad > nextSpawnTime) Spawn();
    }

    private void Spawn()
    {
        if (prefab == null) return;

        nextSpawnTime = spawnMethod switch
        {
            SpawnMethod.FIXED  => Time.timeSinceLevelLoad + spawnRate,
            SpawnMethod.RANDOM => Time.timeSinceLevelLoad + spawnRate * Random.value,
            _                  => throw new ArgumentOutOfRangeException()
        };

        Vector3 spawnLocation = GetSpawnLocation();
        GameObject instance = Instantiate(prefab, spawnLocation, Quaternion.identity, null);
    }

    private Vector3 GetSpawnLocation()
    {
        Vector3 result = Vector3.zero;
        result.x = transform.position.x + Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f);
        result.y = transform.position.y + Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f);
        result.z = transform.position.z + Random.Range(-spawnAreaSize.z * 0.5f, spawnAreaSize.z * 0.5f);
        return result;
    }
}
