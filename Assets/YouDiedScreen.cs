using UnityEngine;
using System.Collections;

public class YouDiedScreen : MonoBehaviour
{
    public float respawnDelay = 3f;
    public string firstLevelName = "level1";  // ← ZMĚŇ na správný název!

    void Start()
    {
        // Zastav čas (volitelné)
        Time.timeScale = 1f;

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        Debug.Log("Restarting to: " + firstLevelName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevelName);
    }
}