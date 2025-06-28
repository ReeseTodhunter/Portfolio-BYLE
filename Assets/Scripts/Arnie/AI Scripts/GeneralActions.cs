using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralActions : Tree
{
    public GameObject weaponPos1;
    public GameObject weaponPos2;
    public BTAgent btagent;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shoot = new BTShootProjectile(Projectiles.BASIC_PROJECTILE, 25, 7, false, true, "Shoot");

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
