using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStims : BaseActivePowerup
{
    [SerializeField] float healAmount = 50.0f;

    public override float UseAbility()
    {
        PlayerController.instance.Heal(healAmount);
        CameraDamageEffect.instance.SetMinOpacity();
        return -1.0f;
    }
}
