using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image healthFill;
    public TextMeshProUGUI healthText;
    public GameObject healthBarContainer;  // Celý panel – pro zapínání/vypínání

    [Header("Boss Reference")]
    public Boss boss;

    [Header("Settings")]
    public float smoothSpeed = 5f;

    private float targetFillAmount = 1f;

    void Start()
    {
        if (healthBarContainer != null)
            healthBarContainer.SetActive(false);

        UpdateHealthBar();
        Debug.Log("HealthBar Start – Boss: " + (boss != null ? "OK" : "NULL"));
    }

    void Update()
    {
        if (boss == null) return;

        targetFillAmount = (float)boss.currentHealth / boss.maxHealth;
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);

        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(boss.currentHealth > 0 && !boss.isDead);
        }
    }

    // Public metoda – voláš z Boss.cs
    public void UpdateHealthBar()
    {
        if (boss == null || healthFill == null || healthText == null) return;

        targetFillAmount = (float)boss.currentHealth / boss.maxHealth;
        healthFill.fillAmount = targetFillAmount;
        healthText.text = boss.currentHealth + " / " + boss.maxHealth;

        Debug.Log("HealthBar updated: " + boss.currentHealth + " / " + boss.maxHealth);
    }
}