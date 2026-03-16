using UnityEngine;
using TMPro;
using System.Collections;

public class VictoryScreen : MonoBehaviour
{
    [Header("UI Text References – přetáhni sem z prefabu")]
    public TextMeshProUGUI enemiesKilledText; // text pro "Enemies Defeated: X"
    public TextMeshProUGUI timeSurvivedText; // text pro "Time Survived: MM:SS"
    public TextMeshProUGUI playerNameText; // text pro "Player: Jméno"

    void Start()
    {
        // NEvolej UpdateStats() hned – počkej, až Boss dokončí Die()
        StartCoroutine(UpdateStats());
    }

    public IEnumerator UpdateStats()
    {
        yield return new WaitForSeconds(0.3f); // 0.3 sekundy delay – dostatečný, ale ne příliš dlouhý

        if (GameManager.Instance == null)
        {
            Debug.LogError("VictoryScreen: GameManager.Instance je NULL!");
            yield break;
        }

        Debug.Log($"VictoryScreen UpdateStats voláno (po delay) – aktuální data z GameManageru:");
        Debug.Log($"  enemiesKilled: {GameManager.Instance.enemiesKilled}");
        Debug.Log($"  runTime: {GameManager.Instance.runTime:F1}s");
        Debug.Log($"  playerName: {GameManager.Instance.playerName}");

        // Enemies Killed
        if (enemiesKilledText != null)
        {
            enemiesKilledText.text = $"Enemies Defeated: {GameManager.Instance.enemiesKilled}";
            enemiesKilledText.ForceMeshUpdate(); // donutí UI překreslit okamžitě
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
            timeSurvivedText.ForceMeshUpdate(); // donutí UI překreslit
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
            playerNameText.ForceMeshUpdate(); // donutí UI překreslit
            Debug.Log("Player name text aktualizován");
        }
        else
        {
            Debug.LogWarning("playerNameText je None – přetáhni text do Inspectoru!");
        }

        Debug.Log("VictoryScreen stats aktualizovány!");
    }
}