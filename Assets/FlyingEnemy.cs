using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float hoverSpeed = 2f;
    public float diveSpeed = 8f;
    public float damage = 10f;
    public float attackCooldown = 2f;
    public float diveDistance = 4f;
    public float hoverHeight = 5f;

    [Header("Health Settings")]
    public int health = 30; // Počet životů
    private int maxHealth;
    public HealthBarUI healthBar; // Referenci přetáhneš v Inspektoru

    private Transform player;
    private float lastAttackTime;
    private bool isDiving = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("FlyingEnemy: No Player found!");

        // Inicializace healthbaru
        maxHealth = health;
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < diveDistance && Time.time > lastAttackTime + attackCooldown && !isDiving)
        {
            isDiving = true;
            lastAttackTime = Time.time;
        }

        if (isDiving)
        {
            Vector2 diveDir = (player.position - transform.position).normalized;
            transform.position += (Vector3)diveDir * diveSpeed * Time.deltaTime;

            if (distanceToPlayer < 0.5f)
            {
                isDiving = false;
            }
        }
        else
        {
            Vector2 hoverDir = (player.position - transform.position).normalized;
            transform.position += new Vector3(hoverDir.x * hoverSpeed * Time.deltaTime, 0, 0);
            transform.position = new Vector3(transform.position.x, hoverHeight, 0);
        }

        // Face player - Tady pozor, u tvého kódu byla sčítání, opravil jsem na porovnání pozic
        float direction = player.position.x > transform.position.x ? 1f : -1f;
        transform.localScale = new Vector3(direction > 0 ? 1 : -1, 1, 1);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerCtrl = collision.gameObject.GetComponent<PlayerController>();
            if (playerCtrl != null) playerCtrl.TakeDamage((int)damage);

            // Netopýr při nárazu do hráče zmizí (sebevražedný útok)
            Die();
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= (int)dmg;

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Pokud chceš, aby i létající nepřátelé přidávali body do GameManageru:
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyKilled();
        }

        Destroy(gameObject);
    }
}