using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Enemies")]
    public GameObject zombiePrefab;     // Normal walker
    public GameObject stalkerPrefab;    // Fast fragile enemy
    public GameObject batPrefab;        // Your flying bats

    [Header("Spawn Settings")]
    public float spawnInterval = 10f;
    public Vector2 spawnOffset = new Vector2(0, 1.0f);   // ← Changed from -0.5f to 1.0f
    public float moveSpeed = 2f;

    [Header("Random Chances")]
    [Range(0f, 1f)] public float stalkerChance = 0.25f;   // 25% Stalker
    [Range(0f, 1f)] public float batChance = 0.20f;       // 20% Bat

    void Start()
    {
        StartCoroutine(SpawnZombies());
        Debug.Log("Spawner started at: " + transform.position);
    }

    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    IEnumerator SpawnZombies()
    {
        while (true)
        {
            float roll = Random.value;   // random number between 0 and 1

            GameObject toSpawn = zombiePrefab;

            if (batPrefab != null && roll < batChance)
            {
                toSpawn = batPrefab;
                Debug.Log("Bat spawned!");
            }
            else if (stalkerPrefab != null && roll < batChance + stalkerChance)
            {
                toSpawn = stalkerPrefab;
                Debug.Log("Stalker spawned!");
            }
            else if (zombiePrefab != null)
            {
                Debug.Log("Zombie spawned!");
            }

            if (toSpawn != null)
            {
                Instantiate(toSpawn, (Vector2)transform.position + spawnOffset, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Spawner: No prefab assigned!");
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}