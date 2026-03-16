using UnityEngine;
using System.Collections;

public class RavenAttack : MonoBehaviour
{
    [Header("Settings")]
    public float idleHoverDistance = 2.5f;
    public float diveSpeed = 15f;
    public float returnSpeed = 12f;
    public int damage = 25;
    public float attackRange = 10f;

    private Transform player;
    private bool isAttacking = false;

    void Start()
    {
        // No CircleCollider2D requirement - simply start hovering
        StartCoroutine(HoverAroundPlayer());
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    void Update()
    {
        if (player == null) return;

        // Face direction toward player
        Vector2 toPlayer = (player.position - transform.position).normalized;
        transform.localScale = new Vector3(toPlayer.x > 0 ? 1 : -1, 1, 1);
    }

    public void AttackNearestEnemy()
    {
        if (isAttacking || player == null) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float closestDistance = attackRange;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            StartCoroutine(DiveAttack(nearestEnemy.transform));
        }
        else
        {
            Debug.Log("No enemies in range!");
        }
    }

    IEnumerator HoverAroundPlayer()
    {
        while (true)
        {
            if (player != null && !isAttacking)
            {
                Vector3 hoverPos = player.position + Vector3.up * idleHoverDistance;
                transform.position = Vector3.MoveTowards(transform.position, hoverPos, 3f * Time.deltaTime);
            }
            yield return null;
        }
    }

    IEnumerator DiveAttack(Transform enemy)
    {
        isAttacking = true;

        // Dive to enemy
        Vector3 startPos = transform.position;
        float diveTime = 0;
        while (diveTime < 1f)
        {
            transform.position = Vector3.Lerp(startPos, enemy.position, diveTime);
            diveTime += Time.deltaTime * diveSpeed;
            yield return null;
        }
        transform.position = enemy.position;

        // Check for collision manually since we're not using trigger colliders
        Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
        if (enemyCollider != null)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(damage);
                Debug.Log($"Raven dealt {damage} damage!");
            }
        }

        // Return to player
        float returnTime = 0;
        Vector3 returnTarget = player.position + Vector3.up * idleHoverDistance;
        while (returnTime < 1f)
        {
            transform.position = Vector3.Lerp(transform.position, returnTarget, returnTime);
            returnTime += Time.deltaTime * returnSpeed;
            yield return null;
        }

        isAttacking = false;
    }
}