using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeActions : Tree
{
    public float damage, range,meleeCooldown;
    protected override BTNode SetupTree(BTAgent agent)
    {
        BTNode melee = new BTMelee(agent,range,meleeCooldown,damage);
        return melee;
    }
}
