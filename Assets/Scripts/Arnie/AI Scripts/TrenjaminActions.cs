using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenjaminActions : Tree
{


    public GameObject projectilePrefab;
    public GameObject circleProjectilePrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTDelayShot strike = new BTDelayShot(projectilePrefab, 50, 35, true, 1.00f, "MagesticusCast");
        BTNode shapeShot = new BTMajesticusShape(agent, circleProjectilePrefab, Projectiles.BYLE_PROJECTILE, ProjectileMats.BASIC_MAT, 35, 20, BTMajesticusShape.attackShape.circle, 1f, 8, "MagesticusShape");


        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(shapeShot);
        temp.Add(strike);

        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}
