using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunnerActions : Tree
{
    /*
     * This tree controls the actions of the shotgunner enemy
     * -Tom
     */
    public GameObject projectilePrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shotgunShot = new BTSpreadShot(agent,Projectiles.BASIC_PROJECTILE,ProjectileMats.BASIC_MAT,3,14,0,7,20,2, "ShotgunShot");
        return shotgunShot;
    }
}
