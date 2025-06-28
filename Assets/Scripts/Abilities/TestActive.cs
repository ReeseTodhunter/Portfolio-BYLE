using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestActive : BaseActivePowerup
{
    public override float UseAbility()
    {
        Instantiate(ProjectileLibrary.instance.GetProjectile(Projectiles.FLOATING_TEXT), PlayerController.instance.transform.position + Vector3.up, Quaternion.Euler(0.0f, 45.0f, 0.0f))
            .GetComponent<ItemPickupFade>().SetText("test ability!");
        return base.UseAbility();
    }
}
