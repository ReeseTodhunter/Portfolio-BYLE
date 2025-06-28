using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShooterFastAction : Tree
{
    public GameObject projectilePrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        //BTNode autoShot = new BTAutoShot(Projectiles.BASIC_PROJECTILE, ProjectileMats.BASIC_MAT, 5, 0.33f, 10, 12, 2);
        BTNode autoShot = new BTAutoShot(projectilePrefab, 5, 0.33f, 10, 10, 3, "AutoShot");
        return autoShot;
    }
}
