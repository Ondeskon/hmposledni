using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Current Run Stats")]
    public string playerName = "Guest";
    public int enemiesKilled = 0;
    public float runTime = 0f;

    [Header("Player Health Persistence")]
    public int playerMaxHealth = 3;
    public int playerCurrentHealth = 3; // Toto se přenáší mezi levely!

    private float startTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager singleton vytvořen! Instance je ready.");

        startTime = Time.time;
        Debug.Log("GameManager Awake volán – Instance nastaven na: " + (Instance != null ? Instance.gameObject.name : "NULL"));
    }

    void Update()
    {
        runTime = Time.time - startTime;
    }

    // Zavolej tohle při startu nové runy (např. z menu nebo po smrti)
    public void StartNewRun()
    {
        playerCurrentHealth = playerMaxHealth; // Reset HP na maximum při nové runě
        enemiesKilled = 0;
        startTime = Time.time;
        runTime = 0f;
        playerName = "Guest"; // nebo nech staré, pokud chceš
        Debug.Log("Nová runa začala – HP resetováno na " + playerCurrentHealth);
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        Debug.Log("Zabitý enemy! Celkem: " + enemiesKilled);
    }
    public async Task EndRunAsync(bool victory = true)
    {
        float timeCompleted = runTime;

        if (FirebaseInitializer.db == null)
        {
            Debug.LogError("Firestore není inicializováno – run se neuloží!");
            return;
        }

        var runData = new Dictionary<string, object>
    {
        { "playerName", playerName },
        { "enemiesKilled", enemiesKilled },
        { "timeCompleted", timeCompleted },
        { "victory", victory },  // ← KLÍČOVÉ: true jen při zabití bosse
        { "timestamp", FieldValue.ServerTimestamp }
    };

        try
        {
            await FirebaseInitializer.db.Collection("runs").AddAsync(runData);
            Debug.Log($"Run uložen do Firestore! Victory: {victory}, {playerName} - {enemiesKilled} zabitých, čas: {timeCompleted:F1}s");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Chyba při ukládání runu do Firestore: " + ex.Message);
        }

        // Reset až po uložení (ale jen pokud chceš resetovat hned – pro victory screen nech stats)
        // ResetRun();  ← zakomentováno – resetuj až po victory screenu nebo v nové runě
    }

    // Nová metoda – zavolej ji až PO zobrazení victory screenu (např. z VictoryScreen)
    public void ResetAfterVictory()
    {
        enemiesKilled = 0;
        startTime = Time.time;
        runTime = 0f;
        // playerName = "Guest"; // nebo nech (VICTORY) – podle tebe
        Debug.Log("Stats resetovány PO zobrazení victory screenu");
    }

    public void SetPlayerName(string name)
    {
        playerName = string.IsNullOrEmpty(name) ? "Guest" : name;
        Debug.Log("Nastaveno jméno: " + playerName);
    }

    public void SetPlayerHealth(int newHealth)
    {
        playerCurrentHealth = Mathf.Clamp(newHealth, 0, playerMaxHealth);
        Debug.Log("HP nastaveno na: " + playerCurrentHealth);
    }
}