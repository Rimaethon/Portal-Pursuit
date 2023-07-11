using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool spawnAtStart;
    public GameObject prefab;

    private void Start()
    {
        Debug.Log("Press Space to spawn cubes");
        if (spawnAtStart) Spawn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Spawn();
    }

    private void Spawn()
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }
}