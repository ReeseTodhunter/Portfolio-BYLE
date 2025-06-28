using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunActions : Tree
{
    public GameObject projectile;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        //BTNode autoShot = new BTAutoShot(Projectiles.BASIC_PROJECTILE, ProjectileMats.BASIC_MAT, 3, 0.33f, 10, 10, 4, "AutoShot");
        BTNode autoShot = new BTSoldierShoot(projectile, 25, 0.1f, 9, 25, 5, "AutoShot", 5, "reload");
        return autoShot;
    }
}
