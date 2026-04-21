using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHealth = 750;
    public int currentHealth;
    public float moveSpeed = 2f;

    [Header("Teleport")]
    public float teleportCooldown = 4f;
    public float minDistanceFromPlayer = 3f;
    public float maxDistanceFromPlayer = 6f;
    public float platformMinX = -12f;
    public float platformMaxX = 28f;
    public float bossHeight = 2f;

    [Header("Melee Attack")]
    public int meleeDamage = 30;
    public float meleeRange = 5f;
    public float meleeCooldown = 3f;
    public Transform attackPoint;

    [Header("Animation")]
    public Animator animator;

    [Header("Victory Screen")]
    public GameObject winningScreenPrefab;

    [Header("Death Animation Timing")]
    public float deathAnimationLength = 3.5f;

    private Transform player;
    private float lastTeleportTime;
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool hasSpawned = false;
    public bool isDead = false;
    private Vector3 originalScale;

    void Awake()
    {
        currentHealth = maxHealth;
        originalScale = transform.localScale;
        lastTeleportTime = -teleportCooldown;
        lastAttackTime = -meleeCooldown;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("Boss: Player not found!");

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.Play("BossIdle", -1, 0f);
        animator.SetFloat("Health", 1f);

        // --- POJISTKA: Nastav bar na 100% hned po startu ---
        UpdateUI();

        // Pokud nemáš jiný skript (Intro), co spouští bosse, odkomentuj tenhle řádek:
        // StartBossAI(); 
    }

    public void StartBossAI()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        // Spustíme proces s čekáním (změň 3.0f na délku tvého zoomu)
        StartCoroutine(DelayedUIAppearance(0f));

        Debug.Log("*** BOSS AI STARTED! ***");
    }

    private IEnumerator DelayedUIAppearance(float delay)
    {
        // Najde HealthBar, i když je v Inspektoru vypnutý (neaktivní)
        BossHealthBar healthBar = FindFirstObjectByType<BossHealthBar>(FindObjectsInactive.Include);

        if (healthBar != null)
        {
            // Počkáme na konec intra
            yield return new WaitForSeconds(delay);

            // Zapneme UI objekt
            healthBar.gameObject.SetActive(true);

            // Nastavíme správnou hodnotu hned při zapnutí
            healthBar.UpdateHealthBar(currentHealth, maxHealth);

            Debug.Log("Boss UI se aktivovalo se zpožděním!");
        }
        else
        {
            Debug.LogError("BossHealthBar nebyl ve scéně nalezen!");
        }
    }

    // Pomocná metoda, ať nemusíš psát ten Find pokaždé
    private void UpdateUI()
    {
        BossHealthBar healthBar = FindFirstObjectByType<BossHealthBar>();
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (!hasSpawned || player == null || isAttacking || isDead) return;

        MoveTowardsPlayer();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= meleeRange && Time.time > lastAttackTime + meleeCooldown)
        {
            StartCoroutine(MeleeAttack());
        }
    }

    void MoveTowardsPlayer()
    {
        if (player == null || isAttacking || isDead) return;

        Vector2 direction = (player.position - transform.position).normalized;

        // Normal movement without hard clamping
        transform.position += new Vector3(direction.x * moveSpeed * Time.deltaTime, 0, 0);

        // Very soft safety correction - only if he's clearly outside the platform
        float currentX = transform.position.x;

        if (currentX < platformMinX + 0.5f)
        {
            transform.position = new Vector3(platformMinX + 2f, bossHeight, transform.position.z);
        }
        else if (currentX > platformMaxX - 0.5f)
        {
            transform.position = new Vector3(platformMaxX - 2f, bossHeight, transform.position.z);
        }

        // Face the player
        float facing = direction.x > 0 ? 1f : -1f;
        transform.localScale = new Vector3(facing * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    IEnumerator MeleeAttack()
    {
        Debug.Log("Boss začíná melee útok!");
        isAttacking = true;
        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger("Swing");

        // Wait a bit for swing animation to start
        yield return new WaitForSeconds(0.35f);

        // Perform the actual hit
        PerformMeleeHit();

        // After attack → teleport behind player
        yield return new WaitForSeconds(0.4f);
        TeleportBehindPlayer();

        isAttacking = false;
        Debug.Log("Melee attack finished - boss can move again");
    }

    private void PerformMeleeHit()
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint is NULL!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, meleeRange, LayerMask.GetMask("Player"));

        foreach (Collider2D hit in hits)
        {
            PlayerController p = hit.GetComponent<PlayerController>();
            if (p != null)
            {
                p.TakeDamage(meleeDamage);
                Debug.Log("Boss hit the player with melee!");
            }
        }
    }
    private void TeleportBehindPlayer()
    {
        if (Time.time < lastTeleportTime + teleportCooldown) return;

        lastTeleportTime = Time.time;

        float offset = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
        float targetX = player.position.x + (player.localScale.x > 0 ? -offset : offset);

        // Very soft clamping
        targetX = Mathf.Clamp(targetX, platformMinX + 2.5f, platformMaxX - 2.5f);

        transform.position = new Vector3(targetX, bossHeight, transform.position.z);

        // Force correct facing
        Vector2 direction = (player.position - transform.position).normalized;
        float facing = direction.x > 0 ? 1f : -1f;
        transform.localScale = new Vector3(facing * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        Debug.Log($"Boss teleported behind player at X = {targetX}");
    }
    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        Debug.Log($"Boss took {dmg} damage. HP left: {currentHealth}/{maxHealth}");

        // Safe way to trigger "Hurt" animation
        if (animator != null)
        {
            animator.SetTrigger("Hurt");     // This is fine even if the trigger doesn't exist (Unity just ignores it)
        }

        // Update health bar
        BossHealthBar healthBar = FindFirstObjectByType<BossHealthBar>();
        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth); // Teď mu posíláš ty informace

        UpdateUI();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Boss died!");
        if (animator != null) animator.SetTrigger("Death");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        this.enabled = false;

        StartCoroutine(ShowWinningScreenAfterAnimation());
    }

    private IEnumerator ShowWinningScreenAfterAnimation()
    {
        yield return new WaitForSeconds(deathAnimationLength);

        if (winningScreenPrefab != null)
        {
            Instantiate(winningScreenPrefab);
            Debug.Log("Winning Screen spawned!");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, meleeRange);
        }
    }
}