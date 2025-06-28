using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrusierMovement : Tree
{
    /*
     * Movement tree for the brusier enemy
     * -Tom
     */
    protected override BTNode SetupTree(BTAgent agent)
    {
        BTNode chase = new BTMoveToPosition(4,BTMoveToPosition.positionType.lineOfSight,1,true);
        return chase;
    }
}
