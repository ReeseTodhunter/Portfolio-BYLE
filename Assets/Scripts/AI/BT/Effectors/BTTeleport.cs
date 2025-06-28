using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTTeleport : BTNode
{
    /*
     * This node allows the agent to teleport
     * -Tom
     */
    private float lastTimeTeleported = 0; //last time node activated
    private float coolDown = 10; //cooldown
    private float teleportWindupDuration = 2, timer = 0, teleportEndDuration = 2; 
    private float maxTeleportDistance;
    public Vector3 teleportPosition; //position that the agent teleports to
    private bool lineOfSight; //whether or not the agent needs a line of sight
    private float minDistanceFromPlayer; //min distance from player the teleport destination can be
    private bool usesVFX; //whether or not the node uses vfx
    private GameObject startVFX,endVFX; //vfx prefabs
    private GameObject startObject,endObject; //vfx references
    private string animName; //animation name
    private string vfxPath; //resources.load path
    private float yPos; //ypos of teleport
    private bool loopingVFX; //whether or not vfx loops

    //Constructor
    public BTTeleport(BTAgent _agent, float _teleportCooldown, float _windUpDuration, float _maxTeleportDistance, bool _lineOfSight, float _minDistanceFromPlayer, bool _usesVFX = false, string _animName = "", string _vfxPath = "", float _yPos = .75f, bool _loopingVFX = true)
    {
        agent = _agent;
        coolDown = _teleportCooldown;
        teleportWindupDuration = _windUpDuration;
        maxTeleportDistance = _maxTeleportDistance;
        lineOfSight = _lineOfSight;
        minDistanceFromPlayer = _minDistanceFromPlayer;
        usesVFX = _usesVFX;
        animName = _animName;
        vfxPath = _vfxPath;
        yPos = _yPos;
        loopingVFX = _loopingVFX;
    }
    public override NodeState Evaluate(BTAgent agent) //Evaluate the output of the node
    {
        if(Time.time - lastTimeTeleported > coolDown)
        {
            //Teleport
            if(usesVFX)
            {
                CreateVFX(); //Create vfx
                startVFX.GetComponent<ParticleSystem>().Play();
            }
            timer = 0;
            teleportPosition = GetTeleportPosition(); //get teleport pos
            if(teleportPosition == Vector3.down) { return NodeState.FAILURE; } //invalid teleport pos
            teleportPosition.y = 1;
            if(teleportPosition == null){return NodeState.FAILURE;} //invalid teleport pos
            if(animName != "") //play anim
            {
                agent.GetComponent<Animator>().Play(animName);
            }
            agent.SetMovementEnabled(false);
            agent.SetCurrentAction(this);
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE; //return false if cooldown still in effect
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        if(usesVFX)
        {
            startVFX.transform.position = agent.transform.position;
            startVFX.transform.position -= Vector3.up * yPos;
        }
        timer += Time.deltaTime;
        if(timer > teleportWindupDuration)
        {
            agent.transform.position = teleportPosition;
            ExitNode();
            return;
        }
    }
    private Vector3 GetTeleportPosition() //Get position
    {
        EQSNode[,] nodes = EQS.instance.GetNodes();
        EQSNode bestNode = null;
        float distance = 0; float bestDistance = 0;
        foreach(EQSNode node in nodes) //Go through all nodes and the get the one with the best distance
        {
            if(!node.GetTraversable()){continue;}
            if(Vector3.Distance(node.GetWorldPos(), agent.transform.position) > maxTeleportDistance){continue;}
            if(node.GetDistance() < minDistanceFromPlayer){continue;}
            if(node.GetLineOfSight() != lineOfSight){continue;}
            distance = Vector3.Distance(agent.transform.position, node.GetWorldPos());
            if(distance > bestDistance)
            {
                bestDistance = distance;
                bestNode = node;
            }
        }
        if(bestNode == null) {return Vector3.down; } // return null
        if(usesVFX)
        {
            endVFX.transform.position = bestNode.GetWorldPos();
            Vector3 pos = endVFX.transform.position;
            pos.y = agent.transform.position.y;
            pos.y -= yPos;
            endVFX.transform.position = pos;
            endVFX.GetComponent<ParticleSystem>().Play();
        }
        return bestNode.GetWorldPos(); //return position
    }
    private void CreateVFX() //Create vfx objects
    {
        if(startVFX != null || endVFX != null)
        {
            DestroyVFX();
        }
        if (vfxPath == "") //load from resource folder
        {
            startObject = Resources.Load("VFX/Cultist/TeleportStart") as GameObject;
            endObject = Resources.Load("VFX/Cultist/TeleportEnd") as GameObject;
        }
        else
        {
            startObject = Resources.Load(vfxPath) as GameObject;
            endObject = Resources.Load(vfxPath) as GameObject;
        }
        startVFX = GameObject.Instantiate(startObject,agent.transform.position,Quaternion.identity);
        endVFX = GameObject.Instantiate(endObject,agent.transform.position,Quaternion.identity);
    }
    private void DestroyVFX() //Destroy all vfx
    {
        if(startObject != null){GameObject.Destroy(startVFX.gameObject);}
        if(endObject != null){GameObject.Destroy(endVFX.gameObject);}
    }
    private void ExitNode() //exit node
    {
        agent.SetMovementEnabled(true);
        agent.ClearCurrentAction();
        if(usesVFX && loopingVFX)
        {
            startVFX.GetComponent<ParticleSystem>().Stop();
            endVFX.GetComponent<ParticleSystem>().Stop();
        }
    }
    private void DeInitialiseNode() //destroy vfx if the agent dies
    {
        DestroyVFX();
    }
}
