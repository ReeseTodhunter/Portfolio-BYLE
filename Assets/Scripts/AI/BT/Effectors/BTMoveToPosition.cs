using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTMoveToPosition : BTNode
{
    /*
        Large behaviour tree effector node. Finds the optimal position for an
        agent to move to, and moves it there. Uses pathfinding if necessary. 
        Pathfinding is calculated from the EQSPathfinder.cs. Quite complex, 
        most documentation is further down, alongside relevant code.
        -Tom
    */
    private float optimalDistance; 
    private float pathUpdateTimer = .5f, timer = 0; //Timer 
    private EQSNode[,] nodes = null;
    private float speed = 3;
    private List<Vector3> path = new List<Vector3>();
    private int pathIter = 0;
    public enum positionType{lineOfSight, noLineOfSight, either}
    private positionType targetType;
    private bool realTimePathfinding; 
    private Vector2Int lastPlayerPos;
    private Vector3 targetPosition;
    //Constructor
    public BTMoveToPosition(float _optimalDistance, positionType positionType = positionType.either, float _speed = 3, bool _realTimePathfinding = true)
    {
        optimalDistance = _optimalDistance;
        targetType = positionType;
        speed = _speed;
        realTimePathfinding = _realTimePathfinding;
    }
    public override NodeState Evaluate(BTAgent _agent)
    {
        agent = _agent;
        //Lock agent brain into running this action
        //Check if the move action has a current path, if not, find a new one
        //Else find new path
        //Find optimal node to move towards, closest is best
        EQSNode bestNode = GetOptimalNode();
        if(bestNode == null){return NodeState.FAILURE;}
        path = new List<Vector3>();
        path = GetPath();
        if(path == null){return NodeState.FAILURE;}
        agent.SetCurrentMovement(this);
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent _agent) //update node
    {
        timer += Time.deltaTime;
        Vector2Int currPlayerPos = EQS.instance.WorldToEQSCoords(PlayerController.instance.transform.position); //Get player pos
        if(currPlayerPos != lastPlayerPos && timer > pathUpdateTimer && realTimePathfinding) //Get path
        {
            timer = 0;
            lastPlayerPos = currPlayerPos;
            pathIter = 1;
            List<Vector3> newPath = GetPath();
            path = newPath;
        }
        if(path == null) //if path null exit
        {
            ExitNode(); return;
        }
        if(pathIter >= path.Count) //if at end of path, exit
        {
            ExitNode();
            return;
        }
        EQSNode nearestNode = EQS.instance.GetNearestNode(agent.transform.position); //get path
        
        if(Mathf.Abs(nearestNode.GetDistance() - optimalDistance) < 2) //exit node
        {
            if(nearestNode.GetLineOfSight() && targetType != positionType.noLineOfSight)
            {
                ExitNode();
                return;
            }
            if(!nearestNode.GetLineOfSight() && targetType != positionType.lineOfSight)
            {
                ExitNode();
                return;   
            }
        }
        //Iterate through pathfinding waypoints
        targetPosition = path[pathIter];
        targetPosition.y = agent.transform.position.y;
        agent.UpdateGizmos(targetPosition);
        
        Vector3 velocity = targetPosition - agent.transform.position;
        velocity.y = 0;
        velocity.Normalize();
        velocity *= speed;
        agent.SetVelocity(velocity);
        if(Vector3.Distance(agent.transform.position, targetPosition) < 0.25f)
        {
            //Move onto next waypoint in path
            pathIter++;
        }

    }
    private void ExitNode()
    {
        agent.ClearCurrentMovement();
        return;
    }
    private EQSNode GetOptimalNode() //Get best node
    {
        nodes = EQS.instance.GetNodes();
        float distance = 0;
        float nodeScore = 0, bestNodeScore = 0;
        float proximity = 0;
        EQSNode bestNode = null;
        foreach(EQSNode node in nodes) //get all nodes
        {
            nodeScore = 0;
            if(!node.GetTraversable()){continue;}
            if( node.GetLineOfSight() && (targetType == positionType.noLineOfSight)){continue;}
            if(!node.GetLineOfSight() && targetType == positionType.lineOfSight){continue;}
            distance = Vector3.Distance(agent.transform.position, node.GetWorldPos());
            nodeScore += 1/distance; //Closer to agent the better
            proximity = Mathf.Abs(node.GetDistance() - optimalDistance);
            nodeScore += 1/proximity;
            if(nodeScore > bestNodeScore)
            {
                bestNode = node;
                bestNodeScore = nodeScore;
            }
        }
        return bestNode;
    }
    private List<Vector3> GetPath() //Get path to location
    {
        EQSNode bestNode = GetOptimalNode();
        if(bestNode == null){return null;}
        List<Vector3> newPath = new List<Vector3>();
        float distance = Vector3.Distance(agent.transform.position, bestNode.GetWorldPos());
        if(!agent.gameObject.TryGetComponent(out EQSPathfinder pathfinder)){Debug.Log("bro add EQSPathfinder to this ai pls"); return null;}

        newPath = agent.gameObject.GetComponent<EQSPathfinder>().GetPathToPosition(bestNode.GetWorldPos(),30);
        agent.UpdateGizmos(newPath);
        return newPath;
    }
}
