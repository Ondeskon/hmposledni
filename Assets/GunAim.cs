using UnityEngine;

public class GunAim : MonoBehaviour
{
    private SpriteRenderer gunSprite;

    void Start()
    {
        gunSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate gun to face mouse (adjust offset if needed)
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Flip gun vertically ONLY when aiming left (mouse left of player)
        if (gunSprite != null)
        {
            bool aimingLeft = mousePos.x < transform.position.x;
            gunSprite.flipY = aimingLeft;
        }

        // Optional: slight offset so gun doesn't point exactly from center
        // transform.localPosition = new Vector3(0.3f, 0, 0);
    }
}