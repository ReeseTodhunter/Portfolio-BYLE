using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleCultistActions : Tree
{
    /*
     * This tree controls the actions of the purple cultist enemy
     * -Tom
     */
    public Material laserMat;
    public GameObject laserPrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        
        BTNode teleport = new BTTeleport(agent, 8, 3, 20, true, 4, true, "Teleport");
        BTNode beamAttack = new BTCultistBeam(agent,laserPrefab,1.33f,2,15,4, laserMat, 10);
        //Composites
        List<BTNode> temp = new List<BTNode>();
        //temp.Add(teleport);
        temp.Add(beamAttack);
        BTNode selector = new BTRandSelector(agent, temp);
        return selector;
    }
}
