using UnityEngine;
using System.Collections;

public class BossIntro : MonoBehaviour
{
    [Header("References")]
    public GameObject bossObject;              // Drag Boss prefab/instance (inactive!)
    public Transform bossSpawnPosition;        // Pozice spawnutí
    public Camera mainCamera;                  // Main Camera
    public Transform playerTransform;          // Jason player

    [Header("Camera Settings")]
    public float cameraMoveDuration = 1.5f;    // Doba pohybu kamery k bossovi
    public float watchSpawnDuration = 2.5f;    // Jak dlouho kamera sleduje spawn

    private Vector3 originalCameraPosition;
    private Vector3 originalCameraOffset;
    private bool introPlayed = false;

    void Awake()
    {
        // Zajišťujeme, že boss je na startu vypnutý
        if (bossObject != null)
            bossObject.SetActive(false);
    }

    void Start()
    {
        if (introPlayed) return;

        if (bossObject == null || bossSpawnPosition == null || mainCamera == null || playerTransform == null)
        {
            Debug.LogError("BossIntro: Chybí nějaká reference!");
            return;
        }

        StartCoroutine(PlayBossIntro());
    }

    IEnumerator PlayBossIntro()
    {
        introPlayed = true;

        // 1. Vypnutí ovládání hráče
        PlayerController player = playerTransform.GetComponent<PlayerController>();
        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (player != null) player.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // 2. Uložení původní kamery (před odpojením)
        originalCameraPosition = mainCamera.transform.position;
        originalCameraOffset = mainCamera.transform.localPosition; // offset relativní k hráči
        mainCamera.transform.SetParent(null); // Odpojíme kameru

        // 3. Spawn bosse – jen jednou
        if (bossObject != null)
        {
            bossObject.transform.position = bossSpawnPosition.position;
            bossObject.SetActive(true);

            Boss bossScript = bossObject.GetComponent<Boss>();
            Animator bossAnimator = bossObject.GetComponent<Animator>();

            if (bossScript != null)
                bossScript.enabled = false; // AI vypnuto do konce intro

            // 4. Pohyb kamery k bossovi
            Vector3 bossViewPosition = new Vector3(
                bossSpawnPosition.position.x,
                bossSpawnPosition.position.y,
                originalCameraPosition.z
            );

            float elapsed = 0f;
            while (elapsed < cameraMoveDuration)
            {
                mainCamera.transform.position = Vector3.Lerp(
                    originalCameraPosition,
                    bossViewPosition,
                    elapsed / cameraMoveDuration
                );
                elapsed += Time.deltaTime;
                yield return null;
            }
            mainCamera.transform.position = bossViewPosition;

            // 5. Spuštění spawn animace
            if (bossAnimator != null)
            {
                bossAnimator.Play("BossSpawn", -1, 0f);
            }

            yield return new WaitForSeconds(watchSpawnDuration);

            // 6. Zapnutí hráče a AI bosse
            if (player != null)
            {
                player.enabled = true;
                Debug.Log("PLAYER ENABLED NOW!");
            }
            if (rb != null)
            {
                rb.simulated = true;
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            if (bossScript != null)
            {
                bossScript.enabled = true;
                bossScript.StartBossAI();
            }

            // 7. Pohyb kamery zpět k hráči
            elapsed = 0f;
            Vector3 currentCamPos = mainCamera.transform.position;
            Vector3 targetCamPos = playerTransform.position + originalCameraOffset;

            while (elapsed < cameraMoveDuration)
            {
                mainCamera.transform.position = Vector3.Lerp(
                    currentCamPos,
                    targetCamPos,
                    elapsed / cameraMoveDuration
                );
                elapsed += Time.deltaTime;
                yield return null;
            }

            // 8. Připojení kamery zpět – s dynamickým flipem offsetu podle aktuálního směru hráče!
            mainCamera.transform.SetParent(playerTransform);

            // Flip offset podle aktuálního otočení hráče (toto řeší přemet!)
            float playerFacing = playerTransform.localScale.x > 0 ? 1 : -1;
            Vector3 flippedOffset = new Vector3(
                Mathf.Abs(originalCameraOffset.x) * playerFacing,
                originalCameraOffset.y,
                originalCameraOffset.z
            );

            mainCamera.transform.localPosition = flippedOffset;
            mainCamera.transform.localRotation = Quaternion.identity;

            Debug.Log($"Kamera připojena zpět | Flip offset: {flippedOffset.x} (player facing: {playerFacing})");
        }
    }
}