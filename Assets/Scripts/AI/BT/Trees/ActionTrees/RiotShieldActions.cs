using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiotShieldActions : Tree
{
    /*
     * This tree controls the actions of the riot shield enemy
     * -Tom
     */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shot = new BTShootProjectile(Projectiles.BASIC_PROJECTILE,15,15,true,true,"RiotAim", 0.5f);
        return shot;
    }
}
