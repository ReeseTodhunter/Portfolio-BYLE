using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VomitActions : Tree
{
    /*
        Action tree for the Basic shooter AI. 
        Only has one node, which is shoot projectile.
    */

    public GameObject vomit;
    public GameObject spit;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode vomitAttack = new BTVomitProjectile(vomit, 50, 7, false, true, "Vomit");
        BTNode spitAttack = new BTAutoShot(spit, 8, .1f, 15, 30, 5, "Vomit");

        //Composites
        List<BTNode> temp = new List<BTNode>();

        temp.Add(vomitAttack);
        temp.Add(spitAttack);


        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}
