using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperMovement : Tree
{
    /*
     * Movement tree for the sniper enemy
     * -Tom
     */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode lineOfSight = new BTHasLineOfSight(agent);
        BTNode GetLineOfSight = new BTMoveToNearest(agent, true, 15, 1, 5);
        BTNode retreat = new BTRetreat(agent,true, 8,3,5);
        BTNode moveToSight = new BTMoveToNearest(agent,true,15,1,5);
        //Composites
        List<BTNode> temp = new List<BTNode>();
        
        temp.Add(lineOfSight);
        temp.Add(retreat);
        BTNode SequenceA = new BTSequence(agent, temp);
        temp.Clear();

        temp.Add(SequenceA);
        temp.Add(moveToSight);
        BTNode root = new BTSelector(agent, temp);
        return root;
    }
}
