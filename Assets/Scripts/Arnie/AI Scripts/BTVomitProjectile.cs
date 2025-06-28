using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTVomitProjectile : BTNode
{
    /*
        Basic projectile firing behaviour for Behaviour tree agents
    */
    private GameObject prefab;
    private bool lockMovement, needsLineOfSight;
    private float velocity, damage;
    private string animName;
    private float delay, delayTimer = 0;
    public BTVomitProjectile(GameObject _projectile, float _velocity, float _damage, bool _lockMovement = false, bool _needsLineOfSight = true, string _animName = "", float _delay = 0)
    {
        prefab = _projectile;
        velocity = _velocity;
        damage = _damage;
        lockMovement = _lockMovement;
        needsLineOfSight = _needsLineOfSight;
        animName = _animName;
        delay = _delay;
    }
    public override NodeState Evaluate(BTAgent agent)
    {
        if (EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight())
        {
            agent.SetCurrentAction(this);
            delayTimer = 0;
            if (animName != "")
            {
                agent.GetComponent<Animator>().Play(animName);
            }
            return NodeState.SUCCESS;
        }
        else
        {
            if (!needsLineOfSight)
            {
                agent.SetCurrentAction(this);
                delayTimer = 0;
                if (animName != "")
                {
                    //agent.GetComponent<Animator>().Play(animName);
                }
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
    public override void UpdateNode(BTAgent agent)
    {
        if (agent.projectileSpawn == null)
        {
            agent.ClearCurrentAction();
            Debug.Log("Agent projectile spawn transform not set up, thats cringe");
            return;
        }
        delayTimer += Time.deltaTime;
        if (delayTimer <= delay) { return; }

        GameObject projectile = GameObject.Instantiate(prefab, agent.projectileSpawn.position, agent.projectileSpawn.rotation);
        projectile.transform.LookAt(PlayerController.instance.transform.position);
        Grenade projectileScript = projectile.GetComponent<Grenade>();
        projectileScript.Init(velocity, 0, 10, damage, agent.gameObject);

        //Exit node
        agent.ClearCurrentAction();
        return;
    }
}

