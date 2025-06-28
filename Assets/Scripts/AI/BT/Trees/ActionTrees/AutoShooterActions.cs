using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShooterActions : Tree
{
    /*
     * This tree controls the actions of the AutoShooter enemy
     * -Tom
     */
    public GameObject projectile;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        //BTNode autoShot = new BTAutoShot(Projectiles.BASIC_PROJECTILE, ProjectileMats.BASIC_MAT, 3, 0.33f, 10, 10, 4, "AutoShot");
        BTNode autoShot = new BTAutoShot(projectile, 3, 0.33f, 10, 15, 3, "AutoShot");
        return autoShot;
    }
}
