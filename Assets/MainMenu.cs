using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    // --- PŘIDEJ TUTO PROMĚNNOU ---
    [SerializeField] private GameObject controlsPanel;

    void Start()
    {
        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.Play();
            musicSource.loop = true;
        }
    }

    // --- PŘIDEJ TYTO DVĚ METODY ---
    public void OpenControls()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(sfxSource.clip);
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(sfxSource.clip);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }
    // ------------------------------

    public void PlayGame()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(sfxSource.clip);
        GetComponent<FadeToBlack>().StartFadeAndLoad("Level1");
    }

    public void QuitGame()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(sfxSource.clip);
        Application.Quit();
    }
}