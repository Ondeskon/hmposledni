using UnityEngine;
using UnityEngine.UI;

public class PlayerHearts : MonoBehaviour
{
    [Header("Heart Images")]
    public Image[] hearts = new Image[3];  // Drag Heart1 do [0], Heart2 do [1], Heart3 do [2]

    [Header("Sprites")]
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    [Header("Player")]
    public PlayerController player;  // Drag Jason Player z Hierarchy

    private int maxHearts = 3;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference v PlayerHearts je NULL!");
        }
        else
        {
            Debug.Log("PlayerHearts Start – currentHealth: " + player.currentHealth);
        }

        UpdateHearts();  // Inicializace na 3 plná srdíčka
    }

    // Voláš tohle po každém hitu z PlayerController
    public void OnPlayerDamage()
    {
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        if (player == null) return;

        int currentHearts = Mathf.Max(0, player.currentHealth);  // 3 → 2 → 1 → 0

        Debug.Log("UpdateHearts – currentHearts: " + currentHearts);

        for (int i = 0; i < maxHearts; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].sprite = (i < currentHearts) ? fullHeartSprite : emptyHeartSprite;
                Debug.Log("Heart[" + i + "] nastaveno na: " + ((i < currentHearts) ? "FULL" : "EMPTY"));
            }
            else
            {
                Debug.LogWarning("Heart[" + i + "] je NULL – drag ho do array!");
            }
        }
    }
}