using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRetreat : BTNode
{
    /*
     * This node causes agents to retreat away from the player
     * -Tom
     */
    private bool maintainLineOfSight; //whether they need to maintain line of sight with the player
    private float maxDistance, speed; //max distance from player and retreat speed
    private Vector3 targetPos; //target position
    private float timer = 0;
    private float maxDuration;

    //constructor
    public BTRetreat(BTAgent _agent,bool _maintainLineOfSight, float _maxDistanceFromAgent, float _speed, float _maxDuration = 5)
    {
        agent = _agent;
        maintainLineOfSight = _maintainLineOfSight;
        if(_maxDistanceFromAgent > 3)
        {
            maxDistance = 3;
        }
        else
        {
            maxDistance = _maxDistanceFromAgent;
        }
        speed = _speed;
        maxDuration = _maxDuration;
    }
    public override NodeState Evaluate(BTAgent agent) //evaluate node output
    {
        //Get all nodes that have the relevant line of sight
        EQSNode[,] allNodes = EQS.instance.GetNodes();
        List<EQSNode> nodesToSort = new List<EQSNode>();
        List<EQSNode> idealNodes = new List<EQSNode>();
        List<EQSNode> validNodes = new List<EQSNode>();
        foreach(EQSNode node in allNodes)
        {
            if(!node.GetTraversable()){continue;}
            if(Vector3.Distance(node.GetWorldPos(), agent.transform.position) >= maxDistance){continue;}
            if(node.GetTraversable() != maintainLineOfSight)
            {
                validNodes.Add(node);
                continue;
            }
            idealNodes.Add(node);
        }
        //Pick either ideal or valid based on choices
        if(idealNodes.Count == 0)
        {
            nodesToSort = validNodes;
        }
        else if(validNodes.Count == 0)
        {
            return NodeState.FAILURE;
        }
        else
        {
            nodesToSort = idealNodes;
        }
        float distance = 0;
        float highestDistance = 0;
        Vector3 target = Vector3.down;
        foreach(EQSNode node in nodesToSort) //Sort all nodes
        {
            distance = node.GetDistance();
            if(distance < 1){continue;}
            if(distance > highestDistance)
            {
                highestDistance = distance;
                target = node.GetWorldPos();
            }
        }
        if(target == Vector3.down){ return NodeState.FAILURE;} //not valid positions
        target.y = agent.transform.position.y;
        targetPos = target;
        timer = 0;
        agent.SetCurrentMovement(this);
        return NodeState.SUCCESS; //return true
    }
    public override void UpdateNode(BTAgent agent) //Update node
    {
        timer += Time.deltaTime;
        if(timer >= maxDuration) //exit node
        {
            ExitNode(agent);
            return;
        }
        Vector3 velocity = targetPos - agent.transform.position; //move in direction of target pos
        velocity.y = 0;
        velocity.Normalize();
        agent.SetVelocity(velocity);
        if(Vector3.Distance(agent.transform.position, targetPos) < 0.15f){ExitNode(agent);} //exit if close enough
    }
    private void ExitNode(BTAgent agent) //exit node
    {
        agent.ClearCurrentMovement();
        return;
    }
}
