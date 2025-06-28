using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaCultistActions : Tree
{
    /*
     * This tree controls the actions of the mega cultist enemy
     * -Arnie
     */

    public GameObject SeekerPrefab;
    public Material laserMat;
    public GameObject laserPrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode shapeShot = new BTShapeShot(agent, Projectiles.CULTIST_PROJECTILE, Projectiles.BYLE_PROJECTILE, ProjectileMats.BASIC_MAT, 25, 20, BTShapeShot.attackShape.circle, 1f, 16, "ShapeAttack");
        BTNode seeker = new BTSpawnEntity(agent, SeekerPrefab, agent.projectileSpawn, 1f, "SeekerAttack", true, 1f);
        BTNode beamAttack = new BTShopKeeperBeam(agent, laserPrefab, 1.33f, 2, 15, 3f, laserMat, 10);

        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(shapeShot);
        temp.Add(seeker);
        temp.Add(beamAttack);

        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}