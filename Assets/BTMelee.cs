using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTMelee : BTNode
{
    private float maxDistance;
    private float cooldown, timeSinceLastUsed;
    private float damage;
    public BTMelee(BTAgent _agent,float maxDistance, float cooldown, float damage)
    {
        agent = _agent;
        this.maxDistance = maxDistance;
        this.cooldown = cooldown;
        this.damage = damage;
    }
    public override NodeState Evaluate(BTAgent agent)
    {
        if(Time.time - timeSinceLastUsed < cooldown) { return NodeState.FAILURE; }
        if(Vector3.Distance(agent.transform.position,PlayerController.instance.transform.position) > maxDistance) { return NodeState.FAILURE; }
        PlayerController.instance.Damage(damage);
        timeSinceLastUsed = Time.time;
        return NodeState.SUCCESS;
    }
}
