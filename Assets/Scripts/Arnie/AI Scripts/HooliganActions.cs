using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HooliganActions : Tree
{
    /*
        Action tree for the Basic shooter AI. 
        Only has one node, which is shoot projectile.
    */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shoot = new BTShootProjectile(Projectiles.MOLOTOV_PREFAB, 20, 7, false, true, "PistolShot");
        return shoot;
    }
}

