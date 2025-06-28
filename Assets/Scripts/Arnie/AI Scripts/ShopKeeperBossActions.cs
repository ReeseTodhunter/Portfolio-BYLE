using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeperBossActions : Tree
{
    public GameObject bulletHellPrefab;
    public GameObject superShotPrefab;
    public GameObject multiShotPrefab;

    public float dashSpeedMultiplier = 5;
    public float maxDashRange = 8;

    public Material laserMat;
    public GameObject laserPrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        BTNode expandingBulletHell = new BTShopKeeperCoinSlam(agent, bulletHellPrefab, 24, 10, 20, 15, "ShopKeeperSlam", 1, .3f, 4);
        BTNode coinThrow = new BTAutoShot(bulletHellPrefab, 15, 0.15f, 15, 30, 2, "CoinThrow");

        //Composites
        List<BTNode> temp = new List<BTNode>();

        temp.Add(expandingBulletHell);
        temp.Add(coinThrow);

        BTNode randSelector = new BTRandSelector(agent, temp);
        return randSelector;
    }
}
