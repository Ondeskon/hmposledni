using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float hoverSpeed = 2f;
    public float diveSpeed = 8f;
    public float damage = 10f;
    public float attackCooldown = 2f; // ← SHORTER cooldown for testing
    public float diveDistance = 4f; // ← BIGGER range to trigger dive easier
    public float hoverHeight = 5f;

    private Transform player;
    private float lastAttackTime;
    private bool isDiving = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("FlyingEnemy: No Player found!");
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Debug.Log("Bat distance to player: " + distanceToPlayer + " | DiveDistance: " + diveDistance); // ← DEBUG

        if (distanceToPlayer < diveDistance && Time.time > lastAttackTime + attackCooldown && !isDiving)
        {
            Debug.Log("BAT DIVING!"); // ← DEBUG
            isDiving = true;
            lastAttackTime = Time.time;
        }

        if (isDiving)
        {
            // Full dive toward player
            Vector2 diveDir = (player.position - transform.position).normalized;
            transform.position += (Vector3)diveDir * diveSpeed * Time.deltaTime;

            // End dive if very close or missed
            if (distanceToPlayer < 0.5f)
            {
                isDiving = false;
                Debug.Log("Dive complete!");
            }
        }
        else
        {
            // Hover/chase
            Vector2 hoverDir = (player.position - transform.position).normalized;
            transform.position += new Vector3(hoverDir.x * hoverSpeed * Time.deltaTime, 0, 0);

            // Lock hover height
            transform.position = new Vector3(transform.position.x, hoverHeight, 0);
        }

        // Face player
        Vector2 dir = player.position + transform.position;
        transform.localScale = new Vector3(dir.x > 0 ? 1 : -1, 1, 1);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("BAT HIT PLAYER!");
            var playerCtrl = collision.gameObject.GetComponent<PlayerController>();
            if (playerCtrl != null) playerCtrl.TakeDamage((int)damage);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float dmg)
    {
        Destroy(gameObject);
    }
}