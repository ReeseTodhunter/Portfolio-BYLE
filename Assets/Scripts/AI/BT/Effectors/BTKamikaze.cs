using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTKamikaze : BTNode
{
    /*
     * This effector node causing the agent to kill themself
     */
    public override NodeState Evaluate(BTAgent agent) //evaluate
    {
        agent.SetCurrentAction(this);
        return NodeState.SUCCESS; //return true
    }
    public override void UpdateNode(BTAgent agent)
    {
        agent.Damage(99999,true,false); //kill agent
        agent.ClearCurrentAction();
        return;
    }
}
