using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperActions : Tree, IWeaponSwapper
{
    /*
     * This tree controls the actions of the AutoShooter enemy
     * -Tom
     */
    public GameObject rifle, pistol;
    public Transform rifleProjectilePos, pistolProjectilePos;
    public string weaponSwapAnimName;
    private int currIndex = 0;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode farAway = new BTWithinRange(Agent, 12, 0, true);
        BTNode withinRange = new BTWithinRange(Agent, 12, 0, false);
        BTNode swapToPistol = new BTSwapWeapon(agent, this, 1, 2);
        BTNode shootPistol = new BTShootProjectile(Projectiles.BASIC_PROJECTILE, 20, 7, false, false, "PistolShoot");
        BTNode swapToSniper = new BTSwapWeapon(agent, this, 0, 2);
        BTNode shootSniper = new BTChargeShot(Projectiles.SNIPER_PROJECTILE, ProjectileMats.SNIPER_MAT, 60, 0, 33, 4, 2, false, true, "SniperShoot");
        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(farAway);
        temp.Add(swapToSniper);
        temp.Add(shootSniper);

        BTNode SequenceA = new BTSequence(Agent, temp);
        temp.Clear();

        temp.Add(withinRange);
        temp.Add(swapToPistol);
        temp.Add(shootPistol);
        BTNode SequenceB = new BTSequence(Agent, temp);
        temp.Clear();

        temp.Add(SequenceA);
        temp.Add(SequenceB);
        BTNode root = new BTSelector(Agent, temp);
        return root;    
    }
    public void SwapWeapon(int index) //Allows the sniper to swap weapons
    {
        if(currIndex == index){return;}
        Agent.GetComponent<Animator>().Play(weaponSwapAnimName);
        currIndex = index;
        if (index == 0)
        {
            rifle.SetActive(true);
            pistol.SetActive(false);
            Agent.projectileSpawn = rifleProjectilePos;
            return;
        }
        rifle.SetActive(false);
        pistol.SetActive(true);
        Agent.projectileSpawn = pistolProjectilePos;
    }
    public float GetCurrWeapon(){return currIndex;}
}
