using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTDelayShot : BTNode
{
    /*
        Behaviour Tree effector node. Allows the agent to fire a projectile after a delay
        -Tom
    */
    private GameObject prefab; //projectile prefab
    private bool lockMovement; //whether or not to lock movement
    private float velocity, damage; //velocity and damage of projectile
    private float waitDuration, waitTimer = 0; //wait time of node
    private string animName; //animation name

    //constructor
    public BTDelayShot(string projectilePath, float _velocity, float _damage, bool _lockMovement = false, float _waitDuration = 2, string _animName = "Null")
    {
        prefab = Resources.Load(projectilePath) as GameObject;
        velocity = _velocity;
        damage = _damage;
        lockMovement = _lockMovement;
        animName = _animName;
        waitDuration = _waitDuration;
    }

    public BTDelayShot(GameObject projectile, float _velocity, float _damage, bool _lockMovement = false, float _waitDuration = 2, string _animName = "Null")
    {
        prefab = projectile;
        velocity = _velocity;
        damage = _damage;
        lockMovement = _lockMovement;
        animName = _animName;
        waitDuration = _waitDuration;
    }

    public override NodeState Evaluate(BTAgent agent) //evaluate output of node
    {
        agent.SetCurrentAction(this); //lock action tree
        if(lockMovement) //lock movement if necessary
        {
            agent.SetMovementEnabled(false);
        }
        waitTimer = 0;
        if(animName != null) //Play anim
        {
            agent.GetComponent<Animator>().Play(animName);
        }
        return NodeState.SUCCESS; //return true
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        waitTimer += Time.deltaTime;
        if(waitTimer < waitDuration){return;} //return if wait not over
        GameObject projectile = GameObject.Instantiate(prefab, agent.projectileSpawn.position, agent.projectileSpawn.rotation); //instantiate projectile
        projectile.transform.LookAt(PlayerController.instance.transform.position); //make projectile face player
        BasicProjectile projectileScript = projectile.GetComponent<BasicProjectile>(); //init projectile
        projectileScript.Init(velocity,0,5,damage,"Enemy");
        //Exit node
        agent.ClearCurrentAction();
        agent.SetMovementEnabled(true);
        return;
    }
}
