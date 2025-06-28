using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadaActions : Tree
{
    public Transform weaponPos1;
    public Transform weaponPos2;
    public Transform weaponPos3;
    public Transform weaponPos4;

    public GameObject projectilePrefab;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode autoShot = new BTQuadShot(projectilePrefab, weaponPos1, weaponPos2, weaponPos3, weaponPos4, 4, 0.33f, 10, 30, 2, "ArmadaShoot", 0);
        return autoShot;
    }
}
