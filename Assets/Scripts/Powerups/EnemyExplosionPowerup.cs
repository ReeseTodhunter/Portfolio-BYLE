using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosionPowerup : BasePowerup
{
    protected override bool OnPickup()
    {
        if (PlayerController.instance.GetModifier(ModifierType.EnemyExplosion) > 0) return false; // Player cannot have more than 1 of this powerup
        return base.OnPickup();
    }
}
