using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTHasLineOfSight : BTNode
{
    /*
        Behaviour Tree conditional node, returns true if the agent
        has a line of sight on the player or not. 
        -Tom
    */
    private bool invertResult; //Whether or not to invert the output of this node
    public BTHasLineOfSight(BTAgent _agent, bool _invertResult = false) //Constructor
    {
        agent = _agent; //Target agent
        invertResult = _invertResult; //whether to invert the result
    }
    public override NodeState Evaluate(BTAgent agent) //Evaluates the output of the node
    {
        //Get nearest node to agent
        EQSNode nearestNode = EQS.instance.GetNearestNode(agent.transform.position); //Gets nearest EQS node
        bool lineOfSight = nearestNode.GetLineOfSight(); //Get the line of sight value of the EQS node
        if(invertResult) //invert
        {
            invertResult = invertResult ? false : true;
        } 
        if(nearestNode.GetLineOfSight()) //Return true
        {
            //Return true
            return NodeState.SUCCESS;
        }
        //return false
        return NodeState.FAILURE; //return false
    }
}
