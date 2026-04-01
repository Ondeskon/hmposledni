using UnityEngine;
using TMPro;
using UnityEngine.UI;              // pro Button
using UnityEngine.SceneManagement; // pro LoadScene
using System.Collections;

public class VictoryScreen : MonoBehaviour
{
    [Header("UI Text References – přetáhni sem z prefabu")]
    public TextMeshProUGUI enemiesKilledText;    // text pro "Enemies Defeated: X"
    public TextMeshProUGUI timeSurvivedText;     // text pro "Time Survived: MM:SS"
    public TextMeshProUGUI playerNameText;       // text pro "Player: Jméno"

    [Header("Back to Menu Button")]
    public Button backToMenuButton;              // ← PŘETAHNI SEM TLAČÍTKO "Button (TMP)" z prefabu

    void Start()
    {
        // Automaticky aktualizuj stats po malém delay
        StartCoroutine(UpdateStats());

        // Připoj listener na tlačítko (pokud ho ještě nemáš v Inspectoru)
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.RemoveAllListeners(); // pro jistotu smaž staré
            backToMenuButton.onClick.AddListener(BackToMenu);
            Debug.Log("Back to Menu tlačítko připojeno automaticky");
        }
        else
        {
            Debug.LogWarning("backToMenuButton reference chybí – přetáhni Button z prefabu do Inspectoru!");
        }
    }

    public IEnumerator UpdateStats()
    {
        yield return new WaitForSeconds(0.3f); // delay, aby Boss stihl aktualizovat GameManager

        if (GameManager.Instance == null)
        {
            Debug.LogError("VictoryScreen: GameManager.Instance je NULL!");
            yield break;
        }

        Debug.Log($"VictoryScreen UpdateStats voláno (po delay) – aktuální data:");
        Debug.Log($"  enemiesKilled: {GameManager.Instance.enemiesKilled}");
        Debug.Log($"  runTime: {GameManager.Instance.runTime:F1}s");
        Debug.Log($"  playerName: {GameManager.Instance.playerName}");

        // Enemies Killed
        if (enemiesKilledText != null)
        {
            enemiesKilledText.text = $"Enemies Defeated: {GameManager.Instance.enemiesKilled}";
            enemiesKilledText.ForceMeshUpdate(); // donutí text překreslit okamžitě
            Debug.Log("Enemies text aktualizován");
        }
        else
        {
            Debug.LogWarning("enemiesKilledText je None – přetáhni text do Inspectoru!");
        }

        // Time Survived (MM:SS)
        if (timeSurvivedText != null)
        {
            float minutes = Mathf.FloorToInt(GameManager.Instance.runTime / 60);
            float seconds = GameManager.Instance.runTime % 60;
            timeSurvivedText.text = $"Time Survived: {minutes:00}:{seconds:00}";
            timeSurvivedText.ForceMeshUpdate();
            Debug.Log("Time text aktualizován");
        }
        else
        {
            Debug.LogWarning("timeSurvivedText je None – přetáhni text do Inspectoru!");
        }

        // Player Name
        if (playerNameText != null)
        {
            playerNameText.text = $"Player: {GameManager.Instance.playerName}";
            playerNameText.ForceMeshUpdate();
            Debug.Log("Player name text aktualizován");
        }
        else
        {
            Debug.LogWarning("playerNameText je None – přetáhni text do Inspectoru!");
        }

        Debug.Log("VictoryScreen stats aktualizovány!");
    }

    // Metoda volaná po kliknutí na Back to menu
    public void BackToMenu()
    {
        Debug.Log("Back to Menu kliknuto – vracím se do hlavního menu");

        // Reset stats pro čistý start nové runy (volitelné, ale doporučuji)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewRun();
        }

        // Načti hlavní menu – ZMĚŇ NÁZEV NA TVŮJ SKUTEČNÝ !!!
        SceneManager.LoadScene("MainMenu"); // např. "Menu", "MainScene", "Start" – přesně jak je v Build Settings
    }
}