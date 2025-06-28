using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWithinRange : BTNode
{
    /*
        Behaviour Tree conditional node. Returns true if the agent is within
        a certain range of the player. Has a custom constructor to tweak these
        values to get the required functionality. Can invert the bool return to
        turn this script into a "too close" check rather than a close enough.
        -Tom
    */
    private float maxDistance, forgivenessRange; //Max distance the agent can be from the player and a forgiveness range, so if they're nearly within range its valid
    private bool reverseBoolValue = false; //Whether or not to reverse the ouput of this node
    public BTWithinRange(BTAgent _agent, float _maxDistance, float _forgivenessRange, bool _reverseBoolValue = false) //Constructor
    {
        agent = _agent; //Agent to check
        maxDistance = _maxDistance; // Max distance
        forgivenessRange = _forgivenessRange; //Forgiveness range
        reverseBoolValue = _reverseBoolValue; //Whether or not to reverse the output of this node
    }
    public override NodeState Evaluate(BTAgent agent) //Evaluate the ouput of this node
    {
        EQSNode closestNode = EQS.instance.GetNearestNode(agent.transform.position); //Get nearest EQS Nodde
        float distance = closestNode.GetDistance(); //Get distance value of that node
        if(distance <= maxDistance + forgivenessRange) //Check whether the EQS Node is within range
        {
            if(reverseBoolValue){return NodeState.FAILURE;} //reverse value
            return NodeState.SUCCESS; //return true
        }
        if(reverseBoolValue){return NodeState.SUCCESS;} //reverse value
        return NodeState.FAILURE; //return false

    }
}
