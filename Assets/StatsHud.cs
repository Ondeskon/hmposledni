using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StatsHUD : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI timeText;

    private float startTime;
    private bool timerStarted = false;     // This prevents resetting on every level

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        TryStartTimer();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryStartTimer();
    }

    private void TryStartTimer()
    {
        if (timerStarted) return;   // ← IMPORTANT: Only start once per run

        string sceneName = SceneManager.GetActiveScene().name.ToLower();

        // Start timer when entering ANY gameplay level
        if (sceneName.Contains("level") || sceneName.Contains("game") || sceneName.Contains("boss"))
        {
            startTime = Time.time;
            timerStarted = true;
            Debug.Log($"Timer STARTED in scene: {sceneName}");
            UpdateUI();
        }
        else
        {
            // In menu → show 00:00 but don't run
            ResetDisplay();
        }
    }

    public void ResetDisplay()
    {
        if (timeText != null) timeText.text = "Time: 00:00";
        if (enemiesText != null) enemiesText.text = "Enemies: 0";
    }

    void Update()
    {
        if (!timerStarted) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.runTime = Time.time - startTime;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (enemiesText != null && GameManager.Instance != null)
        {
            enemiesText.text = $"Enemies: {GameManager.Instance.enemiesKilled}";
        }

        if (timeText != null)
        {
            float time = GameManager.Instance != null ? GameManager.Instance.runTime : (Time.time - startTime);
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    public void RefreshEnemiesCounter()
    {
        if (enemiesText != null && GameManager.Instance != null)
        {
            enemiesText.text = $"Enemies: {GameManager.Instance.enemiesKilled}";
        }
    }
}