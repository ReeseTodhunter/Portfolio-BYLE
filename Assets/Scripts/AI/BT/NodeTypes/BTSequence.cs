using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequence : BTNode
{
    /*
        Behaviour Tree sequence node. Will only return true if all of 
        its child nodes return true.
        -Tom
    */
    public BTSequence() : base(){}
    public BTSequence(BTAgent agent,List<BTNode> _childNodes): base(agent,_childNodes){}
    public override NodeState Evaluate(BTAgent agent)
    {
        bool anyChildIsRunning = false;
        foreach(BTNode node in childNodes)
        {
            switch(node.Evaluate(agent))
            {
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.SUCCESS:
                    continue;
                case NodeState.RUNNING:
                    anyChildIsRunning = true;
                    continue;
                default:
                    state = NodeState.SUCCESS;
                    return state;
            }
        }

        state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}
