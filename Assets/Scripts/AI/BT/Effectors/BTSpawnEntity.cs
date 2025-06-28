using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSpawnEntity : BTNode
{
    /*
     * This node allows agents to spawn other agents
     * -TOm
     */
    private GameObject prefab; //prefab of agent to spawn
    private Transform spawnPos; //position to spawn at 
    private float EntityDelay = 0; // delay before spawning
    private bool bindToTransform = false; //Whether or not to bind to transform
    private float bindDuration = 0; //Bind duration
    private float bindTimer = 0; 
    private GameObject Entity; //Reference to spawned agent
    private string animName;

    //constructor
    public BTSpawnEntity(BTAgent _agent, GameObject _prefab, Transform _spawnPos, float _EntityDelay = 0, string _animName = "",bool _bindToTransform = false, float _bindDuration = 0)
    {
        agent = _agent;
        prefab = _prefab;
        spawnPos = _spawnPos;
        EntityDelay = _EntityDelay;
        bindToTransform = _bindToTransform;
        bindDuration = _bindDuration;
        animName = _animName;
    }
    public override NodeState Evaluate(BTAgent agent) //Evaluate 
    {
        Entity = GameObject.Instantiate(prefab,spawnPos.position,spawnPos.rotation); //instantiate agent
        if(bindDuration > 0) 
        {
            agent.SetCurrentAction(this);
        }
        if(bindToTransform)
        {
            Entity.transform.parent = spawnPos;
        }
        if(EntityDelay > 0) //delay the activation of spawned agent
        {
            Entity.GetComponent<BTAgent>().enabled = false;
        }
        agent.GetComponent<Animator>().Play(animName); //play anim
        bindTimer = 0;
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        bindTimer += Time.deltaTime;
        if(bindTimer > bindDuration)
        {
            Entity.transform.parent = null;
        }
        else
        {
            Entity.transform.localPosition = Vector3.zero;
        }
        if(bindTimer > EntityDelay)
        {
            Entity.GetComponent<BTAgent>().enabled = true;
            Entity.transform.position = agent.projectileSpawn.position;
        }

        if(bindTimer < bindDuration || bindTimer < EntityDelay){return;}
        agent.ClearCurrentAction();
    }
    public override void DeInitialiseNode(BTAgent agent) //deinitialise agent
    {
        if(Entity != null)
        {
            GameObject.Destroy(Entity);
        }
    }
}
