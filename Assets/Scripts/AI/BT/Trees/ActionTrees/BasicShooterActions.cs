using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShooterActions : Tree
{
    /*
     * This tree controls the actions of the basic shooter enemy
     * -Tom
     */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shoot = new BTShootProjectile(Projectiles.BASIC_PROJECTILE, 20, 7, false, true, "PistolShot");
        return shoot;
    }
}
