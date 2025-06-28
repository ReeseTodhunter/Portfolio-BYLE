using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePowerup : MonoBehaviour
{
    public int powerupID;
    [SerializeField] ModifierType modType; // Modifier that will be changed
    [SerializeField] protected float modAmount = 0.0f; // Amount of the modifier that will be changed
    [SerializeField] protected float modVariation = 0.0f; // +/- difference added onto the mod amount for variety among powerups
    [SerializeField] string pickupText = ""; // Text that displays after the number
    [SerializeField] protected string powerupName = ""; // Name of the powerup in item desc box
    [SerializeField] protected string powerupDescription = ""; // Description of the powerup in item desc box
    [SerializeField] protected Sprite icon; // Icon for the powerup

    int playerRange = 0; // How close the player is to powerup. 0 = out of range, 1 = close enough for description to pop up, 2 = close enough to pick up
    GameObject itemDescBox;

    protected void Update()
    {
        // Idle float animation
        transform.position += (Vector3.up * Mathf.Sin(Time.time * 2) / 2) * Time.deltaTime;
    }

    // Virtual OnPickup function. Changed by any child classes for their own respective extra effects.
    // Most powerups just change a modifier though, so the code here will be fine on it's own
    protected virtual bool OnPickup()
    {
        if (GameManager.GMinstance != null)
        {
            GameManager.GMinstance.powerupsPickedUp[powerupID] = true;
        }

        // Add powerup to hud
        if (PowerupUI.instance != null && icon != null) PowerupUI.instance.AddPowerup(icon, FormatText(powerupName), FormatText(powerupDescription));

        PlayerController.instance.AddModifier(modType, modAmount);
        return true;
    }

    // Called when the player enters/exits a powerup trigger box
    private void TriggerUpdate()
    {
        // Player has walked into item description range
        if (playerRange > 0)
        {
            // Spawn item box
            ItemPopupController.instance.AddBox(FormatText(powerupName), FormatText(powerupDescription), transform.position);

            // Player has walked into pick up range
            if (playerRange == 2) 
            {
                // Run pickup code. If OnPickup() returns false then the pickup will not be used
                if (!OnPickup()) return;

                // Play pickup sound effect
                PlayerController.instance.PlayItemPickup();

                // Paste text onto floating text if possible
                if (ProjectileLibrary.instance != null)
                {
                    GameObject createdText = Instantiate(ProjectileLibrary.instance.GetProjectile(Projectiles.FLOATING_TEXT), PlayerController.instance.transform.position + Vector3.up * 2.0f, Quaternion.Euler(45.0f, 0.0f, 0.0f));
                    createdText.GetComponent<ItemPickupFade>().SetText(FormatText(pickupText));
                }

                // Destroy item description box
                ItemPopupController.instance.RemoveBox(FormatText(powerupName), FormatText(powerupDescription), transform.position);

                // Destroy pickup
                Destroy(gameObject);
            }
        }
        else
        {
            // Destroy item description box
            ItemPopupController.instance.RemoveBox(FormatText(powerupName), FormatText(powerupDescription), transform.position);
        }
    }
    protected void OnTriggerEnter(Collider other)
    {
        // Check if player has walked into trigger
        if (other.gameObject == PlayerController.instance.gameObject)
        {
            playerRange++;
            TriggerUpdate();
        }
    }
    protected void OnTriggerExit(Collider other)
    {
        // Check if player has walked out of trigger
        if (other.gameObject == PlayerController.instance.gameObject)
        {
            playerRange--;
            TriggerUpdate();
        }
    }

    // Lets other code change the values on the powerup for "variety"
    public void SetModAmount(float a_modAmount)
    {
        modAmount = a_modAmount;
    }
    public void RandomiseModAmount()
    {
        modAmount += Random.Range(-modVariation, modVariation); ;
    }

    // Inserts number values into text
    protected string FormatText(string inputString)
    {
        return inputString
            .Replace("{percent}", Mathf.Round(modAmount * 100.0f).ToString() + "%")
            .Replace("{integer}", Mathf.Round(modAmount).ToString())
            .Replace("{-percent}", Mathf.Round(modAmount * -100.0f).ToString() + "%")
            .Replace("{float}", (System.Math.Round(modAmount, 2)).ToString()); // rounds to 2 decimal points
    }

    // Inserts number values into text. Shows modAmount values including ranges as opposed to just single value
    protected string FormatTextRange(string inputString)
    {
        if (modVariation == 0.0f) return FormatText(inputString); // If no variation in potential power-up values, do not try to display a range
        return inputString
            .Replace("{percent}", Mathf.Round((modAmount - modVariation) * 100.0f).ToString() + "-" + Mathf.Round((modAmount + modVariation) * 100.0f).ToString() + "%")
            .Replace("{integer}", Mathf.Round(modAmount - modVariation).ToString() + "-" + Mathf.Round(modAmount + modVariation).ToString())
            .Replace("{-percent}", Mathf.Round((modAmount - modVariation) * -100.0f).ToString() + "-" + Mathf.Round((modAmount + modVariation) * -100.0f).ToString() + "%")
            .Replace("{float}", (System.Math.Round((modAmount - modVariation), 2)).ToString() + "-" + (System.Math.Round((modAmount + modVariation), 2)).ToString()); // rounds to 2 decimal points
    }

    public string GetDescription()
    {
        return powerupDescription;
    }

    public string GetDescriptionEncyclopedia()
    {
        return FormatTextRange(powerupDescription);
    }

    public string GetTitle()
    {
        return powerupName;
    }
}
