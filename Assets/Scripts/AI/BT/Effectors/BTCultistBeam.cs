using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCultistBeam : BTNode
{
    /*
        Behaviour Tree effector tree, controls the laser beam attack of
        the purple cultist enemy.
        -Tom
    */
    private float windupDuration; //windup duration of the attack
    private GameObject LaserObject; //prefab of the laser
    private Vector3 startPos, endPos, currPos; //start, end and current pos of the laser
    private float damagePerSecond,duration; //damage per second and duration of attack
    private float timer = 0; //duration timer
    private enum ScriptState //State of the script
    {
        inactive,
        charge,
        chargeWait,
        beam,
        exit
    }
    private ScriptState currState = ScriptState.inactive; //current state of sctipt
    private float ticksPerSecond; //amount of times the player can be damaged a second
    private float tickTimer = 0;
    private LineRenderer lineRnd; //line renderer of the laser
    private Quaternion lockedRot; //rotation of the laser
    private float moveDuration = .5f, moveTimer = 0; //spped of the lasers movement
    
    //Constructor
    public BTCultistBeam(BTAgent _agent, GameObject _prefab, float _windUpDuration, float _radius, float _damagePerSecond, float _duration, Material _laserMat, float _ticksPerSecond = 5)
    {
        agent = _agent;
        LaserObject = GameObject.Instantiate(_prefab,agent.transform.position,Quaternion.identity); //instantiate laser
        lineRnd = LaserObject.GetComponent<LineRenderer>(); // get line renderer
        windupDuration = _windUpDuration;
        damagePerSecond = _damagePerSecond;
        duration = _duration;
        ticksPerSecond = _ticksPerSecond;
    }
    public override NodeState Evaluate(BTAgent agent) //Get the value of the node
    {
        //always return true
        currState = ScriptState.charge; //set state
        timer = 0;
        agent.SetCurrentAction(this); //lock action tree
        agent.SetMovementEnabled(false); //Lock movement
        LaserObject.SetActive(true); //enabled laser
        moveTimer = 0;
        currPos = agent.projectileSpawn.position; //get position of laser start
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent)
    {
        LaserObject.transform.position = agent.projectileSpawn.position; //get laser start
        LaserObject.transform.forward = agent.projectileSpawn.forward; //get laser forward vector
        lineRnd.SetPosition(0,LaserObject.transform.position); //set positons of laser
        lineRnd.SetPosition(1,currPos);
        endPos = FindEndLocation(); //Get end of laser 
        LaserObject.transform.GetChild(2).position = lineRnd.GetPosition(1); //set position of laser end object
        switch(currState)
        {
            case ScriptState.inactive:
                break;
            case ScriptState.charge: //charging up laser

                agent.GetComponent<Animator>().speed = 1 / windupDuration; //set anim speed
                agent.GetComponent<Animator>().Play("BeamChargeUp"); //play agent anim
                LaserObject.GetComponent<Animator>().speed = 1 / windupDuration; //set laser anim speed
                LaserObject.GetComponent<Animator>().Play("Windup",0); //play laser anim

                currState = ScriptState.chargeWait;
                timer = 0;
                break;
            case ScriptState.chargeWait: //wait for laser
                ChargeWaitUpdate();
                break;
            case ScriptState.beam: //shoot laser
                BeamUpdate();
                break;
            case ScriptState.exit: //exit
                agent.ClearCurrentAction(); //unlock action tree
                agent.SetLookingAtPlayer(true); //unlock agent look
                agent.SetMovementEnabled(true); //unlock movement
                LaserObject.GetComponent<Animator>().Play("WindDown",0); //play exit anims
                agent.GetComponent<Animator>().Play("BeamExit");
                break;
        }
        return;
    }

    private void ChargeWaitUpdate() //waiting for laser
    {
        timer += Time.deltaTime;
        if(timer < windupDuration){return;}
        timer = 0;
        tickTimer = 99;
        agent.GetComponent<Animator>().speed = 1;
        agent.GetComponent<Animator>().Play("BeamAttack"); //play laser animations
        LaserObject.GetComponent<Animator>().Play("Active");
        lockedRot = agent.transform.rotation;
        agent.SetLookingAtPlayer(false); //lock look direction
        currState = ScriptState.beam;
    }
    private void BeamUpdate() //updating beam
    {
        timer += Time.deltaTime; //increment timers
        tickTimer += Time.deltaTime;
        moveTimer += Time.deltaTime;
        startPos = agent.projectileSpawn.position; //set start pos of laser
        currPos = Vector3.Lerp(startPos, endPos, moveTimer / moveDuration); //get current pos of laser
        agent.transform.rotation = lockedRot; //lock rotation
        float distance = Vector3.Distance(currPos,startPos); //get distance
        if(tickTimer > 1 / ticksPerSecond) //foreach tick, try and damage the player
        {
            tickTimer = 0;
            RaycastHit hit;
            if(Physics.Raycast(agent.projectileSpawn.position, agent.transform.forward, out hit, distance, LayerMask.GetMask("Untraversable", "Player", "Hurtbox", "SeeThrough"), QueryTriggerInteraction.Ignore))
            {
                if(hit.collider.gameObject == PlayerController.instance.gameObject)
                {
                    float damage = damagePerSecond / ticksPerSecond;
                    PlayerController.instance.Damage( damage,false,false);
                }
            }
        }
        if(timer < duration){return;}
        timer = 0;
        currState = ScriptState.exit; //exit 
    }
    public Vector3 FindEndLocation() //Get end location of the laser
    {
        RaycastHit hit;
        Vector3 direction = agent.transform.forward;
        direction.Normalize();
        Vector3 endLocation;
        //Raycast forwards until hitting the player or an obstacle
        if(Physics.Raycast(agent.projectileSpawn.position,direction, out hit, 50, LayerMask.GetMask("Untraversable", "Player", "Hurtbox", "SeeThrough"), QueryTriggerInteraction.Ignore))
        {
            endLocation = hit.point;
            endLocation.y = 1;
        }
        else
        {
            endLocation = agent.transform.position + direction * 50;
        }
        return endLocation;
    }
    public override void DeInitialiseNode(BTAgent agent) //Destroy laser if the agent dies
    {
        if(LaserObject != null){GameObject.Destroy(LaserObject);}
    }
}
