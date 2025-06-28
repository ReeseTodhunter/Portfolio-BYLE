using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JockeyActions : Tree
{
    public float dashSpeedMultiplier = 5;

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
        //BTNode dashAttack = new BTMinionRush(30, 1f, dashSpeedMultiplier, 10, 10, 1.5f, true, true);
        BTNode laserSpray = new BTPsychicShot(agent, minigunProjectile, null, spawnPos, 1.0f, 3, .01f, 40, 360, 10, 15f, "LaserAttack", "Idle");
        BTNode cerebralThrow = new BTCerebralThrow(agent, cerebral, 0.8f, 5, 20, 15f, 45, "CerebralThrow", "CerebralRecieve", spawnPos);

        //Composites
        List<BTNode> temp = new List<BTNode>();

        temp.Add(cerebralThrow);
        temp.Add(laserSpray);
        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}
