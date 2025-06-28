using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearProjectileAbility : BaseActivePowerup
{
    public override float UseAbility()
    {
        // Destroy every projectile lol
        Projectile[] projectiles = FindObjectsOfType(typeof(Projectile)) as Projectile[];
        for (int i = projectiles.Length-1; i >= 0; --i)
        {
            if (projectiles[i].GetParentObject() == PlayerController.instance.gameObject) continue; // Do not destroy player projectiles
            Destroy(projectiles[i].gameObject);
        }

        CameraController.instance.ShakeCameraOverTime(0.5f, 2.0f);
        return base.UseAbility();
    }
}
