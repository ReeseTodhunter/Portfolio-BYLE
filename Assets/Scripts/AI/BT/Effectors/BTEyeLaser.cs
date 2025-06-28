using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTEyeLaser : BTNode
{
    /*
        Behaviour Tree effector node, Controls the eye laser attack of the Big
        Boris enemy
        -Tom
    */
    private GameObject laserEye; //Transform position of the laser
    private GameObject laserLinePrefab; //prefab of the laser
    private GameObject laserLine; //Line of the laser
    private GameObject laserEndVFX; //VFX for the end of the laser
    private GameObject laserStartVFX; //VFx for the start of the laser
    private GameObject laserEndGroundDestructionVFX; //trail that follows the end of the laser
    private float windupDuration, windupTimer, windDownDuration; //windups
    private float laserMoveSpeed; //move speed for the end of the laser
    private float laserMoveTimer = 0; //timer for movement
    private float lastTimeUsed = 0; //last time node active
    private float cooldown; //cooldown duration
    private Vector3 startPos, endPos, currPos, direction; //laser positions
    private float timeSinceHitPlayer; //used for damage per second
    private string startAnim, endAnim; //anims
    private enum ScriptState //controls what state the node is in
    {
        Inactive,
        Windup,
        LaserMove,
        windDown,
        Exit
    }
    private ScriptState currState = ScriptState.Inactive; //current state

    //constructor
    public BTEyeLaser(BTAgent _agent, GameObject _laserEye,float _windUpDuration,float _laserMoveSpeed, float _cooldown, float _windDownDuration, string _startAnim, string _endAnim)
    {
        agent = _agent;
        laserEye = _laserEye;
        windupDuration = _windUpDuration;
        laserMoveSpeed = _laserMoveSpeed;
        cooldown = _cooldown;
        windDownDuration = _windDownDuration;
        startAnim = _startAnim;
        endAnim = _endAnim;
    }
    public override NodeState Evaluate(BTAgent agent) //evaluates the output of the node
    {
        if(Time.time - lastTimeUsed < cooldown){return NodeState.FAILURE;} //cooldown not finished, return false
        windupTimer = 0;
        agent.SetCurrentAction(this); //lock action tree and movement
        agent.SetMovementEnabled(false);
        if(laserLine == null) //if laser line is null, instantiate it
        {
            laserLinePrefab = Resources.Load("BigBoris/LaserLinePrefab") as GameObject;
            laserLine = GameObject.Instantiate(laserLinePrefab,laserEye.transform.position,Quaternion.identity);
            laserLine.layer = agent.gameObject.layer;
        }
        if(laserEndGroundDestructionVFX == null) //instantiate vfx if null
        {
            GameObject temp = Resources.Load("BigBoris/LaserGroundDestruction") as GameObject;
            laserEndGroundDestructionVFX = GameObject.Instantiate(temp,agent.transform.position,Quaternion.identity);
            laserEndGroundDestructionVFX.SetActive(false);
        }
        laserLine.SetActive(false); //hide laser line
        currState = ScriptState.Windup; //set state to windup
        agent.GetComponent<Animator>().Play(startAnim); //start anim
        return NodeState.SUCCESS;// return true
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        switch(currState)
        {
            case ScriptState.Inactive: //do nothing
                break;
            case ScriptState.Windup: //windup laser
                windupTimer += Time.deltaTime;
                if(windupTimer < windupDuration){break;} //return if windup not done
                currState = ScriptState.LaserMove;
                direction = PlayerController.instance.transform.position - agent.transform.position; //set lasser direction
                direction.Normalize();
                endPos = agent.transform.position + direction * 40;
                endPos.y = 0;
                startPos = agent.transform.position + direction * 2;
                startPos.y = 0;
                currPos = startPos;
                currState = ScriptState.LaserMove;
                agent.SetLookingAtPlayer(false); //lock agent direction
                laserLine.SetActive(true);
                laserEndGroundDestructionVFX.SetActive(true); //activate vfx
                laserEndGroundDestructionVFX.transform.rotation = agent.transform.rotation;
                laserEndGroundDestructionVFX.GetComponent<ParticleSystem>().Play();
                break;
            case ScriptState.LaserMove: //move laser 
                currPos += direction * laserMoveSpeed * Time.deltaTime; //move laser along
                currPos.y = 0;
                if(Vector3.Distance(currPos,endPos) < 1) //if at the end, stop laser
                {
                    windupTimer = 0;
                    currState = ScriptState.windDown;
                    agent.GetComponent<Animator>().Play(endAnim);
                    laserEndGroundDestructionVFX.GetComponent<ParticleSystem>().Stop();
                    if(laserLine != null)
                    {
                        GameObject.Destroy(laserLine.gameObject);
                    }
                    agent.SetLookingAtPlayer(true);
                    break;
                }
                //move laser linerenderer
                laserLine.GetComponent<LineRenderer>().SetPosition(0,laserEye.transform.position);
                laserLine.GetComponent<LineRenderer>().SetPosition(1,currPos);
                laserEndGroundDestructionVFX.transform.position = currPos;
                CheckPlayerHit(); //check if hit player
                break;
            case ScriptState.windDown: //wind down, lock agent 
                windupTimer += Time.deltaTime;
                if(windupTimer < windDownDuration){break;}
                currState = ScriptState.Exit;
                break;
            case ScriptState.Exit:  //exit
                ExitNode();
                break;
        }
    }
    private void ExitNode() //exit node
    {
        lastTimeUsed = Time.time;
        if(laserEndGroundDestructionVFX != null)
        {
            GameObject.Destroy(laserEndGroundDestructionVFX.gameObject);
        }   
        agent.ClearCurrentAction();
        agent.SetMovementEnabled(true);
    }
    private void CheckPlayerHit() //if player within laser radius and not invincible, do damage
    {
        if(Time.time - timeSinceHitPlayer < 0.25f){return;}
        Vector3 playerPos = PlayerController.instance.transform.position;
        playerPos.y = 0;
        Vector3 laserPos = currPos;
        laserPos.y = 0;
        float distance = Vector3.Distance(playerPos,laserPos);
        if(distance < 3)
        {
            PlayerController.instance.Damage(20,false,true,false,EffectType.Burn,false);
        }
        timeSinceHitPlayer = Time.time;
    }
}
