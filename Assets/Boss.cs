using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHealth = 750;
    public int currentHealth;
    public float moveSpeed = 2f;

    [Header("Teleport")]
    public float teleportCooldown = 5f;
    public float minDistanceFromPlayer = 3f;
    public float maxDistanceFromPlayer = 5f;
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
    public float deathAnimationLength = 3.5f; // ← ZDE NASTAV PŘESNOU DÉLKU DEATH ANIMACE

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
        {
            animator = GetComponent<Animator>();
            if (animator == null) Debug.LogError("Animator chybí!");
        }

        animator.Play("BossIdle", -1, 0f);
        animator.SetFloat("Health", 1f);
    }

    public void StartBossAI()
    {
        if (hasSpawned) return;
        hasSpawned = true;
        Debug.Log("*** BOSS AI SPUNŠTĚNO! ***");
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
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += new Vector3(direction.x * moveSpeed * Time.deltaTime, 0, 0);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, platformMinX, platformMaxX),
            bossHeight,
            transform.position.z
        );

        float facing = direction.x > 0 ? 1 : -1;
        transform.localScale = new Vector3(facing * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    IEnumerator MeleeAttack()
    {
        Debug.Log("Boss začíná melee útok!");
        isAttacking = true;
        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger("Swing");

        // Detekce hned na začátku animace (když boss začíná švihat)
        yield return new WaitForSeconds(0.1f); // malý delay, aby se animace spustila

        PerformMeleeHit(); // první kontrola (začátek švihu)

        yield return new WaitForSeconds(0.4f); // zbytek animace

        PerformMeleeHit(); // druhá kontrola (konec švihu – největší síla)

        yield return new WaitForSeconds(0.3f); // malý follow-through

        TeleportBehindPlayerImmediate();

        isAttacking = false;
        Debug.Log("Melee útok dokončen – boss může pokračovat v pohybu");
    }

    // Nová pomocná metoda – vyčistí kód
    private void PerformMeleeHit()
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint na bossovi je NULL – útok selhal!");
            return;
        }

        Vector2 attackPos = attackPoint.position;
        float currentRange = meleeRange; // můžeš dynamicky měnit range během animace

        Debug.Log($"Melee hit check: Pos = {attackPos}, Player Pos = {player.position}, Distance = {Vector2.Distance(attackPos, player.position)}, Range = {currentRange}");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPos,
            currentRange,
            LayerMask.GetMask("Player")
        );

        Debug.Log($"Hits nalezeno: {hits.Length}");

        foreach (Collider2D hit in hits)
        {
            Debug.Log($"Zasažen: {hit.name} | Layer: {LayerMask.LayerToName(hit.gameObject.layer)}");

            PlayerController p = hit.GetComponent<PlayerController>();
            if (p != null)
            {
                p.TakeDamage(1);
                Debug.Log("*** BOSS ZASÁHL HRÁČE – 1 srdíčko ubráno! ***");
            }
            else
            {
                Debug.LogWarning("Hit na Player layeru, ale bez PlayerController!");
            }
        }
    }

    void TeleportBehindPlayerImmediate()
    {
        if (Time.time < lastTeleportTime + teleportCooldown) return;

        lastTeleportTime = Time.time;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        float offset = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
        float targetX = player.position.x + (player.localScale.x > 0 ? offset : -offset);
        targetX = Mathf.Clamp(targetX, platformMinX + 2f, platformMaxX - 2f);

        transform.position = new Vector3(targetX, bossHeight, transform.position.z);

        Vector2 direction = (player.position - transform.position).normalized;
        float facing = direction.x > 0 ? 1 : -1;
        transform.localScale = new Vector3(facing * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        Debug.Log($"Boss HP: {currentHealth}/{maxHealth}");

        if (animator != null)
            animator.SetFloat("Health", (float)currentHealth / maxHealth);

        BossHealthBar healthBar = FindFirstObjectByType<BossHealthBar>();
        if (healthBar != null) healthBar.UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Boss died!");

        if (animator != null)
        {
            animator.SetTrigger("Death");
            Debug.Log("Death trigger spuštěn");
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerName += " (VICTORY)";
            _ = GameManager.Instance.EndRunAsync(true);
            Debug.Log($"Boss zemřel – stats v GameManageru před spawnem screenu: enemiesKilled = {GameManager.Instance.enemiesKilled}, runTime = {GameManager.Instance.runTime:F1}s");
        }

        StartCoroutine(ShowWinningScreenAfterAnimation());
    }

    private IEnumerator ShowWinningScreenAfterAnimation()
    {
        Debug.Log($"Čekám {deathAnimationLength} sekund na dokončení death animace");
        yield return new WaitForSeconds(deathAnimationLength);

        if (animator != null)
        {
            animator.enabled = false;
            Debug.Log("Animator vypnutý");
        }

        if (winningScreenPrefab == null)
        {
            Debug.LogError("WinningScreenPrefab není nastavený!");
            yield break;
        }

        GameObject victory = Instantiate(winningScreenPrefab);
        victory.SetActive(true);
        Debug.Log("Winning Screen spawned!");

        // Malý delay, aby se data stihla aktualizovat (Unity někdy potřebuje frame)
        yield return null;

        VictoryScreen victoryScript = victory.GetComponent<VictoryScreen>();
        if (victoryScript != null)
        {
            victoryScript.UpdateStats();
            Debug.Log("VictoryScreen stats aktualizovány!");
        }
        else
        {
            Debug.LogWarning("VictoryScreen script chybí na prefabu winning screenu!");
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