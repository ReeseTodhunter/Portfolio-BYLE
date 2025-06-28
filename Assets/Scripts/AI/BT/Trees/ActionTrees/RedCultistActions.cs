using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCultistActions : Tree
{
    /*
     * This tree controls the actions of the red cultist enemy
     * -Tom
     */
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shapeShot = new BTShapeShot(agent,Projectiles.CULTIST_PROJECTILE,Projectiles.BYLE_PROJECTILE,ProjectileMats.BASIC_MAT,25,25,BTShapeShot.attackShape.random,1f,12, "ShapeAttack");
        BTNode teleport = new BTTeleport(agent, 8, 3, 20, true, 4, true, "Teleport");

        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(shapeShot);
        temp.Add(teleport);

        BTNode selector = new BTSelector(agent, temp);
        return selector;
    }
}
