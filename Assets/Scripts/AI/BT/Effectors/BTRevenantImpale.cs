using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRevenantImpale : BTNode
{
    /*
     * This node allows the revenant boss to impale the player
     * -Tom
     */
    private float impaleDamage,slamDamage; //damage for the node
    float windupDelay; //windup duratiom
    float impaleRadius,dashSpeed; // radius of impale and speed of dash
    string windupAnim,impaleAnim; 
    float cooldown, windDownDuration;
    float timer;
    float timeLastUsed = 0; //last time the node was activated
    private Vector3 targetPos;
    private float impaleDuration, impaleBindTime, impaleUnbindTime; //duration of impale, time when the player is bound and not bound to the boss
    private Transform impalePosition; //impale transform
    private TrailRenderer trailRend; //trail renderer for the dash
    private bool impaled = false;

    //Constructor
    public BTRevenantImpale(BTAgent _agent, float _impaleDamage, float _slamDamage,float _windupDelay, string _windupAnim, string _impaleAnim,
                            float _impaleRadius, float _impaleDuration, float _impaleBindTime, float _impaleUnbindTime,
                            float _dashSpeed, float _cooldown, float _windDownDuration, Transform _impalePosition, TrailRenderer _dashTrail)
    {
        agent = _agent;
        impaleDamage = _impaleDamage;
        slamDamage = _slamDamage;
        windupDelay = _windupDelay;
        windupAnim = _windupAnim;
        impaleAnim = _impaleAnim;
        impaleRadius = _impaleRadius;
        dashSpeed = _dashSpeed;
        cooldown = _windDownDuration;
        impaleDuration = _impaleDuration;
        impaleBindTime = _impaleBindTime;
        impaleUnbindTime = _impaleUnbindTime;
        impalePosition = _impalePosition;
        trailRend = _dashTrail;
    }
    private enum ScriptState
    {
        Inactive,
        Windup,
        CalculateDestination,
        Dash,
        Impale,
        Exit    
    }
    private ScriptState currState; //current script state
    public override NodeState Evaluate(BTAgent agent) //Evaluate the output of the node
    {
        if(Time.time - timeLastUsed < cooldown){return NodeState.FAILURE;} //still cooling down
        timer = 0;
        agent.GetComponent<Animator>().speed = 1 / windupDelay; //play anims 
        agent.GetComponent<Animator>().Play(windupAnim);
        agent.SetCurrentAction(this); //lock agent behaviour
        agent.SetMovementEnabled(false);
        currState = ScriptState.Windup;
        impaled = false;
        return NodeState.SUCCESS; //return true
    }
    public override void UpdateNode(BTAgent agent)
    {
        switch(currState)
        {
            case ScriptState.Inactive: //do nothing
                break;
            case ScriptState.Windup: //windup to dash
                timer += Time.deltaTime;
                if(timer < windupDelay){break;}
                timer = 0;
                currState = ScriptState.CalculateDestination;
                break;
            case ScriptState.CalculateDestination: //Calculate where the agent needs to dash to
                //Get last traversable eqs node in the direction of the player
                Vector3 direction = PlayerController.instance.transform.position - agent.transform.position;
                direction.Normalize();
                EQSNode currNode = EQS.instance.GetNearestNode(agent.transform.position);
                targetPos = GetTargetPos(); 
                if(targetPos == Vector3.up)
                {
                    currState = ScriptState.Exit; //no valid positions, exit
                    break;
                }
                targetPos.y = agent.transform.position.y;
                agent.GetComponent<Collider>().enabled = false; //make agent go through wals
                agent.GetComponent<Rigidbody>().useGravity = false;
                currState = ScriptState.Dash;
                trailRend.enabled = true;
                agent.SetLookingAtPlayer(false);
                break;
            case ScriptState.Dash: //dash towards the player
                Vector3 dashDirection = targetPos - agent.transform.position;
                dashDirection.Normalize();
                agent.transform.position += dashDirection * dashSpeed * Time.deltaTime; //dash in direction
                if(Vector3.Distance(agent.transform.position, targetPos) < 2) //if close enough to end, exit
                {
                    agent.transform.position = targetPos;
                    currState = ScriptState.Exit;
                    return;
                }
                if(CanImpalePlayer()) //if can impale the player, impale
                {
                    timer = 0;
                    currState = ScriptState.Impale;
                    agent.GetComponent<Animator>().speed = 1;
                    agent.GetComponent<Animator>().Play(impaleAnim);
                }
                //Check for player collision
                break;
            case ScriptState.Impale: //Impale
                timer += Time.deltaTime;
                if(timer < impaleBindTime){break;}
                //Impale
                else if(timer > impaleBindTime && !impaled) //Set up all the impale stuff
                {
                    PlayerController.instance.Damage(impaleDamage,true,false,true);
                    PlayerController.instance.transform.parent = impalePosition;
                    PlayerController.instance.transform.position = impalePosition.position;
                    PlayerController.instance.transform.rotation = impalePosition.rotation;
                    PlayerController.instance.GetComponent<Collider>().enabled = false;
                    PlayerController.instance.GetComponent<Rigidbody>().useGravity = false;
                    PlayerController.instance.GetComponent<Animator>().Play("Impaled");
                    PlayerController.instance.FreezeGameplayInput(true);
                    impaled = true;
                }
                //Release
                else if(timer > impaleUnbindTime && impaled) //release the player
                {
                    PlayerController.instance.Damage(slamDamage,true,false,true);
                    PlayerController.instance.transform.parent = null;
                    PlayerController.instance.transform.rotation = Quaternion.identity;
                    Vector3 pos = PlayerController.instance.transform.position;
                    pos.y = 1;
                    PlayerController.instance.GetComponent<Collider>().enabled = true;
                    PlayerController.instance.GetComponent<Rigidbody>().useGravity = true;
                    PlayerController.instance.ResetPlayerAnimationState();
                    if(PlayerController.instance.GetHealth() > 0) //prevent pushing the player under the map
                    {
                        PlayerController.instance.FreezeGameplayInput(false);
                        PlayerController.instance.transform.position = pos;
                    }
                    impaled = false;
                    timer = 0;
                    currState  =ScriptState.Exit;
                }
                break;
            case ScriptState.Exit: //exit
                timer += Time.deltaTime;
                if(timer < windDownDuration){break;}
                trailRend.enabled = false;
                ExitNode();
                break;
        }
    }
    private Vector3 GetTargetPos()
    {
        int steps = 0;
        //Iterate steps until hitting an untraversable node
        Vector3 startPos = PlayerController.instance.transform.position;
        Vector3 direction = startPos - agent.transform.position;
        direction.Normalize();
        Vector3 currPos;
        EQSNode currNode;
        while(steps < 25)
        {
            steps ++;
            currPos = startPos + direction * steps; 
            currNode = EQS.instance.GetNearestNode(currPos);
            if(currNode == null)
            {
                return Vector3.up;
            }
            else if(!currNode.GetTraversable())
            {
                return currNode.GetWorldPos();
            }
        }
        return Vector3.up;
    }
    private bool CanImpalePlayer() //Checks if the agent is close enough and the player can be hurt
    {
        Vector3 pos = agent.transform.position;
        pos.y = PlayerController.instance.transform.position.y;
        float distance = Vector3.Distance(pos,PlayerController.instance.transform.position);
        if(distance > impaleRadius){return false;}
        if(PlayerController.instance.IsInvulnerable()){return false;}
        return true;
    }
    private void ExitNode() //exit node
    {
        timeLastUsed = Time.time;
        currState = ScriptState.Inactive;
        agent.GetComponent<Collider>().enabled = true;
        agent.GetComponent<Rigidbody>().useGravity = true;
        agent.GetComponent<Animator>().speed = 1;
        agent.GetComponent<Animator>().Play("Idle");
        agent.SetLookingAtPlayer(true);
        agent.SetMovementEnabled(true);
        agent.ClearCurrentAction();
    }
}
