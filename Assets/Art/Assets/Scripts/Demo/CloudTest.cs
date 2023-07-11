using UnityEngine;

public class CloudTest : MonoBehaviour
{
    public int numViewDirections = 100;
    public int numClouds = 10;
    public int cloudSpawnSeed;
    public bool randomizeCloudSeed;

    public float spawnRadius = 10;

    [Range(0, 1)] public float startHeight;

    public GameObject cloudPrefab;
    public GameObject cloudCorePrefab;

    private void Start()
    {
        var goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        var angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (var i = 0; i < numViewDirections; i++)
        {
            var t = (float)i / numViewDirections;
            var inclination = Mathf.Acos(1 - (1 - startHeight) * t);
            var azimuth = angleIncrement * i;

            var x = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            var y = Mathf.Cos(inclination);
            var z = Mathf.Sin(inclination) * Mathf.Cos(azimuth);

            var g = Instantiate(cloudPrefab, transform.position + new Vector3(x, y, z) * spawnRadius,
                Quaternion.identity, transform);
        }

        if (randomizeCloudSeed) cloudSpawnSeed = Random.Range(-10000, 10000);
        var prng = new System.Random(cloudSpawnSeed);

        for (var i = 0; i < numClouds; i++)
        {
            var t = (float)prng.NextDouble();
            var inclination = Mathf.Acos(1 - (1 - startHeight) * t);
            var azimuth = angleIncrement * i;

            var x = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            var y = Mathf.Cos(inclination);
            var z = Mathf.Sin(inclination) * Mathf.Cos(azimuth);

            var g = Instantiate(cloudCorePrefab, transform.position + new Vector3(x, y, z) * spawnRadius,
                Quaternion.identity, transform);
        }
    }
}