using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRandWalk : BTNode
{
    /*
     * movement node that makes the agent move in a random direction
     * -Tom
     */
    private float minDistanceFromAgent, maxDuration,durationTimer, minDistanceFromOldPos; //paramaters for rand direction
    private Vector3 targetPos; //target pos
    private Vector3 oldPos = Vector3.zero; //start pos


    //Constructor
    public BTRandWalk(BTAgent _agent, float _minDistanceFromAgent, float _maxDuration, float _minDistanceFromOldPos = 5)
    {
        agent = _agent;
        minDistanceFromAgent = _minDistanceFromAgent;
        maxDuration = _maxDuration;
        minDistanceFromOldPos = 10;
    }
    public override NodeState Evaluate(BTAgent agent) //evaluate the output of the node
    {
        targetPos = GetTargetPos(); //get target location
        agent.SetCurrentMovement(this); 
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        if(targetPos == Vector3.zero){ExitNode(); return;}
        durationTimer += Time.deltaTime;
        if(durationTimer >= maxDuration){ExitNode(); return;}
        Vector3 velocity = targetPos - agent.transform.position;
        velocity.y = 0;
        if(velocity.magnitude < 0.25f){ExitNode(); return;}
        velocity.Normalize(); //move in direction
        agent.SetVelocity(velocity);
    }
    private Vector3 GetTargetPos() //Get target pos
    {
        EQSNode[,] nodes = EQS.instance.GetNodes();
        float score;
        float highscore = 0;
        Vector3 agentPos = agent.transform.position;
        agentPos.y = 1;
        Vector3 pos;
        EQSNode bestNode = null;
        foreach(EQSNode node in nodes) //get all nodes
        {
            if(!node.GetTraversable()){continue;}
            pos = node.GetWorldPos();
            pos.y = 1;
            if(Vector3.Distance(pos,oldPos) < minDistanceFromOldPos){continue;}
            if(Vector3.Distance(pos,agentPos) < minDistanceFromAgent){continue;}
            score = 0;
            score += 1/Vector3.Distance(pos,agentPos);
            if(score > highscore)
            {
                highscore = score; //get best node
                bestNode = node;
            }
        }
        if(bestNode != null)
        {
            return bestNode.GetWorldPos(); //return best node
        }
        return Vector3.zero; 
    }
    private void ExitNode() //exit node
    {
        agent.ClearCurrentMovement();
        agent.SetVelocity(Vector3.zero);
        durationTimer = 0;
    }
}
