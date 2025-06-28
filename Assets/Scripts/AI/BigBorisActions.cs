using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBorisActions : Tree
{
    /*
     * Action tree for the big boris boss
     * -Tom
     */
    public GameObject minigunProjectile;
    public Transform spawnPos;
    public Transform vfxPos;
    public GameObject artilleryProjectile;
    public GameObject artilleryVFX;
    public GameObject EyeGameObject;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode minigunSpray = new BTBorisMinigun(agent,minigunProjectile,null,spawnPos,6,.07f,50,120,10,12.5f,"BulletSpray","Idle");
        BTNode artillery = new BTBorisArtillery(agent,artilleryProjectile,artilleryVFX,vfxPos,6,15,.25f,10,40,1,"BulletArtilleryWindup","Idle");
        BTNode laserEye = new BTEyeLaser(agent,EyeGameObject,1,40,4,2, "LaserEyeWindup", "LaserEyeWindDown");
        //composite
        List<BTNode> temp = new List<BTNode>();
        temp.Add(minigunSpray);
        temp.Add(artillery);
        temp.Add(laserEye);
        BTNode rndSelector = new BTRandSelector(agent,temp);
        return rndSelector;
    }
}
