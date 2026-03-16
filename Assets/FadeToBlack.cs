using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeToBlack : MonoBehaviour
{
    [SerializeField] private Image fadeImage;           // Drag tvůj full-screen black Image sem
    [SerializeField] private float fadeDuration = 0.8f; // Doba fade (sekundy)

    private bool isFading = false;

    // Hlavní metoda, kterou budeš volat z tlačítek nebo jiných skriptů
    public void StartFadeAndLoad(string sceneName)
    {
        if (isFading)
        {
            Debug.Log("Fade už běží, ignoruju další volání");
            return;
        }

        StartCoroutine(FadeAndLoad(sceneName));
        Debug.Log("Fade called for scene: " + sceneName);
        // zbytek kódu...

    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        isFading = true;

        Debug.Log("Fade coroutine STARTED - Alpha before: " + fadeImage.color.a);

        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            Debug.Log("Fading... t = " + t + ", alpha = " + fadeImage.color.a);
            yield return null;
        }

        fadeImage.color = endColor;
        Debug.Log("Fade DONE - Alpha now: " + fadeImage.color.a);

        SceneManager.LoadScene(sceneName);
        isFading = false;
    }
    // Volitelná metoda pro reset fade (když chceš vrátit alpha na 0)
    public void ResetFade()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
        isFading = false;
    }
}