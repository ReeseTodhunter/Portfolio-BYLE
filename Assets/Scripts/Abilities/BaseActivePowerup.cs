using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseActivePowerup : MonoBehaviour
{
    public int activePowerUpID;
    [SerializeField] protected float cooldown; // Cooldown between uses
    [SerializeField] string powerupName = ""; // Name of the powerup in item desc box
    [SerializeField] string powerupDescription = ""; // Description of the powerup in item desc box
    [SerializeField] Sprite powerupIcon = null; // Icon that displays on ui when this ability is held
    [SerializeField] float equippedSize = 0.5f; // Size to scale up/down to when equipped
    [SerializeField] Vector3 equippedPosition = new Vector3(0.5f, -0.5f, -0.6f); // Position on the player to move to when equipped
    protected bool equipped = false;

    protected void Update()
    {
        if (!equipped)
        {
            transform.position += (Vector3.up * Mathf.Sin(Time.time * 2) / 2) * Time.deltaTime;
        }
    }

    public virtual float UseAbility()
    {
        return cooldown;
    }

    public virtual void Equip()
    {
        if (equipped) return; // Cannot equip same object twice

        equipped = true;
        transform.parent = PlayerController.instance.gameObject.transform.GetChild(0);
        transform.position = PlayerController.instance.gameObject.transform.position + (Quaternion.Euler(PlayerController.instance.transform.rotation.eulerAngles) * equippedPosition);
        transform.localScale *= equippedSize;

        if (GameManager.GMinstance != null)
        {
            GameManager.GMinstance.powerupsPickedUp[activePowerUpID] = true;
        }
        if (ItemPopupController.instance != null)
        {
            ItemPopupController.instance.RemoveBox(powerupName, powerupDescription, transform.position);
        }
    }
    public virtual void Unequip()
    {
        if (!equipped) return; // Cannot unequip a non active ability

        equipped = false;
        transform.parent = null;
        transform.position = PlayerController.instance.transform.position;
        transform.rotation = Quaternion.identity;
        transform.localScale *= 1.0f/equippedSize;

        ItemPopupController.instance.AddBox(powerupName, powerupDescription, transform.position);
    }

    // Getters
    public bool IsEquipped() { return equipped; }
    public float GetCooldownTime() { return cooldown; }
    public Sprite GetSprite() { return powerupIcon; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.instance.gameObject)
        {
            ItemPopupController.instance.AddBox(powerupName, powerupDescription, transform.position);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.instance.gameObject)
        {
            ItemPopupController.instance.RemoveBox(powerupName, powerupDescription, transform.position);
        }
    }

    public string GetDescription()
    {
        return powerupDescription;
    }

    public string GetTitle()
    {
        return powerupName;
    }
}
