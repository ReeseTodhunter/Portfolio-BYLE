using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : BasePowerup
{
    protected override bool OnPickup()
    {
        // Early return and don't pickup if players health is full
        if (PlayerController.instance.GetHealthRatio() >= 1.0f) return false;
        CameraDamageEffect.instance.SetMinOpacity();
        float healthGiven = PlayerController.instance.Heal(modAmount);
        return true;
    }
}
