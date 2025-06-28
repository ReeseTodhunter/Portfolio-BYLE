using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupremeShotgunnerActions : Tree
{
    public GameObject projectilePrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shotgunShot = new BTSpreadShot(agent, Projectiles.BASIC_PROJECTILE, ProjectileMats.BASIC_MAT, 5, 14, 0, 10, 45, 2, "ShotgunShot");
        return shotgunShot;
    }
}
