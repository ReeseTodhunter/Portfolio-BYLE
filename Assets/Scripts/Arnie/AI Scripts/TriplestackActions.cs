using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriplestackActions : Tree
{
    public float dashSpeedMultiplier = 5;

    public Material laserMat;
    public GameObject laserPrefab;

    public GameObject minigunProjectile;
    public GameObject cerebral;
    public GameObject cerebralModel;

    public Transform spawnPos;

    public GameObject activeProjctile;

    public void SetCerebralVisible(bool _visible)
    {
        cerebralModel.SetActive(_visible);
    }

    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode laserSpray = new BTPsychicShot(agent, minigunProjectile, null, spawnPos, 1.0f, 3, .0025f, 80, 360, 10, 15f, "TriAttack", "Idle");

        //Composites
        List<BTNode> temp = new List<BTNode>();

        //temp.Add(cerebralThrow);
        temp.Add(laserSpray);
        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}
