using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTBorisArtillery : BTNode
{
    /*
        Behaviour Tree Effector node, this node is written for the "Big Boris" 
        enemy. It controls the Artillery attack of Big Boris, where the boss
        shoots its weapon in the air and the projectiles then land on the ground
        -Tom
    */
    private float cooldown, timeSinceLastUsed = 0; //Cooldown of the node
    private GameObject projectilePrefab, vfxPrefab; //Prefab of the projectile and the vfx for the projectile being shot in the air
    private Transform vfxPos; //Position of vfx spawn
    private float projectileCount, timeBetweenProjectiles,projectileIntervalTimer,currProjectileCount,projectileDamage; //Variables to control the projectile
    private float projectileSpreadRadius; //Spread that the projectiles will rain down in
    private float windupDelay, windupTimer; //Windup to sync with animations
    private string startAnim, exitAnim; //animations
    private List<GameObject> vfxObjects = new List<GameObject>(); //cached vfx to delete them all
    private List<Vector3> validSpawns = new List<Vector3>(); //Valid spawns for landing projectiles
    private enum scripState //Enum that controls what state the node is in
    { 
        Inactive,
        Windup,
        SpawnProjectiles,
        Wait,
        Exit
    }
    private scripState currState; //Current state
    
    //Constructor
    public BTBorisArtillery(BTAgent _agent, GameObject _projectilePrefab, GameObject _vfxPrefab, Transform _vfxPos,float _cooldown,float _projectileCount,float _timeBetweenProjectiles, float _projectileDamage, float _projectileSpreadRadius, float _nodeWindup, string _startAnim = "", string _exitAnim = "")
    {
        agent = _agent;
        vfxPos = _vfxPos;
        vfxPrefab = _vfxPrefab;
        cooldown = _cooldown;
        projectilePrefab = _projectilePrefab;
        projectileCount = _projectileCount;
        timeBetweenProjectiles = _timeBetweenProjectiles;
        projectileDamage = _projectileDamage;
        projectileSpreadRadius = _projectileSpreadRadius;
        windupDelay = _nodeWindup;
        startAnim = _startAnim;
        exitAnim = _exitAnim;
    }
    public override NodeState Evaluate(BTAgent _agent) //Evaluate node
    {
        if(Time.time - timeSinceLastUsed < cooldown){return NodeState.FAILURE;} //return false if cooldown not done
        currState = scripState.Windup; //set state
        agent.GetComponent<Animator>().Play(startAnim); //play animation
        agent.SetMovementEnabled(false); //Lock movement
        agent.SetCurrentAction(this); //lock action tree
        windupTimer = 0; //reset windup timer
        projectileIntervalTimer = 0; //reset shootin interval timer
        currProjectileCount = 0; //reset projectile count
        vfxObjects.Clear(); //reset vfx list
        validSpawns.Clear(); //Reset valid spawns
        return NodeState.SUCCESS;         //return true
    }
    public override void UpdateNode(BTAgent agent) //Update node
    {
        if(vfxObjects.Count != 0 && currState != scripState.Inactive) //Move all vfx objects if they aren't null
        {
            foreach(GameObject vfxObject in vfxObjects)
            {
                if(vfxObject == null){continue;}
                vfxObject.transform.position += vfxObject.transform.forward * 25 * Time.deltaTime; //Move object
            }
        }
        switch(currState)
        {
            case scripState.Inactive: //do nothing
                break;
            case scripState.Windup: //Windup, wait for anim to finish
                windupTimer += Time.deltaTime;
                if(windupTimer < windupDelay){break;} //return if anim not done
                windupTimer = 0;
                currState = scripState.SpawnProjectiles; //move to next state
                validSpawns = GetRandomExplosionPositions(projectileSpreadRadius); //Get all valid spawns
                break;
            case scripState.SpawnProjectiles: //spawn projectiles
                projectileIntervalTimer += Time.deltaTime;
                if(projectileIntervalTimer < timeBetweenProjectiles){break;} //If inbtween shots and interval not met, return
                if(currProjectileCount >= projectileCount){windupTimer = 0; currState = scripState.Wait; break;} //Go to next state if all projectiles fired
                projectileIntervalTimer = 0;
                currProjectileCount ++; //increment projectiles 
                GameObject vfx = GameObject.Instantiate(vfxPrefab,vfxPos.position,Quaternion.identity); //instantiate vfx
                vfx.GetComponent<KillScript>().lifeTime = 2f; //set killscript
                vfx.GetComponent<KillScript>().decayStart = 1f; 
                vfxObjects.Add(vfx); //add to list
                vfx.transform.rotation = vfxPos.rotation; //set rotation

                Vector3 spawn; //Get spawn
                if(validSpawns.Count > 0) //get spawn position from list of spawns
                {
                    spawn = validSpawns[Random.Range(0,validSpawns.Count)];
                    validSpawns.Remove(spawn);
                } 
                else //if no valid spawns, put directly on top of player
                {
                    spawn = PlayerController.instance.transform.position;
                }
                GameObject impact = GameObject.Instantiate(projectilePrefab,spawn,Quaternion.identity); //instantiate raining projectile
                impact.GetComponent<Explosion>().radius = 1; //set blast radius
                impact.GetComponent<Explosion>().playerDamage = 15; //set damage
                break;
            case scripState.Wait:
                windupTimer += Time.deltaTime;
                if(windupTimer < 2){break;} //if still waiting, return
                currState = scripState.Exit; //next state
                break;
            case scripState.Exit:
                ExitNode(); //exit node
                break;
        }
    }
    private List<Vector3> GetRandomExplosionPositions(float _range) //Get a list of valid positions
    {
        EQSNode[,] nodes = EQS.instance.GetNodes(); //Get all EQS nodes
        List<Vector3> validPositions = new List<Vector3>();
        foreach(EQSNode node in nodes) //Add any EQS node's positions if its valid
        {
            if(!node.GetTraversable()){continue;}
            if(node.GetDistance() > _range){continue;}
            validPositions.Add(node.GetWorldPos());
        }
        return validPositions; //return positions
    }
    private void ExitNode() //exit node
    {
        agent.GetComponent<Animator>().Play(exitAnim); //play exit anim
        agent.ClearCurrentAction(); //unlock action tree
        agent.SetMovementEnabled(true); //unlock movement
        timeSinceLastUsed = Time.time;
        currState = scripState.Inactive;
    }
}
