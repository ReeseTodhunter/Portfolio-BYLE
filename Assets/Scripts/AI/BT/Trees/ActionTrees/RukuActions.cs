using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RukuActions : Tree
{
    /*
     * This tree controls the actions of the Ruku mini-boss enemy
     * -Tom
     */
    public GameObject hat;
    public GameObject projectilePrefab;
    public Transform throwTransform;
    public void SetHatVisible(bool _visible)
    {
        hat.SetActive(_visible);
    }
    protected override BTNode SetupTree(BTAgent agent)
    {
        SetHatVisible(true);
        //Slash
        BTNode swordSlash = new BTSpreadShot(Agent, "Projectiles/ElliptoidProjectile", 3, 25, 0, 15, 30, 0.65f,"KatanaSlash", 3);
        //Hat throw
        BTNode hatThrow = new BTRukuHatThrow(agent,projectilePrefab,1.65f,5,25,35,45,"HatThrow","HatRecieve",throwTransform);
        //Smoke teleport
        BTNode teleport = new BTTeleport(agent, 5, 0, 25, true, 10, true,"","VFX/Ruku/Teleport",0, false);
        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(hatThrow);
        temp.Add(swordSlash);
        temp.Add(teleport);
        BTNode rndSelector = new BTRandSelector(Agent, temp);
        return rndSelector;
    }
}
