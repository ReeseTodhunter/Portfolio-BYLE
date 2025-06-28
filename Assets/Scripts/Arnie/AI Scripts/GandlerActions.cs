using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GandlerActions : Tree
{
    public GameObject projectilePrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        //BTNode autoShot = new BTAutoShot(Projectiles.BASIC_PROJECTILE, ProjectileMats.BASIC_MAT, 5, 0.33f, 10, 12, 2);
        BTNode autoShot = new BTAutoShot(projectilePrefab, 2, 1.0f, 10, 10, 3, "GandlerShoot");
        BTNode shoot = new BTShootProjectile(Projectiles.GANDLER_PROJECTILE, 30, 30, false, true, "GandlerShoot", 1.0f);
        return shoot;
    }
}
