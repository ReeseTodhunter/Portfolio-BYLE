using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStarterActions : Tree
{
    public GameObject projectile;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        //BTNode autoShot = new BTAutoShot(Projectiles.BASIC_PROJECTILE, ProjectileMats.BASIC_MAT, 3, 0.33f, 10, 10, 4, "AutoShot");
        BTNode autoShot = new BTFlameThrowerShoot(projectile, 30, 0.05f, 10, 10, 3, "AutoShot" , 25);
        return autoShot;
    }
}