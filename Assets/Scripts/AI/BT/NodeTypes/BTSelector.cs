using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSelector : BTNode
{
    /*  
        Behaviour Tree selector node. Will return true if any of its 
        child nodes return true.
        -Tom
    */
    public BTSelector() : base(){}
    public BTSelector(BTAgent agent, List<BTNode> _childNodes) : base(agent, _childNodes){}
    public override NodeState Evaluate(BTAgent agent)
    {
        foreach(BTNode node in childNodes)
        {
            switch(node.Evaluate(agent))
            {
                case NodeState.FAILURE:
                    continue;
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                default:
                    continue;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }
}
