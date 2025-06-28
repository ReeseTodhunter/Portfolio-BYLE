using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicatorMovement : Tree
{
    public float safeDistance = 25;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode withinRange = new BTWithinRange(agent, safeDistance, 0);
        BTNode lineOfSight = new BTHasLineOfSight(agent);
        BTNode retreat = new BTRetreat(agent, false, 35, 1, 5);
        BTNode directMove = new BTDirectMove(4, 1, BTDirectMove.FailState.ALL);

        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(lineOfSight);
        temp.Add(retreat);
        BTNode SequencerA = new BTSequence(agent, temp);

        temp.Clear();
        temp.Add(withinRange);
        temp.Add(SequencerA);
        BTNode SequencerB = new BTSequence(agent, temp);

        temp.Clear();
        temp.Add(directMove);
        temp.Add(SequencerB);
        BTNode root = new BTSequence(agent, temp);

        return root;
    }
}