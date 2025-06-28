using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTDirectMove : BTNode
{
    /*
        Behaviour Tree effector node, moves the agent in a direction until
        the set fail state is reached
        -Tom
    */
    private float desiredDistance; //desired distance to the player
    private float speed; //move speed
    private float minDistance; //min distance to the player
    private float maxDuration; //max duration of node
    private float timer = 0;
    public enum FailState //fail states of the node
    {
        NONE,
        SIGHTLOST,
        DURATION,
        ALL
    }
    private FailState failState; //current fail state

    //constructor
    public BTDirectMove(float _desiredDistance, float _speed, FailState _failState = FailState.NONE,float _minDistance = 0,  float _maxDuration = 4)
    {
        desiredDistance = _desiredDistance;
        speed = _speed;
        failState = _failState;
        minDistance = _minDistance;
        maxDuration = _maxDuration;
    }
    public override NodeState Evaluate(BTAgent _agent) //Evaluate output of the node
    {
        agent = _agent;
        agent.SetCurrentMovement(this); //lock movement tree to this node
        timer = 0;
        return NodeState.SUCCESS; //retun true
    }
    public override void UpdateNode(BTAgent _agent) //update node
    {
        timer += Time.deltaTime;
        if(timer > maxDuration && (failState == FailState.DURATION || failState == FailState.ALL)) //if timer over max duration, exit
        {
            agent.SetVelocity(Vector3.zero);
            ExitNode();
        }
        float distance = Vector3.Distance(agent.transform.position, PlayerController.instance.transform.position);
        if(distance <= minDistance) //if distance at min distance, exit
        {
            agent.SetVelocity(Vector3.zero);
            ExitNode();
        }
        if(!EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight() && (failState == FailState.SIGHTLOST || failState == FailState.ALL)) //if line of sight lost, exit
        {            
            agent.SetVelocity(Vector3.zero);
            ExitNode();
        }

        //move in direction
        Vector3 velocity = PlayerController.instance.transform.position - agent.transform.position;
        velocity.Normalize();
        velocity.y = 0;
        velocity *= speed;
        agent.SetVelocity(velocity);
    }
    private void ExitNode() //exit node
    {
        agent.ClearCurrentMovement(); //unlcok movement tree
    }
}
