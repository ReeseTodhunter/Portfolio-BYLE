using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseActions : Tree
{
    /*
        Action tree for the Basic shooter AI. 
        Only has one node, which is shoot projectile.
    */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shoot = new BTShootProjectile(Projectiles.NURSE_PROJECTILE, 20, 7, false, true, "Throw");
        BTNode spread = new BTSpreadShot(agent, "Projectiles/NurseProjectile", 4, 20, 0, 7, 20, 0, "Throw");
        BTNode megaSpread = new BTSpreadShot(agent, "Projectiles/NurseProjectile", 20, 20, 0, 10, 360, 0.5f, "SpreadAttack");


        //Composites
        List<BTNode> temp = new List<BTNode>();

        temp.Add(shoot);
        temp.Add(spread);
        temp.Add(megaSpread);
        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}
