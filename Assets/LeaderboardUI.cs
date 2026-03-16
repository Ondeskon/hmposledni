using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References - Rows")]
    public TextMeshProUGUI[] rankTexts = new TextMeshProUGUI[10];
    public TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[10];
    public TextMeshProUGUI[] killsTexts = new TextMeshProUGUI[10];
    public TextMeshProUGUI[] timeTexts = new TextMeshProUGUI[10];

    [Header("UI Panels Toggle")]
    public GameObject mainMenuPanel; // ← Přetáhni MainMenuPanel (s Play/Exit) sem
    public GameObject leaderboardPanel; // ← Přetáhni LeaderboardPanel (self) sem

    [Header("Loading & Fallback")]
    public GameObject loadingPanel; // „Načítám...“ overlay (optional)
    public TextMeshProUGUI noDataText; // Text „Žádné výsledky“ (optional)

    [Header("Sorting Button")]
    public Button sortButton; // ← Přetáhni tlačítko pro sortování sem
    public TextMeshProUGUI sortButtonText; // ← TextMeshPro uvnitř tlačítka (přetáhni)

    private FirebaseFirestore db;
    private bool sortByEnemies = true; // true = enemies desc, false = time asc

    void Start()
    {
        // Nastav sort button
        if (sortButton != null)
        {
            sortButton.onClick.AddListener(ToggleSortMode);
            UpdateSortButtonText();
        }
        else
        {
            Debug.LogWarning("SortButton reference chybí – přetáhni tlačítko do Inspectoru!");
        }

        // Spusť načítání s malým delayem
        Invoke(nameof(TryLoadLeaderboard), 1f);
    }

    private void ToggleSortMode()
    {
        sortByEnemies = !sortByEnemies;
        UpdateSortButtonText();
        _ = LoadLeaderboard(); // refresh leaderboardu s novým sortem
        Debug.Log($"Sort přepnut na: {(sortByEnemies ? "Enemies Killed" : "Time Completed")}");
    }

    private void UpdateSortButtonText()
    {
        if (sortButtonText != null)
        {
            sortButtonText.text = sortByEnemies ? "Sort by Enemies Killed" : "Sort by Time Completed";
        }
    }

    private async void TryLoadLeaderboard()
    {
        if (FirebaseInitializer.db == null)
        {
            Debug.LogWarning("Firebase db ještě není inicializováno – čekám...");
            await Task.Delay(1000);
            TryLoadLeaderboard();
            return;
        }

        db = FirebaseInitializer.db;
        Debug.Log("LeaderboardUI: Firebase db ready");

        // Schovej leaderboard na začátku
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);

        // Zobraz loading overlay (pokud existuje)
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (noDataText != null) noDataText.gameObject.SetActive(false);

        await LoadLeaderboard();
    }

    public void ToggleLeaderboard()
    {
        if (leaderboardPanel == null)
        {
            Debug.LogError("LeaderboardPanel reference chybí!");
            return;
        }

        Debug.Log("ToggleLeaderboard voláno! Panel active: " + leaderboardPanel.activeSelf);

        if (leaderboardPanel.activeSelf)
        {
            leaderboardPanel.SetActive(false);
        }
        else
        {
            leaderboardPanel.SetActive(true);
            _ = LoadLeaderboard(); // Načti vždy při otevření
        }
    }

    public void BackToMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            Debug.Log("Main Menu zapnut!");
        }
        else
        {
            Debug.LogWarning("MainMenuPanel reference chybí!");
        }

        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
            Debug.Log("Leaderboard vypnut!");
        }
        else
        {
            Debug.LogWarning("LeaderboardPanel reference chybí!");
        }

        Debug.Log("BackToMainMenu voláno!");
    }

    public async Task LoadLeaderboard()
    {
        Debug.Log("LoadLeaderboard START");

        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (noDataText != null) noDataText.gameObject.SetActive(false);

        // Smaž stará data v textových polích
        for (int i = 0; i < 10; i++)
        {
            if (i < rankTexts.Length && rankTexts[i] != null) rankTexts[i].text = "";
            if (i < nameTexts.Length && nameTexts[i] != null) nameTexts[i].text = "";
            if (i < killsTexts.Length && killsTexts[i] != null) killsTexts[i].text = "";
            if (i < timeTexts.Length && timeTexts[i] != null) timeTexts[i].text = "";
        }

        // Nastav query – jen dokončené runy + sort podle aktuálního módu
        Query query = db.Collection("runs")
            .WhereEqualTo("victory", true);  // ← KLÍČOVÁ ZMĚNA: jen vítězové

        if (sortByEnemies)
        {
            query = query.OrderByDescending("enemiesKilled");
        }
        else
        {
            query = query.OrderBy("timeCompleted");
        }

        query = query.Limit(10);

        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        Debug.Log($"Snapshot přijat! Count: {(snapshot != null ? snapshot.Count : 0)} | Sort: {(sortByEnemies ? "Enemies desc" : "Time asc")} | Jen vítězové");

        if (snapshot == null || snapshot.Count == 0)
        {
            Debug.Log("Žádné dokončené runy v leaderboardu.");
            if (loadingPanel != null) loadingPanel.SetActive(false);
            if (noDataText != null) noDataText.gameObject.SetActive(true);
            return;
        }

        int rank = 1;
        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            if (!doc.Exists) continue;

            Dictionary<string, object> data = doc.ToDictionary();

            string name = data.ContainsKey("playerName") ? data["playerName"].ToString() : "Unknown";
            long kills = data.ContainsKey("enemiesKilled") ? (long)data["enemiesKilled"] : 0;
            double time = data.ContainsKey("timeCompleted") ? (double)data["timeCompleted"] : 0;

            int index = rank - 1;
            if (index < 10)
            {
                if (rankTexts[index] != null) rankTexts[index].text = rank + ".";
                if (nameTexts[index] != null) nameTexts[index].text = name;
                if (killsTexts[index] != null) killsTexts[index].text = kills.ToString();

                // Čas formátovaný jako MM:SS
                if (timeTexts[index] != null)
                {
                    int minutes = Mathf.FloorToInt((float)time / 60);
                    int seconds = Mathf.FloorToInt((float)time % 60);
                    timeTexts[index].text = $"{minutes:00}:{seconds:00}";
                }
            }

            rank++;
        }

        // Po načtení: schovej loading, zobraz leaderboard
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(true);

        Debug.Log($"Leaderboard END – {rank - 1} záznamů (jen vítězové)");
    }
}