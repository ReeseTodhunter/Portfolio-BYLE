using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpredictorActions : Tree
{
    public GameObject projectilePrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        float rand = Random.Range(0, 1f);

        //Primitives
        BTNode shoot = new BTUnPredictable(projectilePrefab, 15, 0.10f, 18, 20, 3 + rand, "HavocShoot", 45);
        return shoot;
    }
}