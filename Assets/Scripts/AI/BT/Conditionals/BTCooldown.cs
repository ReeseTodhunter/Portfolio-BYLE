using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCooldown : BTNode
{
    /*
        Behaviour Tree conditional node, returns true or false based
        on whether the set duration of time has elapsed
        -Tom
    */
    private float lastTimeEvaluated = 0; //time since last checked
    private float cooldownDuration = 0; //cooldown duration
    public BTCooldown(float _coolDownDuration = 1) //Constructor
    {
        cooldownDuration = 1; //set cooldown duration
    }
    public override NodeState Evaluate(BTAgent agent)
    {
        bool tooSoon = Time.time - lastTimeEvaluated < cooldownDuration ? true : false; //Whether or not its too soon to return true
        if (!tooSoon)
        {
            lastTimeEvaluated = Time.time; //Reset last time since last checked
        }
        return tooSoon ? NodeState.FAILURE : NodeState.SUCCESS; //return state
    }
}
