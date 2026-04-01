using UnityEngine;

public class PersistentHUD : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("StatsHUD set to DontDestroyOnLoad – will survive scene changes");
    }
}