using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRandSelector : BTNode
{
    /*
     * Functions the same as BTSelector, however it randomly selects its child nodes rather than in order
     */
    public BTRandSelector() : base(){}
    public BTRandSelector(BTAgent agent, List<BTNode> _childNodes) : base(agent, _childNodes){}
    public override NodeState Evaluate(BTAgent agent)
    {
        int x = 0;
        List<BTNode> temp = new List<BTNode>();
        temp.AddRange(childNodes);
        while(temp.Count > 0 || x > 100) //Pick random nodes
        {
            x++;
            int index = Random.Range(0,temp.Count);
            BTNode currNode = temp[index];
            switch(currNode.Evaluate(agent))
            {
                case NodeState.FAILURE:
                    break;
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                default:
                    break;
            }
            temp.RemoveAt(index);
        }
        if(x >= 100) //prevent infinite loop
        {
            Debug.LogWarning("Warning : BTRandSelector overflow");
        }
        state = NodeState.FAILURE;
        return state;
    }
}
