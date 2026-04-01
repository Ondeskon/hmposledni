using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }

    void LateUpdate()
    {
        // KLÍÈOVÉ: Toto udrží healthbar vždy správnì otoèený, 
        // i když se nepøítel v EnemyControlleru otáèí (flipuje)
        transform.rotation = Quaternion.identity;
    }
}