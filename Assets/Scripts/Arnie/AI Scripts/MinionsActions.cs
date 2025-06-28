using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionsActions : Tree
{
    public float dashSpeedMultiplier = 5;

    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode dashAttack = new BTMinionRush(30, 1f, dashSpeedMultiplier, 10, 10, 1.5f, true, true);
        return dashAttack;
    }
}
