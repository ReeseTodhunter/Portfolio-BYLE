using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserMovement : Tree
{
    /*
     * Movement tree for the chaser enemy
     * -Tom
     */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode chase = new BTMoveToPosition(0, BTMoveToPosition.positionType.lineOfSight,1,true);
        BTNode hasLineOfSight = new BTHasLineOfSight(agent);
        BTNode directChase = new BTDirectMove(0,1,BTDirectMove.FailState.SIGHTLOST,0,4f);

        //Composites
        List<BTNode> temp = new List<BTNode>();
        
        temp.Add(hasLineOfSight);
        temp.Add(directChase);
        BTNode sequenceA = new BTSequence(agent, temp);

        temp.Clear();
        temp.Add(sequenceA);
        temp.Add(chase);
        BTNode root = new BTSelector(agent, temp);
        
        return root;
    }
}
