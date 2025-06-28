using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterActions : Tree
{
    public GameObject projectilePrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shotgunShot = new BTSpreadShot(agent, Projectiles.BASIC_PROJECTILE, ProjectileMats.BASIC_MAT, 7, 20, 0, 15, 45, 0, "ShotgunShot");
        BTNode autoShot = new BTAutoShot(projectilePrefab, 5, 0.25f, 15, 20, 0);

        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(shotgunShot);
        temp.Add(autoShot);
        BTNode rndSelector = new BTRandSelector(Agent, temp);
        return rndSelector;

    }
}

