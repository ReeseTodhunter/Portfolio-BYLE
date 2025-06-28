using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTPounce : BTNode
{
    /*
        Behaviour tree effector. This node allows the agent to perfrom a "pounce"
        attack. As of 23w02, this action works alongside
        the confidence system, which itself probably wont last long before being replaced.
        This script lerps the agent along a line towards and past the player, 
        giving the appereance of a dash / pounce attack. Contains no obstacle detection
        as of 23w02, so the agent can dash through obstacles.
        -Tom
    */
    private float maxPounceDistance = 2; //max distance 
    private float windUpDuration = 0.5f, windUpTimer = 0; //wind up timers
    private float pounceSpeed = 0; //pounce speeds
    private float duration = 2, durationTimer = 0; //duration timers
    private float overshootDistance = 0; //how far the agent can overshoot the player
    private float damage = 0; //pounce damage
    private Vector3 startPos, endPos; //positions
    private float hitRange = 0.75f; //radius of grab
    private bool leaveTrail = false, buildupParticles = false; //whether to use vfx or not
    private GameObject trail, buildup; //vfx objects
    private enum actionState{ windup, calculateTarget, dash, grab, chokeslam}; //script state
    private actionState currentState = actionState.windup; //current state
    private float chokeslamDuration = 1.25f; //chokeslam duration
    private float chokeslamTimer = 0; //timer

    //Constructor
    public BTPounce(float _maxPounceDistance,float _windUpDuration, float _pounceSpeed, float _overshootDistance = 10, float _damage = 25, float _hitRange = 1.5f, bool _leaveTrail = false, bool _buildUpParticles = false)
    {
        maxPounceDistance = _maxPounceDistance;
        windUpDuration = _windUpDuration;
        pounceSpeed = _pounceSpeed;
        overshootDistance = _overshootDistance;
        damage = _damage;
        hitRange = _hitRange;
        leaveTrail = _leaveTrail;
        buildupParticles = _buildUpParticles;
    }
    public override NodeState Evaluate(BTAgent _agent) //Evaluate output
    {
        agent = _agent;
        currentState = actionState.windup;
        windUpTimer = 0;
        durationTimer = 0;
        chokeslamTimer = 0;
        EQSNode nearestNode = EQS.instance.GetNearestNode(agent.transform.position); 
        if( nearestNode.GetLineOfSight() && nearestNode.GetDistance() < maxPounceDistance) //inside range and has line of sight,return true
        {
            agent.SetCurrentAction(this);
            agent.SetMovementEnabled(false);
            if(leaveTrail)
            {
                if(trail == null){CreateTrail();}
                trail.GetComponent<ParticleSystem>().Stop();
            }
            if(buildupParticles)
            {
                if(buildup == null){CreateBuildup();}
                buildup.GetComponent<ParticleSystem>().Play();
            }
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE; //If player outside of range or no line of sight, return false
    }
    public override void UpdateNode(BTAgent agent)//update node
    {
        switch(currentState)
        {
            case actionState.windup: //wait to dash
                windUpTimer += Time.deltaTime;
                if(windUpTimer >= windUpDuration){currentState = actionState.calculateTarget;}
                break;
            case actionState.calculateTarget: //Get end position of dash
                if(tryGetEndPos())
                {
                    buildup.GetComponent<ParticleSystem>().Stop();
                    currentState = actionState.dash;
                    if(leaveTrail){trail.GetComponent<ParticleSystem>().Play();}
                    break;
                }
                else // invalid end pos, exit
                {
                    ExitNode(agent);
                    break;
                }
            case actionState.dash: //dash
                durationTimer += Time.deltaTime;
                if(durationTimer > duration) //exit dash
                {
                    ExitNode(agent);
                    break;
                }
                //check distance
                float distance = Vector3.Distance(agent.transform.position, endPos);
                if(distance < hitRange){ExitNode(agent); return;}
                if(Vector3.Distance(agent.transform.position, PlayerController.instance.transform.position) < hitRange) //if player within hit range
                {
                    if(PlayerController.instance.GetFreezeGameplayInput()) //if player already locked, return 
                    {
                        ExitNode(agent);
                        return;
                    }
                    currentState = actionState.grab; //Go to grab state
                    //Bind player to agent
                    PlayerController.instance.transform.parent = agent.projectileSpawn;
                    PlayerController.instance.transform.position = agent.projectileSpawn.position;
                    PlayerController.instance.FreezeGameplayInput(true);
                    agent.SetLookingAtPlayer(false);
                    agent.SetVelocity(Vector3.zero);
                    PlayerController.instance.GetComponent<Collider>().enabled = false;
                    break;
                }
                Vector3 velocity = endPos - startPos; //dash in direcrion
                velocity.Normalize();
                velocity *= pounceSpeed;
                agent.SetVelocity(velocity);
                break;
            case actionState.grab:
                //Start chokeslam animation
                agent.GetComponent<Animator>().Play("Chokeslam"); //chokeslam anim
                currentState = actionState.chokeslam;
                break;
            case actionState.chokeslam: //Chokeslam 
                chokeslamTimer += Time.deltaTime;
                if(chokeslamTimer >= chokeslamDuration) //slam
                {
                    AttackPlayer(); //damage player
                    PlayerController.instance.transform.parent = null;
                    Vector3 pos = PlayerController.instance.transform.position;
                    pos.y = 1;
                    PlayerController.instance.transform.rotation = Quaternion.identity;
                    PlayerController.instance.GetComponent<Collider>().enabled = true;
                    if(PlayerController.instance.GetHealth() > 0)
                    {
                        PlayerController.instance.FreezeGameplayInput(false);
                        PlayerController.instance.transform.position = pos;
                    }
                    agent.SetLookingAtPlayer(true);
                    ExitNode(agent);
                }
                break;
        }
    }
    private bool tryGetEndPos() //Get end location using a spherecast
    {
        startPos = agent.transform.position;
        Vector3 directionOfPounce = PlayerController.instance.transform.position - agent.transform.position;
        directionOfPounce.Normalize();
        RaycastHit hit;
        endPos = PlayerController.instance.transform.position + directionOfPounce * overshootDistance;
        if(Physics.SphereCast(agent.transform.position, .5f, directionOfPounce, out hit, maxPounceDistance, 1 << 8))
        {
            if(Vector3.Distance(hit.point, agent.transform.position) <5){return false;}
            endPos = hit.point - directionOfPounce * 3f;
            endPos.y = agent.transform.position.y;
        }
        return true;
    }
    private void AttackPlayer() //damage player
    {
        PlayerController.instance.Damage(damage, false, true);
    }
    private void ExitNode(BTAgent agent) //exit node
    {
        if(trail != null){trail.GetComponent<ParticleSystem>().Stop();}
        if(buildup != null){buildup.GetComponent<ParticleSystem>().Stop();}
        agent.ClearCurrentAction();
        agent.SetMovementEnabled(true);
    }
    #region Trail
    private void CreateTrail() //Create trail vfx
    {
        if(trail != null)
        {
            DestroyTrail();
        }
        var trailPrefab = Resources.Load("VFX/DashTrail");
        if(trailPrefab == null)
        {
            throw new System.IO.FileNotFoundException("Failed to find dash prefab, please check resource folder");
        }
        trail = GameObject.Instantiate(trailPrefab as GameObject, agent.transform.position + agent.transform.forward * -.5f,agent.transform.rotation);
        trail.transform.parent = agent.transform;
    }
    private void DestroyTrail() //delete trail vfx
    {
        if(trail == null){return;}
        GameObject.Destroy(trail.gameObject);
    }
    #endregion
    #region BuildUp
    private void CreateBuildup() //Instantiate buildup vfx
    {
        if(trail != null)
        {
            DestroyBuildup();
        }
        var trailPrefab = Resources.Load("VFX/DashBuildup");
        if(trailPrefab == null)
        {
            throw new System.IO.FileNotFoundException("Failed to find dash prefab, please check resource folder");
        }
        Vector3 offset = new Vector3(0,-0.9f,-0.75f); //trust me bro
        buildup = GameObject.Instantiate(trailPrefab as GameObject, agent.transform.position,agent.transform.rotation);
        buildup.transform.parent = agent.transform;
        buildup.transform.localPosition = offset;
    }
    private void DestroyBuildup() //destroy buildup vfx
    {
        if(buildup == null){return;}
        GameObject.Destroy(buildup.gameObject);
    }
    #endregion
    
    public override void DeInitialiseNode(BTAgent agent) //destroy vfx if the agent dies
    {
        DestroyTrail();
        DestroyBuildup();
    }
}
