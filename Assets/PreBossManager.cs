using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PreBossManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button bringItOnButton;
    public Button imScaredButton;

    [Header("Too Bad Panel")]
    public GameObject tooBadPanel;  // Drag TooBadPanel

    void Start()
    {
        // Connect buttons
        bringItOnButton.onClick.AddListener(() => LoadBossFight());
        imScaredButton.onClick.AddListener(() => ShowTooBad());
    }

    public void LoadBossFight()
    {
        SceneManager.LoadScene("Level3");  // tvùj boss level
    }

    public void ShowTooBad()
    {
        StartCoroutine(TooBadSequence());
    }

    IEnumerator TooBadSequence()
    {
        tooBadPanel.SetActive(true);  // Show black "TOO BAD" screen
        yield return new WaitForSeconds(1f);  // 1 second
        tooBadPanel.SetActive(false);
        LoadBossFight();  // Load boss
    }
}