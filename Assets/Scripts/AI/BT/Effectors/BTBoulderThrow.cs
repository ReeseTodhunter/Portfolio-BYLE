using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class BTBoulderThrow : BTNode
{
    /*
        Behaviour Tree Effector node. Boulder throw attack for the bruiser enemy. Can either throw
        a projectile directly at the player or throw it in an arc if the player is behind cover
        -Tom
    */
    private float decoupleTime, speed, timeBetweenEffects , totalDuration, timer, damageRadius, damage; 
    private GameObject throwVFX,impactVFX; //VFX for attack
    private GameObject throwObject,impactObject; //object to throw
    private string animationName; //anim to play
    private bool directShot = false; //Whether or not throwing the projectile directly at the player
    private GameObject projectilePrefab; //prefab for the projectile
    private GameObject projectile; //projectile reference
    private enum scriptState //States the node can be in
    {
        throwing,
        flying,
        falling,
        exit
    }
    private scriptState currState = scriptState.exit; //current state

    //Constructor for throw
    public BTBoulderThrow(BTAgent _agent,float _decoupleTime, float _speed, float _timeBeforeFall, float _totalDuration = 6,float _damageRadius = 3, float _damage = 20, string _animName = "Throw")
    {
        agent = _agent;
        decoupleTime = _decoupleTime;
        speed = _speed;
        timeBetweenEffects = _timeBeforeFall;
        damageRadius = _damageRadius;
        damage = _damage;
        totalDuration = _totalDuration;
        throwVFX = Resources.Load("VFX/Charger/BoulderThrow") as GameObject;
        impactVFX = Resources.Load("VFX/Charger/BoulderImpact") as GameObject;
        animationName = _animName;
        directShot = false;
    }
    //Constructor for direct throw
    public BTBoulderThrow(BTAgent _agent, string _projectileFileLocation, float _decoupleTime, float _speed, float _damage, float _totalDuration = 4,string _animName = "DirectThrow")
    {
        agent = _agent;
        directShot = true;
        projectilePrefab = Resources.Load(_projectileFileLocation) as GameObject;
        decoupleTime = _decoupleTime;
        damage = _damage;
        speed = _speed;
        totalDuration = _totalDuration;
        animationName = _animName;
    }
    public override NodeState Evaluate(BTAgent agent) //Evaluate the result of the node
    {
        if(directShot && !EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight()) //if direct shot and has no line of sight on the player return false
        {
            return NodeState.FAILURE;
        }
        if(directShot) //direct shot
        {
            projectile = GameObject.Instantiate(projectilePrefab,agent.projectileSpawn.position,agent.projectileSpawn.rotation); //Instantiate projectile
            projectile.transform.parent = agent.projectileSpawn; //Set projectiles parent as the bruisers hand, so that it can use it in animation
            projectile.GetComponent<BasicProjectile>().Init(0,0,10,damage,"Enemy"); //init projectile script
            projectile.GetComponent<Collider>().enabled = false; // disable collider for projectile
        }
        else //arc shot
        {
            CreateVFX(); //Creat vfx 
            impactObject.SetActive(false); //hide object
            throwObject.GetComponent<VisualEffect>().Play(); //Begin visual effect for projectiel
            throwObject.transform.parent = agent.projectileSpawn; //Set parent to brusier hand
        }
        timer = 0; //reset timer
        agent.SetCurrentAction(this); //lock action tree
        agent.GetComponent<Animator>().Play(animationName); // play animation
        agent.SetMovementEnabled(false); //lock movement
        currState = scriptState.throwing; // set state to throwing
        return NodeState.SUCCESS; //return true
    }
    public override void UpdateNode(BTAgent agent)
    {
        timer += Time.deltaTime;
        switch(currState)
        {
            case scriptState.throwing:
                if(timer < decoupleTime) //If timer hasn't reach decoupl, return
                {
                    break;
                }
                if(directShot) //direct shot
                {
                    projectile.transform.parent = null; //uncouple from hand
                    projectile.transform.LookAt(PlayerController.instance.transform.position); //look at player
                    projectile.transform.position = agent.projectileSpawn.position + agent.projectileSpawn.forward; //move the projectile towards the player
                }
                else //arc shot
                {
                    throwObject.transform.parent = null; //uncouple from hand
                    throwObject.transform.rotation = agent.projectileSpawn.rotation; //set rotation
                }
                currState = scriptState.flying; //next state
                agent.SetMovementEnabled(true); //unlock movement
                break;
            case scriptState.flying:
                if(directShot) //direct shot
                {
                    projectile.GetComponent<BasicProjectile>().speed = speed; //set speed
                    projectile.GetComponent<Collider>().enabled = true; //enabled collision
                    currState = scriptState.falling; //next state
                    break;
                }
                if(throwObject != null)
                {
                    throwObject.transform.position += throwObject.transform.forward * Time.deltaTime * speed; //Move forward
                }
                if(timer >= timeBetweenEffects) //Enabled impact
                {
                    impactObject.SetActive(true);
                    impactObject.transform.position = PlayerController.instance.transform.position;
                    currState = scriptState.falling;
                }
                break;
            case scriptState.falling:
                if(timer >= totalDuration) //wait until duration passed, then exit node
                {
                    currState = scriptState.exit;
                }
                break;
            case scriptState.exit:
                ExitNode();
                break;
        }
    }
    public override void DeInitialiseNode(BTAgent agent) //Delete all vfx when the agent dies
    {
        DestroyVFX();
    }
    private void ExitNode() //exit node
    {
        agent.ClearCurrentAction(); //unlock action tree
        DestroyVFX(); //destroy any vfx
        return;
    }
    private void CreateVFX() //create vfx objecs
    {
        DestroyVFX();
        throwObject = GameObject.Instantiate(throwVFX,agent.projectileSpawn.position,agent.projectileSpawn.rotation);
        impactObject = GameObject.Instantiate(impactVFX,agent.transform.position,Quaternion.identity);
    }
    private void DestroyVFX() //delete vfx objects
    {
        if(throwObject != null){GameObject.Destroy(throwObject);}
        if(impactObject != null){GameObject.Destroy(impactObject);}
    }
}
