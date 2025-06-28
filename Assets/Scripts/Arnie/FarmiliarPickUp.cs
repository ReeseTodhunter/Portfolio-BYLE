using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmiliarPickUp : BasePowerup
{

    public GameObject farmiliar;
    protected override bool OnPickup()
    {
        GameObject entity = Instantiate(farmiliar, PlayerController.instance.transform.position, Quaternion.identity);
        // Add powerup to hud
        if (PowerupUI.instance != null && icon != null) PowerupUI.instance.AddPowerup(icon, FormatText(powerupName), FormatText(powerupDescription));
        return true;
    }
}
