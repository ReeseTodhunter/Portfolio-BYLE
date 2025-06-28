using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCultistActions : Tree
{
    /*
     * action tree for the green cultist enemy
     * -Tom
     */
    public GameObject SeekerPrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        
        BTNode teleport = new BTTeleport(agent, 8, 3, 20, true, 4, true, "Teleport");
        BTNode seeker = new BTSpawnEntity(agent,SeekerPrefab,agent.projectileSpawn,2,"SeekerAttack",true,2);
        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(seeker);
        temp.Add(teleport);

        BTNode selector = new BTSelector(agent, temp);
        return selector;
    }
}

