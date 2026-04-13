using UnityEngine;

public class Stalker : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.5f;

    [Header("Attack")]
    public int damage = 15;
    public float attackCooldown = 1.0f;

    [Header("Health")]
    public int maxHealth = 3;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 initialScale;
    private float lastAttackTime = -100f;
    private int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        initialScale = transform.localScale;
        currentHealth = maxHealth;

        // Anti-sinking & spawn fixes
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.linearVelocity = Vector2.zero;

        // Force correct size
        transform.localScale = new Vector3(1f, 1f, 1f);

        // Lift up when spawning so it doesn't rise from ground
        transform.position += new Vector3(0, 1.2f, 0);

        if (GetComponent<Collider2D>() == null)
            Debug.LogError("Stalker is missing a Collider2D!");
    }

    void Update()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > 1.6f)
        {
            rb.linearVelocity = direction * moveSpeed;

            // FIXED FLIPPING - This should finally work correctly
            float flip = (direction.x > 0) ? 1f : -1f;
            transform.localScale = new Vector3(flip * Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Animation
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }

        // Attack when close
        if (distance <= 1.8f && Time.time > lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        lastAttackTime = Time.time;
        PlayerController p = player.GetComponent<PlayerController>();
        if (p != null)
        {
            p.TakeDamage(damage);
            Debug.Log("Stalker attacked the player!");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Stalker took {damageAmount} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Stalker has been killed!");
        Destroy(gameObject);
    }
}