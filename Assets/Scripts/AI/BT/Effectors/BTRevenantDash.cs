using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRevenantDash : BTNode
{
    /*
     * This node allows the revenant boss to dash in a direction, through walls
     * -Tom
     */
    private float minDistance,maxDistance; //Min max distances for dash
    private Vector3 targetPosition; //target dash pos
    private Vector3 startPosition; //start dash pos
    private float cooldown; //cooldown for fash
    private float timeLastUsed = 0; //last time node used
    private float dashSpeed = 0; //dash speed
    private float maxDuration = 3; //max dash duration
    private float durationTimer = 0; //timer
    private TrailRenderer trail; //dash trail
    private float dashCooldown = 1;
    private enum ScriptState //state of the script
    {
        Inactive,
        DashSetup,
        Dashing,
        Cooldown,
        Exit
    }
    private ScriptState currState; //current state of the script

    //constructor
    public BTRevenantDash(BTAgent _agent, float _maxDistance, float _minDistance, float _cooldown, float _dashSpeed = 20, float _dashCooldown = 1,TrailRenderer _dashTrail = null)
    {
        agent = _agent;
        maxDistance = _maxDistance;
        minDistance = _minDistance;
        cooldown = _cooldown;
        timeLastUsed = -cooldown;
        dashSpeed = _dashSpeed;
        trail = _dashTrail;
        trail.enabled = false;
        dashCooldown = _dashCooldown;
        currState = ScriptState.Inactive;
    }
    public override NodeState Evaluate(BTAgent agent) //evaluate the output of the node
    {
        if(Time.time - timeLastUsed < cooldown){return NodeState.FAILURE;} //cooldown not done
        targetPosition = Vector3.up;
        EQSNode[,] nodes = EQS.instance.GetNodes(); //get all nodes
        float currDistance = EQS.instance.GetNearestNode(agent.transform.position).GetDistance();
        foreach(EQSNode node in nodes) //go over all nodes and get first valid node
        {
            if(!node.GetTraversable()){continue;}
            if(!node.GetLineOfSight()){continue;}
            if(Vector3.Distance(agent.transform.position,node.GetWorldPos()) > maxDistance){continue;}
            if(Vector3.Distance(agent.transform.position,node.GetWorldPos()) < minDistance){continue;}
            targetPosition = node.GetWorldPos();
            targetPosition.y = agent.transform.position.y;
            break;
        }
        if(targetPosition == Vector3.up){return NodeState.FAILURE;} //no valid nodes, return false
        agent.SetCurrentAction(this);
        agent.SetMovementEnabled(false);
        agent.SetLookingAtPlayer(true);
        currState = ScriptState.DashSetup;
        return NodeState.SUCCESS; //return true
    }   
    public override void UpdateNode(BTAgent agent) //Update node
    {
        switch(currState)
        {
            case ScriptState.Inactive: //do nothing
                break;
            case ScriptState.DashSetup: //Build up to the dash, calculate positions, and make agent go through walls
                startPosition = agent.transform.position;
                targetPosition.y = startPosition.y;
                currState = ScriptState.Dashing;
                agent.GetComponent<Collider>().enabled = false;
                agent.GetComponent<Rigidbody>().useGravity = false;
                currState = ScriptState.Dashing;
                durationTimer = 0;
                trail.enabled = true;
                break;
            case ScriptState.Dashing: //dash
                durationTimer += Time.deltaTime;
                if(Vector3.Distance(targetPosition, agent.transform.position) < 1.5f || durationTimer >= maxDuration) //if within range of end or duration reached, exit
                {
                    agent.transform.position = targetPosition;
                    durationTimer = 0;
                    agent.SetVelocity(Vector3.zero);
                    currState = ScriptState.Cooldown;
                    break;
                }
                Vector3 velocity = targetPosition - startPosition;
                velocity.y = 0;
                velocity.Normalize();
                velocity *= dashSpeed;
                agent.SetVelocity(velocity); //dasj
                break;
            case ScriptState.Cooldown: //cooldown
                durationTimer += Time.deltaTime;
                if(durationTimer < 3){break;}
                currState = ScriptState.Exit;
                agent.GetComponent<Collider>().enabled = true;
                agent.GetComponent<Rigidbody>().useGravity = true;
                durationTimer = 0;
                break;
            case ScriptState.Exit: //exit node
                durationTimer += Time.deltaTime;
                if(durationTimer < dashCooldown){break;}
                timeLastUsed = Time.time;
                trail.enabled = false;
                currState = ScriptState.Inactive;
                ExitNode();
                break;
        }
    }
    private void ExitNode() //Exit node
    {
        agent.ClearCurrentAction();
        agent.SetVelocity(Vector3.zero);
        agent.SetLookingAtPlayer(true);
        agent.SetMovementEnabled(true);
        agent.GetComponent<Collider>().enabled = true;
        agent.GetComponent<Rigidbody>().useGravity = true;
    }
}
