using UnityEngine;
using Firebase;
using Firebase.Firestore;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseFirestore db;

    async void Start()
    {
        Debug.Log("Inicializuji Firebase...");

        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            Debug.Log("Firebase OK! Všechny závislosti jsou ready.");
            FirebaseApp app = FirebaseApp.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
            Debug.Log("Firestore instance vytvořena: " + db.App.Name);
        }
        else
        {
            Debug.LogError("Firebase selhal! Status: " + dependencyStatus);
        }
    }
}