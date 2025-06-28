using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTBorisMinigun : BTNode
{
    /*
        Behaviour Tree effector node, controls the boss "Big Boris"'s
        minigun attack
        -Tom
    */
    private GameObject projectilePrefab, muzzleFlashPrefab; //prefabs
    private float cooldown; //node cooldown
    private float timeLastUsed = 0; //time since nodde was activated
    private float projectileCount, currProjectileCount; //Projectile counts
    private float spreadInDegrees; //projectile spread
    private float damage,velocity; //damage and velocity of projectiles
    private string entryAnim,exitAnim; //animations that the node will play
    private float timeBetweenShot, shotTimer; //Interval between projectiles
    private Transform spawnPos; //spawn location of projectiles
    private enum ScriptState //Enum that controls what state the node is in
    {
        inactive,
        shooting,
        exit
    }
    private ScriptState currState; //current state

    //Constructor
    public BTBorisMinigun(BTAgent _agent, GameObject _projectilePrefab, GameObject _muzzleFlashPrefab, Transform _spawnPos, float _cooldown, float _timeBetweenShot,int _projectileCount, int _spreadInDegrees, float _damagePerProjectile, float _velocity, string _entryAnim = "", string _exitAnim = "")
    {
        agent = _agent;
        projectilePrefab = _projectilePrefab;
        muzzleFlashPrefab = _muzzleFlashPrefab;
        spawnPos = _spawnPos;
        cooldown = _cooldown;
        timeBetweenShot = _timeBetweenShot;
        projectileCount = _projectileCount;
        spreadInDegrees = _spreadInDegrees;
        damage = _damagePerProjectile;
        velocity = _velocity;
        entryAnim = _entryAnim;
        exitAnim = _exitAnim;
    }
    public override NodeState Evaluate(BTAgent agent) //Evaluate node value
    {
        if(Time.time - timeLastUsed < cooldown){return NodeState.FAILURE;} //if cooldown not finished, return false
        agent.SetCurrentAction(this); //lock action tree to this
        currState = ScriptState.shooting; //set state
        agent.GetComponent<Animator>().Play(entryAnim); //play animation
        currProjectileCount = 0;
        shotTimer = 0;
        return NodeState.SUCCESS; //return true
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        switch(currState)
        {
            case ScriptState.inactive: //do nothing
                break;
            case ScriptState.shooting: //Shoot projectiles
                if(currProjectileCount >= projectileCount){currState = ScriptState.exit;} //exit if all projectiles shot
                shotTimer += Time.deltaTime;
                if(shotTimer <= timeBetweenShot){break;} //return if interval between shots not met
                shotTimer = 0;
                currProjectileCount ++; //iterate 
                GameObject projectile = GameObject.Instantiate(projectilePrefab,spawnPos.position, spawnPos.rotation); //Instantiate projectile
                projectile.transform.rotation = spawnPos.rotation; //Set rotation
                Vector3 angle = projectile.transform.eulerAngles; //Get angles
                angle.y += Random.Range(-spreadInDegrees/2,spreadInDegrees/2); //apply random spread
                projectile.transform.eulerAngles = angle; //apply spread to projectile
                projectile.GetComponent<Projectile>().Init(velocity,0,10,damage,agent.gameObject); //init projectile
                break;
            case ScriptState.exit:
                ExitNode(); //exit node
                break;
        }
    }
    private void ExitNode()
    {
        agent.ClearCurrentAction(); //unlock action tree
        currState = ScriptState.inactive; //reset state
        agent.GetComponent<Animator>().Play(exitAnim); //play exit animation
        timeLastUsed = Time.time; //reset cooldown
    }
}
