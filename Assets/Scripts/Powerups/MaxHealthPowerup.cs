using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHealthPowerup : BasePowerup
{
    protected override bool OnPickup()
    {
        CameraDamageEffect.instance.SetMinOpacity();
        return base.OnPickup();
    }
}
