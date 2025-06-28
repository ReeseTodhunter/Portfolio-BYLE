using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTGreenCultistActions : Tree
{
    /*
     * This tree controls the actions of the Green cultist enemy
     * -Tom
     */
    public GameObject SeekerPrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode seeker = new BTSpawnEntity(agent,SeekerPrefab,agent.projectileSpawn,1f,"SeekerAttack",true,1f);
        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(seeker);

        BTNode selector = new BTSelector(agent, temp);
        return selector;
    }
}

