using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Image fillImage;

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (fillImage == null)
        {
            Debug.LogError("BossHealthBar: Fill Image není pøiøazen v Inspektoru!");
            return;
        }

        float fillAmount = (float)currentHealth / (float)maxHealth;
        fillImage.fillAmount = fillAmount;

        // Tohle uvidíš v konzoli - pokud se èísla mìní, ale bar ne, je chyba v nastavení Image v Unity
        Debug.Log($"UI Update: Bar na {fillAmount * 100}%");
    }
}