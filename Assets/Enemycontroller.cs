using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public int health = 3;
    private int maxHealth; // Přidáno pro healthbar
    public int damage = 10;
    public float damageCooldown = 1f;

    [Header("UI")]
    public HealthBarUI healthBar; // Přetáhni sem skript z Canvasu

    private float lastDamageTime;
    private Transform player;
    private Rigidbody2D rb;
    private Vector3 initialScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        initialScale = transform.localScale;
        lastDamageTime = -damageCooldown;

        // Nastavíme maxHealth na začátku
        maxHealth = health;
        if (healthBar != null) healthBar.UpdateHealthBar(health, maxHealth);

        if (player == null)
            Debug.LogWarning("Player not found! Tag the player as 'Player'.");
    }

    void Update()
    {
        if (player == null) return;

        float direction = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // Tvůj původní flip sprite
        transform.localScale = new Vector3(
            direction > 0 ? -Mathf.Abs(initialScale.x) : Mathf.Abs(initialScale.x),
            initialScale.y,
            initialScale.z
        );
    }

    public void TakeDamage(int damageTaken) // Přejmenoval jsem parametr, aby se nepletl s public int damage
    {
        health -= damageTaken;

        // AKTUALIZACE HEALTHBARU
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }

        Debug.Log($"Enemy hit! Damage taken: {damageTaken}, Health left: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyKilled();
            Debug.Log($"Enemy killed! enemiesKilled increased to: {GameManager.Instance.enemiesKilled}");
        }
        else
        {
            Debug.LogWarning("GameManager.Instance je null při smrti nepřítele!");
        }

        Debug.Log("Enemy destroyed!");
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time > lastDamageTime + damageCooldown)
        {
            lastDamageTime = Time.time;

            PlayerController playerCtrl = collision.gameObject.GetComponent<PlayerController>();
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(damage);
                Debug.Log($"Enemy dealt {damage} damage to player (cooldown respected)");
            }
        }
    }
}