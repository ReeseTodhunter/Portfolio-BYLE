using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossMovement : Tree
{
    /*
     * Movement tree for the slime boss enemy
     * -Tom
     */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Rand movement
        BTNode randWalk = new BTRandWalk(agent,8,4);
        return randWalk;
    }
}
