using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSpreadShot : BTNode
{

    /*
    * This node allows agents to fire multiple projectiles in a spread, functioning like a shotgun
    * -Tom
    */
    private float projectileCount,speed,acceleration,damage,arc; //number of projectiles, and their stats and the spread arc
    private GameObject projectilePrefab; //projectile predfabs
    private float cooldown, lastTimeUsed = 0; //cooldown duration
    private float delayDuration, delayTimer = 0; //delay duration
    private string animName; //animation to play when shooting
    private bool needsLineOfSight = false, locksMovement = true; //whether the node needs a line of sight and whether it locks movement
    private enum ScriptState{
        INACTIVE,
        SHOOTING,
        COOLDOWN,
        EXIT
    }
    private ScriptState currState = ScriptState.INACTIVE; //current state of the script

    //Constructors
    public BTSpreadShot(BTAgent _agent, Projectiles _projectileType, ProjectileMats _projectileMat,int _projectileCount, float _speed, float _acceleration,float _damage, float _arcDegrees, float _cooldown = 5, string _animName = "Null")
    {
        projectilePrefab = ProjectileLibrary.instance.GetProjectile(_projectileType);
        agent = _agent;
        projectileCount = _projectileCount;
        speed = _speed;
        acceleration = _acceleration;
        damage = _damage;
        arc = _arcDegrees;
        cooldown = _cooldown;
        delayDuration = 0;
        animName = _animName;
        needsLineOfSight = false;
    }
    public BTSpreadShot(BTAgent _agent, string projectilePath, int _projectileCount, float _speed, float _acceleration, float _damage, float _spreadInDegrees, float _delay, string _animName = "Null",float _cooldown = 0, bool _needsLineOfSight = false, bool _locksMovement = true)
    {
        agent = _agent;
        projectilePrefab = Resources.Load(projectilePath) as GameObject;
        projectileCount = _projectileCount;
        speed = _speed;
        acceleration = _acceleration;
        damage = _damage;
        arc = _spreadInDegrees;
        delayDuration = _delay;
        cooldown = _cooldown;
        animName = _animName;
        needsLineOfSight = _needsLineOfSight;
        locksMovement = _locksMovement;
    }

    public override NodeState Evaluate(BTAgent agent) //evaluate the output of the node
    {
        if(Time.time - lastTimeUsed > cooldown)
        {
            if(!EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight() && needsLineOfSight) //return true if has line of sight
            {
                return NodeState.SUCCESS;
            }
            if(locksMovement) //lock movement
            {
                agent.SetMovementEnabled(false);
            }
            agent.SetCurrentAction(this);
            if(animName != "Null") //play anim
            {
                agent.GetComponent<Animator>().Play(animName);
            }
            delayTimer = 0;
            currState = ScriptState.SHOOTING;
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE; //stil cooling down
    }
    public override void UpdateNode(BTAgent agent)
    {
        switch(currState)
        {
            case ScriptState.INACTIVE:
                break;
            case ScriptState.SHOOTING: //shoot projectiles
                delayTimer += Time.deltaTime;
                if(delayTimer < delayDuration){return;}
                //Get start rot of projectileSpread
                Quaternion rot;
                rot = Quaternion.LookRotation(PlayerController.instance.transform.position - agent.transform.position, Vector3.up);
                float offset = arc / 2;
                Vector3 eulerRot = rot.eulerAngles;
                eulerRot.y -= offset;
                Quaternion startRot = Quaternion.Euler(eulerRot);
                float angleInterval = arc / projectileCount;
                for(int i = 0; i < projectileCount; i++) //Create all the projectiles in an arc
                {
                    Vector3 projectileRot = eulerRot + (i * Vector3.up * angleInterval);
                    projectileRot.x = 0;
                    projectileRot.z = 0;
                    Quaternion ProjectileQuat = Quaternion.Euler(projectileRot);
                    Vector3 spawnPos = agent.projectileSpawn.position;
                    spawnPos.y = 1f;
                    GameObject projectile = GameObject.Instantiate(projectilePrefab, spawnPos, ProjectileQuat); //instantiate projectile
                    projectile.GetComponent<Projectile>().Init(speed,acceleration,6,damage,agent.gameObject); //init projectile
                    
                }
                delayTimer = 0;
                currState = ScriptState.COOLDOWN;
                break;
            case ScriptState.COOLDOWN: //cooldown 
                delayTimer += Time.deltaTime;
                if(delayTimer < delayDuration){break;}
                currState = ScriptState.EXIT;
                break;
            case ScriptState.EXIT:
                ExitNode();
                break;
        }
        ExitNode();
        return;
    }
    private void ExitNode() //exit node
    {
        lastTimeUsed = Time.time;
        if(locksMovement)
        {
            agent.SetMovementEnabled(true);
        }
        agent.ClearCurrentAction();
    }
}
