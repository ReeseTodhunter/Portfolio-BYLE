using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncineratorActions : Tree
{
    public GameObject projectile;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode FlameThrow = new BTFlameThrowerShoot(projectile, 60, 0.01f, 10, 10, 3, "FlameMouth", 25);
        BTNode MoltovToss = new BTDelayMolotov(Projectiles.MOLOTOV_PREFAB, 20, 15, false, 1.0f, "MolotovThrow", 1.5f);

        //Composites
        List<BTNode> temp = new List<BTNode>();

        temp.Add(FlameThrow);
        temp.Add(MoltovToss);
        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}