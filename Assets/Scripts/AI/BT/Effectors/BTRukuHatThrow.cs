using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRukuHatThrow : BTNode
{
    /*
     * Node that allows the Ruku mini-boss to throw his hat as a projectile
     * -Tom
     */
    private float timeLastActivated = 0; //time since node was last activated
    private float cooldown; 
    private GameObject prefab; //projectie prefab
    private float waitDuration, waitTimer;
    private float damage, velocity, maxDistance; //damage, velocity and max distanc of projectile
    private string throwAnim, recieveAnim; //animations of attack
    private GameObject projectile; //projectile reference
    private Transform spawnTransform; //spawn location of projectile
    private enum ScriptState
    {
        INACTIVE,
        THROW,
        WAIT,
        RECIEVE,
        EXIT
    }
    private ScriptState currState = ScriptState.INACTIVE; //current script state

    //Constructor
    public BTRukuHatThrow(BTAgent _agent, GameObject _prefab, float _waitDuration, float _cooldown, float _damage, float _velocity, float _maxDistance, string _throwAnim, string _recieveAnim, Transform _spawnTranform)
    {
        agent = _agent;
        prefab = _prefab;
        waitDuration = _waitDuration;
        cooldown = _cooldown;
        damage = _damage;
        velocity = _velocity;
        maxDistance = _maxDistance;
        throwAnim = _throwAnim;
        recieveAnim = _recieveAnim;
        spawnTransform = _spawnTranform;
    }

    public override NodeState Evaluate(BTAgent agent) //evaluate node
    {
        if(Time.time - timeLastActivated < cooldown){return NodeState.FAILURE;} //if still cooling down, return false
        agent.SetCurrentAction(this);
        waitTimer = 0;
        agent.GetComponent<Animator>().Play(throwAnim); //throw anim
        currState = ScriptState.THROW;
        return NodeState.SUCCESS; //return true
    }

    public override void UpdateNode(BTAgent agent)
    {
        switch(currState)
        {
            case ScriptState.INACTIVE: //do nothing
                break;
            case ScriptState.THROW: //throw hat
                waitTimer += Time.deltaTime;
                if(waitTimer < waitDuration){break;}
                projectile = GameObject.Instantiate(prefab,spawnTransform.position,agent.transform.rotation); //instantiate projectile
                Vector3 pos = projectile.transform.position;
                pos.y = 0.5f;
                projectile.transform.position = pos;
                projectile.GetComponent<RukuHatProjectile>().Init(velocity,0,99,damage,agent.gameObject,false); //init projectile
                projectile.GetComponent<RukuHatProjectile>().InitHat(this,20); //init hat specific data
                currState = ScriptState.WAIT;
                agent.GetComponent<RukuActions>().SetHatVisible(false); //hide hat on Ruku's model
                //Instantiate projectile
                break;
            case ScriptState.WAIT:
                //Wait until gameobject is close enough to recieve
                if(projectile != null) //Grab hat if it can
                {
                    if(!projectile.GetComponent<RukuHatProjectile>().isReturning()){break;}
                    if(Vector3.Distance(agent.transform.position,projectile.transform.position) > 4){break;}
                }
                agent.GetComponent<Animator>().Play(recieveAnim);
                agent.GetComponent<RukuActions>().SetHatVisible(true); //show hat again
                currState = ScriptState.RECIEVE;
                waitTimer = 0;
                break;
            case ScriptState.RECIEVE:
                waitTimer += Time.deltaTime;
                if(waitTimer <= 1){break;}
                currState = ScriptState.EXIT;
                //recieve gameobject
                break;
            case ScriptState.EXIT:
                ExitNode();
                break;
        }
    }
    private void ExitNode() //exit node
    {
        currState = ScriptState.INACTIVE;
        agent.ClearCurrentAction();
        agent.GetComponent<RukuActions>().SetHatVisible(true);
    }
    public override void DeInitialiseNode(BTAgent agent) //Destroy hat if agent dies
    {
        if(projectile != null)
        {
            GameObject.Destroy(projectile);
        }
    }
}
