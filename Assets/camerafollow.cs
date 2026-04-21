using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Smoothness")]
    public float smoothSpeed = 0.125f;

    // Current limits (will be changed per level)
    private float minX = -10f;
    private float maxX = 50f;
    private float minY = -10f;
    private float maxY = 10f;

    private Vector3 offset;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

        Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    // Public method to change limits from other scripts (e.g. Level Loader)
    public void SetCameraLimits(float newMinX, float newMaxX, float newMinY = -10f, float newMaxY = 10f)
    {
        minX = newMinX;
        maxX = newMaxX;
        minY = newMinY;
        maxY = newMaxY;
        Debug.Log($"Camera limits updated → MinX: {minX}, MaxX: {maxX}");
    }
}