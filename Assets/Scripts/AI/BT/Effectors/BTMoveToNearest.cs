using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTMoveToNearest : BTNode
{
    /*
     * This effector node makes the agent move to the node closest to the player that the agent can reach
    */
    private bool lineOfSight; //requires line of sight
    private float maxDistance; //max distance from player
    private EQSNode bestNode; //best node, target to move to
    private List<Vector3> path = new List<Vector3>(); //path
    private float maxDuration, timer; //max duration of node
    private Vector3 targetPos, endPos, currPos; //target locations
    private int pathIter = 0; //current point on the path
    private float speed; //speed

    //constructor
    public BTMoveToNearest(BTAgent _agent,bool _lineOfSight, float _maxDistanceFromAgent, float _speed = 3,float _maxDuration = 5)
    {
        agent = _agent;
        lineOfSight = _lineOfSight;
        maxDistance = _maxDistanceFromAgent;
        speed = _speed;
        maxDuration = _maxDuration;
    }
    public override NodeState Evaluate(BTAgent agent) //evalulate node
    {
        path = new List<Vector3>();
        timer = 0;
        bestNode = null;
        EQSNode[,] nodes = EQS.instance.GetNodes();
        float closest = 999, distance = 0;
        foreach(EQSNode node in nodes) //Get all nodes that are valid and within distance to player
        {
            if(!node.GetTraversable()){continue;}
            if(node.GetDistance() > maxDistance){continue;}
            if(lineOfSight != node.GetLineOfSight())
            {
                continue;
            }
            distance = Vector3.Distance(agent.transform.position, node.GetWorldPos());
            if(distance < closest)
            {
                closest = distance;
                bestNode = node;
            }
        }
        if(bestNode == null){return NodeState.FAILURE;} //if no valid nodes, return false
        agent.SetCurrentMovement(this);
        path = agent.gameObject.GetComponent<EQSPathfinder>().GetPathToPosition(bestNode.GetWorldPos(), 30); //try pathfind to best position
        pathIter = 0;
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent)
    {
        //Exit conditions
        if(bestNode == null || path == null) //if any are null, exit
        {
            ExitNode(agent);
            return;
        }
        timer += Time.deltaTime;
        if(timer >= maxDuration) // if over max duration, exit
        {
            ExitNode(agent);
            return;
        }
        if(pathIter >= path.Count) //if end of path, exit
        {
            ExitNode(agent);
            return;
        }
        
        //Move towards current node in path
        EQSNode nearestNode = EQS.instance.GetNearestNode(agent.transform.position);
        Vector3 targetPos = path[pathIter];
        agent.UpdateGizmos(path);
        agent.UpdateGizmos(targetPos);
        Vector3 velocity = targetPos - agent.transform.position;
        velocity.y = 0;
        velocity.Normalize();
        velocity *= speed;
        agent.SetVelocity(velocity);

        //Check if required to iterate through path
        if(Vector3.Distance(nearestNode.GetWorldPos(),targetPos) < 0.05f)
        {
            pathIter++;
        }

    }
    private void ExitNode(BTAgent agent) //exit 
    {
        agent.ClearCurrentMovement();
        return;
    }
}
