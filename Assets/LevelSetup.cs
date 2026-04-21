using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    [Header("Camera Limits for this level")]
    public float minX = -10f;
    public float maxX = 50f;
    public float minY = -10f;
    public float maxY = 10f;

    void Start()
    {
        CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();

        if (camFollow != null)
        {
            camFollow.SetCameraLimits(minX, maxX, minY, maxY);
        }
        else
        {
            Debug.LogError("CameraFollow script not found on Main Camera!");
        }
    }
}