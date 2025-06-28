using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTShootProjectile : BTNode
{
    /*
        Basic projectile firing behaviour for Behaviour tree agents
        -Tom
    */
    private GameObject prefab; //projectile prefab
    private bool needsLineOfSight; //whether or not the agent needs a line of sight to shoot
    private float velocity, damage; //velocity and damage of the projectile
    private string animName; //name of shoot animation
    private float delay, delayTimer = 0;

    //constructor
    public BTShootProjectile(Projectiles _projectile, float _velocity, float _damage, bool _lockMovement = false, bool _needsLineOfSight = true, string _animName = "", float _delay = 0)
    {
        prefab = ProjectileLibrary.instance.GetProjectile(_projectile);
        velocity = _velocity;
        damage = _damage;
        needsLineOfSight = _needsLineOfSight;
        animName = _animName; 
        delay = _delay;
    }
    public override NodeState Evaluate(BTAgent agent) //Evaluate node ouput
    {

        //Determine if the agent can shoot or not, and play the anim if they can
        if(EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight())
        {
            agent.SetCurrentAction(this);
            delayTimer = 0;
            if(animName != "")
            {
                agent.GetComponent<Animator>().Play(animName);
            }
            return NodeState.SUCCESS;
        }
        else
        {
            if(!needsLineOfSight)
            {
                agent.SetCurrentAction(this);
                delayTimer = 0;
                if(animName != "")
                {
                    agent.GetComponent<Animator>().Play(animName);
                }
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
    public override void UpdateNode(BTAgent agent) //Update behaviour
    {
        delayTimer += Time.deltaTime;
        if(delayTimer <= delay){return;}
        
        GameObject projectile = GameObject.Instantiate(prefab, agent.projectileSpawn.position, agent.projectileSpawn.rotation); //Instantiate projectile

        projectile.transform.LookAt(PlayerController.instance.transform.position);
        Projectile projectileScript = projectile.GetComponent<Projectile>(); //init projectile
        projectileScript.Init(velocity, 0, 10, damage, agent.gameObject);

        //Exit node
        agent.ClearCurrentAction();
        return;
    }
}
