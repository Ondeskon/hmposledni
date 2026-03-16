using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;

    public void SetDirection(Vector2 dir)
    {
        // Make sure we have a reference to the Rigidbody2D
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rb != null)
        {
            // Normalize and apply — works left/right/up/down
            Vector2 normalizedDir = dir.normalized;
            rb.linearVelocity = normalizedDir * speed;

            Debug.Log("Bullet velocity: " + rb.linearVelocity);  // ← DEBUG: Check console
        }
        else
        {
            Debug.LogError("Bullet is missing Rigidbody2D component!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss")) // ← ADD "Boss"
        {
            // Existing enemy code...
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null) enemy.TakeDamage(20);

            // Boss code
            Boss boss = other.GetComponent<Boss>();
            if (boss != null) boss.TakeDamage(20);

            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}