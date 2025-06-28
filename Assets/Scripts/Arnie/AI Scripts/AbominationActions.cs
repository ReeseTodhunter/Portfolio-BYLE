using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationActions : Tree
{
    public GameObject[] enemies;
    public Transform spawnPos;
    public GameObject SpawnEffect;

    public int timer = 0;

    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTAbominationSpawn spawn = new BTAbominationSpawn(enemies, spawnPos, SpawnEffect, 1.5f, "Idle", 3f);

        //Composites
        List<BTNode> temp = new List<BTNode>();

        temp.Add(spawn);
        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}