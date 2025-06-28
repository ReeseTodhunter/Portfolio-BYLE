using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShooterMovement : Tree
{
    /*
        Basic shooter Movement tree. This agent will try to gain
        a line of sight on the player and get within a certain 
        distance.
        -Tom
    */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode hasLineOfSight = new BTHasLineOfSight(agent);
        BTNode withinRange = new BTWithinRange(agent,12,1);
        BTNode directMove = new BTDirectMove(4, 1, BTDirectMove.FailState.ALL);
        BTNode move = new BTMoveToPosition(4, BTMoveToPosition.positionType.lineOfSight,1,true);

        //Composites
        List<BTNode> temp = new List<BTNode>();
        
        temp.Add(withinRange);
        temp.Add(directMove);
        BTNode SelectorB = new BTSelector(agent, temp);
        temp.Clear();

        temp.Add(hasLineOfSight);
        temp.Add(SelectorB);
        BTNode SequenceA = new BTSequence(agent, temp);
        temp.Clear();

        temp.Add(SequenceA);
        temp.Add(move);
        BTNode root = new BTSelector(agent, temp);

        return root;
    }
}
