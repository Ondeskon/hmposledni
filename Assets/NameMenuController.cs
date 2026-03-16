using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NameMenuController : MonoBehaviour
{
    [Header("Name Panel References")]
    public GameObject namePanel;                    // Celý NamePanel (GameObject)
    public GameObject nameInputObject;              // ← Drag celý NamInput objekt sem (GameObject)
    public Button guestButton;                      // Tlačítko "Play as Guest"
    public Button confirmButton;                    // Tlačítko "Confirm Name"

    [Header("Main Menu Panel References")]
    public GameObject mainMenuPanel;                // Celý MainMenuPanel

    private TMPro.TMP_InputField nameInput;         // ← Privátní reference na TMP komponentu

    void Start()
    {
        // Inicializace TMP_InputField z GameObjectu
        if (nameInputObject != null)
        {
            nameInput = nameInputObject.GetComponent<TMPro.TMP_InputField>();
            if (nameInput == null)
            {
                Debug.LogError("Na objektu " + nameInputObject.name + " chybí komponenta TMP_InputField!");
            }
        }
        else
        {
            Debug.LogError("nameInputObject není připojený – dragni NamInput objekt!");
        }

        // Na začátku vidíme jen name panel
        if (namePanel != null) namePanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        // Připojení tlačítek
        if (guestButton != null) guestButton.onClick.AddListener(PlayAsGuest);
        if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmName);
    }

    public void PlayAsGuest()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerName = "Guest";
        }
        else
        {
            Debug.LogWarning("GameManager.Instance není dostupný!");
        }

        ShowMainMenu();
    }

    public void ConfirmName()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance není dostupný!");
            ShowMainMenu();
            return;
        }

        string name = "";
        if (nameInput != null)
        {
            name = nameInput.text.Trim();
        }

        if (string.IsNullOrEmpty(name))
            name = "Guest";

        GameManager.Instance.playerName = name;
        Debug.Log("Nastaveno jméno: " + name);

        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        if (namePanel != null) namePanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}