using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBruiser : Tree
{
    /*
     * This tree controls the actions of the bruiser enemy
     * -Arnie
     */

    public GameObject projectilePrefab;
    public GameObject circleProjectilePrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode Slam = new BTSpreadShot(agent, "Projectiles/SphereProjectile", 24, 20, 0, 20, 180, 1, "HammerSlam", 4);
        BTNode HammerSlam = new BTSlimeBulletHell(agent, circleProjectilePrefab, 36, 2, 20, 15, "HammerSlam", 1, 0.2f, 4);
        BTNode HammerShot = new BTHammerAutoShot(projectilePrefab, 6, 0.75f, 20, 27.5f, 3, "HammerSwing"); ;


        BTNode lineOfSight = new BTHasLineOfSight(agent);
        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(HammerSlam);
        temp.Add(HammerShot);
        BTNode SelectorA = new BTRandSelector(agent, temp);
        temp.Clear();
        temp.Add(lineOfSight);
        temp.Add(SelectorA);
        BTNode SequenceA = new BTSequence(agent, temp);
        return SequenceA;
    }
}