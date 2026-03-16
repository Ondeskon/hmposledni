using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // pro SceneManager.LoadScene

public class PlayerController : MonoBehaviour
{
    Animator animator;
    public float speed = 5f;
    public float jumpForce = 10f;
    public GameObject bulletPrefab;
    public Transform gunTip;
    public float fireRate = 0.5f;
    public int maxHealth = 3;
    public int currentHealth = 3;
    // RAVEN
    public GameObject ravenPrefab;
    private RavenAttack raven;
    private float ravenCooldown = 0f;
    // DAMAGE FLASH
    public GameObject damageFlashImage;
    // MELEE ATTACK
    public Transform attackPoint;
    // YOU DIED
    public GameObject youDiedPrefab;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float nextFireTime;
    private Vector3 initialScale;
    public GameObject[] bloodPrefabs;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
        nextFireTime = 0;

        // ← KLÍČOVÁ ZMĚNA: Načti HP z GameManageru (přenáší se mezi levely)
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.playerCurrentHealth;
            Debug.Log($"HP načteno z GameManageru při startu levelu: {currentHealth}/{maxHealth}");
        }
        else
        {
            currentHealth = maxHealth;
            Debug.LogWarning("GameManager.Instance je NULL – HP resetováno na max!");
        }

        animator = GetComponent<Animator>();
        animator.ResetTrigger("Slash");

        // Spawn Raven
        if (ravenPrefab != null)
        {
            GameObject ravenObj = Instantiate(ravenPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            raven = ravenObj.GetComponent<RavenAttack>();
            if (raven != null)
            {
                raven.SetPlayer(transform);
                Debug.Log("Raven spawned successfully");
            }
        }

        // Aktualizuj srdíčka hned na začátku
        PlayerHearts heartsUI = FindFirstObjectByType<PlayerHearts>();
        if (heartsUI != null)
            heartsUI.OnPlayerDamage();
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // Flip player body based on mouse aim
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        if (mousePos.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Shoot
        if (Input.GetMouseButton(0) && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Animation
        float speedAnim = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        animator.SetFloat("Speed", speedAnim);

        // Test damage (stisknutím T ubere 1 srdíčko)
        if (Input.GetKeyDown(KeyCode.T)) TakeDamage(1);

        // Raven attack
        if (Input.GetKeyDown(KeyCode.Q) && Time.time > ravenCooldown && raven != null)
        {
            raven.AttackNearestEnemy();
            ravenCooldown = Time.time + 3f;
        }

        // Melee attack
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Slash");
            MeleeAttack(); // Volám melee attack hned (nebo přes Animation Event)
        }
    }

    void MeleeAttack()
    {
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint is NULL – melee nefunguje!");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, 1.5f, LayerMask.GetMask("Enemy"));
        Debug.Log("Melee hit count: " + hitEnemies.Length); // DEBUG – musí být >0

        foreach (Collider2D enemy in hitEnemies)
        {
            // Běžní nepřátelé
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.TakeDamage(50);
                Debug.Log("Melee hit normálního nepřítele!");
                continue;
            }

            // BOSS – nové!
            Boss boss = enemy.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(50);
                Debug.Log("*** MAČETA ZASÁHLA BOSSE – 50 damage! ***");
                continue;
            }

            Debug.Log("Hit něco na layeru Enemy, ale bez scriptu: " + enemy.name);
        }
    }

    void Shoot()
    {
        if (gunTip == null || bulletPrefab == null) return;

        Vector2 shootDir = gunTip.right;

        GameObject bullet = Instantiate(bulletPrefab, gunTip.position, Quaternion.identity);
        BulletController bulletCtrl = bullet.GetComponent<BulletController>();
        if (bulletCtrl != null)
        {
            bulletCtrl.SetDirection(shootDir);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1); // 1 srdíčko za dotek
            Destroy(collision.gameObject);
            SpawnBlood(collision.transform.position);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    public void TakeDamage(int heartsToLose = 1) // defaultně 1 srdíčko
    {
        currentHealth -= heartsToLose;
        currentHealth = Mathf.Max(currentHealth, 0); // nesmí jít pod 0

        Debug.Log($"Player hit! Ubráno {heartsToLose} srdíček, zbývá: {currentHealth}/{maxHealth}");

        // ← KLÍČOVÁ ZMĚNA: Ulož aktuální HP do GameManageru (přenáší se do dalšího levelu)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerCurrentHealth = currentHealth;
            Debug.Log("HP uloženo do GameManageru pro další levely: " + currentHealth);
        }

        // Aktualizuj srdíčka UI
        PlayerHearts heartsUI = FindFirstObjectByType<PlayerHearts>();
        if (heartsUI != null)
            heartsUI.OnPlayerDamage();

        // Camera shake
        if (CameraShake.instance != null)
            StartCoroutine(CameraShake.instance.Shake(0.2f, 0.4f));

        // Damage flash
        if (damageFlashImage != null)
            StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void SpawnBlood(Vector3 pos)
    {
        if (bloodPrefabs.Length == 0) return;

        GameObject blood = Instantiate(
            bloodPrefabs[Random.Range(0, bloodPrefabs.Length)],
            pos + (Vector3)Random.insideUnitCircle * 0.4f,
            Quaternion.Euler(0, 0, Random.Range(0, 360))
        );
        Destroy(blood, 3f);
    }

    IEnumerator DamageFlash()
    {
        CanvasGroup cg = damageFlashImage.GetComponent<CanvasGroup>();
        if (cg == null) cg = damageFlashImage.AddComponent<CanvasGroup>();

        cg.alpha = 0.6f;
        yield return new WaitForSeconds(0.12f);
        cg.alpha = 0f;
    }
    void Die()
    {
        Debug.Log("========== PLAYER DIED ==========");

        // Freeze hráče
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (animator != null)
            animator.enabled = false;

        // ← ODEBRÁNO: Neukládáme runu při smrti – jen při zabití bosse
        // if (GameManager.Instance != null)
        // {
        //     StartCoroutine(EndRunCoroutine());
        // }

        // Spawn You Died screen
        if (youDiedPrefab != null)
        {
            Instantiate(youDiedPrefab);
            Debug.Log("YouDied prefab spawned!");
        }
        else
        {
            Debug.LogError("youDiedPrefab is NULL!");
        }

        // Po 3 sekundách vrať do Main Menu
        StartCoroutine(GoToMainMenuAfterDeath(3f));

        enabled = false;
    }
    // ← JEDINÁ verze metody EndRunCoroutine (smazal jsem duplicitu)
    private IEnumerator EndRunCoroutine()
    {
        yield return null; // malý delay pro jistotu
        yield return GameManager.Instance.EndRunAsync(); // await jako yield return Task
        Debug.Log("EndRunAsync dokončeno!");
    }

    private IEnumerator GoToMainMenuAfterDeath(float delay)
    {
        yield return new WaitForSeconds(delay); // 3 sekundy You Died screen

        // Reset stats pro novou runu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewRun(); // reset HP, enemiesKilled, runTime atd.
            Debug.Log("Stats resetovány pro novou runu");
        }

        // Load Main Menu scény (ZMĚŇ NA TVŮJ NÁZEV SCÉNY – např. "MainMenu", "Menu", "StartScene")
        SceneManager.LoadScene("MainMenu");
    }
}