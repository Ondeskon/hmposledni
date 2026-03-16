using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;  

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;  // Drag music AudioSource here
    [SerializeField] private AudioSource sfxSource;    // Drag SFX AudioSource here (new one)

    void Start()
    {
        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.Play();
            musicSource.loop = true;
        }
    }

    public void PlayGame()
    {
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(sfxSource.clip);  // Play click
        }

        // Call fade instead of direct load
        GetComponent<FadeToBlack>().StartFadeAndLoad("Level1");
    }

    private IEnumerator PlaySoundAndLoad()
{
    if (sfxSource != null)
    {
        sfxSource.PlayOneShot(sfxSource.clip);
        yield return new WaitForSeconds(sfxSource.clip.length * 0.8f);  // Wait almost full sound duration
    }
    else
    {
        yield return new WaitForSeconds(0.3f);  // Fallback delay if no SFX
    }

    SceneManager.LoadScene("Level1");
}

public void QuitGame()
    {
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(sfxSource.clip);  // Quick click sound
        }
        Application.Quit();
    }
}