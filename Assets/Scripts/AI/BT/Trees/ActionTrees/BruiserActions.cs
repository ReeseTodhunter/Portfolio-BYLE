using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserActions : Tree
{
    /*
     * This tree controls the actions of the bruiser enemy
     * -Tom
     */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTDelayShot strike = new BTDelayShot("Projectiles/HammerProjectile", 15,30,false,1.17f,"Strike");
        BTNode Slam = new BTSpreadShot(agent,"Projectiles/SphereProjectile",24,20,0,20,180,1,"Slam", 4);


        BTNode lineOfSight = new BTHasLineOfSight(agent);
        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(Slam);
        temp.Add(strike);
        BTNode SelectorA = new BTRandSelector(agent,temp);
        temp.Clear();
        temp.Add(lineOfSight);
        temp.Add(SelectorA);
        BTNode SequenceA = new BTSequence(agent,temp);
        return SequenceA;
    }
}
