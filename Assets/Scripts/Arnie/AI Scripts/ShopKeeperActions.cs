using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeperActions : Tree
{
    public GameObject weaponPos1;
    public GameObject weaponPos2;
    public BTAgent btagent;
    /*
        Action tree for the Basic shooter AI. 
        Only has one node, which is shoot projectile.
    */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shoot = new BTShootProjectile(Projectiles.BASIC_PROJECTILE, 35, 7, false, true);
        
        return shoot;

    }

    private void Update()
    {
        if (btagent.projectileSpawn == weaponPos1.transform)
        {
            btagent.projectileSpawn = weaponPos2.transform;
        }
        else
        {
            btagent.projectileSpawn = weaponPos1.transform;
        }
    }
}

