using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapon Slots (UI)")]
    public Image gunSlot;
    public Image macheteSlot;

    [Header("Actual Weapons")]
    public GameObject gunObject;        // The entire Gun GameObject (parent)
    public GameObject macheteObject;    // The entire Machete GameObject (parent)

    [Header("Visual Feedback")]
    public Color selectedColor = Color.white;
    public Color normalColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    private bool isGunSelected = true;

    void Start()
    {
        // Make sure we start with Gun selected
        isGunSelected = true;
        UpdateWeaponUI();
    }

    public void SwitchToGun()
    {
        isGunSelected = true;
        UpdateWeaponUI();
    }

    public void SwitchToMachete()
    {
        isGunSelected = false;
        UpdateWeaponUI();
    }

    private void UpdateWeaponUI()
    {
        // UI Highlight
        if (gunSlot != null) gunSlot.color = isGunSelected ? selectedColor : normalColor;
        if (macheteSlot != null) macheteSlot.color = !isGunSelected ? selectedColor : normalColor;

        // Actually enable/disable weapons
        if (gunObject != null) gunObject.SetActive(isGunSelected);
        if (macheteObject != null) macheteObject.SetActive(!isGunSelected);

        Debug.Log("Weapon switched → " + (isGunSelected ? "GUN" : "MACHETE"));
    }

    // Quick switch with Q key
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isGunSelected = !isGunSelected;
            UpdateWeaponUI();
        }
    }
}