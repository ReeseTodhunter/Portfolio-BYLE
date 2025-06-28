using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerActions : Tree
{
    /*
     * This tree controls the actions of the charger enemy
     * -Tom
     */
    public float dashSpeedMultiplier = 5;
    public float maxDashRange = 8;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode lineOfSight = new BTHasLineOfSight(agent, false);
        BTNode boulderThrow = new BTBoulderThrow(agent, 1.167f, 30, 2, 7, 3, 20);
        BTNode dashAttack = new BTChargePounce(30, 1f, dashSpeedMultiplier, 10, 25, 1.5f, true, true);
        BTNode directThrow = new BTBoulderThrow(agent, "Projectiles/DirectBoulderThrow", 1.1167f, 30, 20);
        BTNode withinRange = new BTWithinRange(agent, maxDashRange, 0, false);

        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(withinRange);
        temp.Add(dashAttack);
        BTNode sequenceA = new BTSequence(agent, temp);
        temp.Clear();

        temp.Add(sequenceA);
        temp.Add(directThrow);
        BTNode selectorA = new BTSelector(agent, temp);
        temp.Clear();

        temp.Add(lineOfSight);
        temp.Add(selectorA);
        BTNode SequenceB = new BTSequence(agent, temp);
        temp.Clear();

        temp.Add(SequenceB);
        temp.Add(boulderThrow);
        BTNode root = new BTSelector(agent, temp);

        return root;
        //BTNode boulderThrow = new BTBoulderThrow(agent, 1.167f, 30, 2, 7, 3, 20);
        //BTNode dashAttack = new BTMinionRush(30, 1f, dashSpeedMultiplier, 10, 25, 1.5f, true, true);
        //BTNode directThrow = new BTBoulderThrow(agent, "Projectiles/DirectBoulderThrow", 1.1167f, 30, 20);

        //Composites
        //List<BTNode> temp = new List<BTNode>();

        //temp.Add(boulderThrow);
        //temp.Add(dashAttack);
        //temp.Add(directThrow);
        //BTNode randSelector = new BTRandSelector(agent, temp);
        //return randSelector;
    }
}
