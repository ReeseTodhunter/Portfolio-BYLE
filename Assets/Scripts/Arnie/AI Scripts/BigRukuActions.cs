using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRukuActions : Tree
{
    /*
     * This tree controls the actions of the bruiser enemy
     * -Arnie
     */

    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        //Smoke teleport
        BTNode teleport = new BTBigRukuTeleport(agent, 2, 0, 25, true, 10, true, "Teleport", "VFX/Ruku/Teleport", 0, false);
        BTNode HammerSlam = new BTBigRukuBlast(agent, "Projectiles/ElliptoidProjectile", 16, 5, 20, 15, "Spin", 1, 0.3f, 3);


        BTNode lineOfSight = new BTHasLineOfSight(agent);
        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(HammerSlam);
        temp.Add(teleport);
        BTNode SelectorA = new BTRandSelector(agent, temp);
        temp.Clear();
        temp.Add(lineOfSight);
        temp.Add(SelectorA);
        BTNode SequenceA = new BTSequence(agent, temp);
        return SequenceA;
    }
}