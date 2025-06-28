using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerActions : Tree
{
    public GameObject minion;
    public GameObject shinyMinion;
    public GameObject SpawnEffect;

    public GameObject projectile;

    public int timer = 0;

    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shoot = new BTShootProjectile(Projectiles.DUPLICATOR_PROJECTILE, 20, 7, false, true, "Shoot", 0f);
        //BTNode autoShot = new BTAutoShot(projectile, 3, 0.33f, 10, 10, 3, "Shoot");
        BTSpawnMinion spawn = new BTSpawnMinion(minion, shinyMinion, SpawnEffect, 0.4f, 169, "Duplicate", 1.0f);

        //Composites
        List<BTNode> temp = new List<BTNode>();

        temp.Add(shoot);
        temp.Add(spawn);
        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}