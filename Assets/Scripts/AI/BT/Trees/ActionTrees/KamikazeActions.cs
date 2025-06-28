using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeActions : Tree
{
    /*
     * This tree controls the actions of the kamikaze enemy
     * -Tom
     */
    public float proximity = 3;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode withinRange = new BTWithinRange(agent, proximity, 0,false);
        BTNode kamikaze = new BTKamikaze();

        //Composites
        List<BTNode> temp = new List<BTNode>();

        temp.Add(withinRange);
        temp.Add(kamikaze);
        BTNode root = new BTSequence(agent, temp);

        return root;
    }
}
