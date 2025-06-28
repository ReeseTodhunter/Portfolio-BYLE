using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossActions : Tree
{
    /*
     * This tree controls the actions of the Slime Boss enemy
     * -Tom
     */
    public GameObject bulletHellPrefab;
    public GameObject superShotPrefab;
    public GameObject multiShotPrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        BTNode expandingBulletHell = new BTSlimeBulletHell(agent,bulletHellPrefab,24,5,20,15,"",1,.1f,4);
        BTNode superShot = new BTSlimeSuperShot(agent,superShotPrefab,bulletHellPrefab,3,25,20,1,48,"",true,4);
        BTNode multiShot = new BTAutoShot(multiShotPrefab, 5, .1f, 15, 30, 2);

        //Composites
        List<BTNode> temp = new List<BTNode>();
        
        temp.Add(expandingBulletHell);
        temp.Add(superShot);
        temp.Add(multiShot);
        BTNode randSelector = new BTRandSelector(agent,temp);
        return randSelector;
    }
}
