using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject zombiePrefab;     // Your zombie prefab
    public float spawnInterval = 10f;   // Every 10 seconds
    public Vector2 spawnOffset = new Vector2(0, -0.5f);  // Below spawner
    public float moveSpeed = 2f;        // Speed moving LEFT towards player

    void Start()
    {
        StartCoroutine(SpawnZombies());
        Debug.Log("Spawner started at right side: " + transform.position);
    }

    void Update()
    {
        // Move LEFT towards player
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        Debug.Log("Spawner moving LEFT! X: " + transform.position.x);
    }

    IEnumerator SpawnZombies()
    {
        while (true)
        {
            if (zombiePrefab != null)
            {
                Instantiate(zombiePrefab, (Vector2)transform.position + spawnOffset, Quaternion.identity);
                Debug.Log("Zombie spawned from right!");
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}