using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCerebralThrow : BTNode
{
    private float timeLastActivated = 0;
    private float cooldown;
    private GameObject prefab;
    private float waitDuration, waitTimer;
    private float damage, velocity, maxDistance;
    private string throwAnim, recieveAnim;
    private GameObject projectile;
    private Transform spawnTransform;
    private enum ScriptState
    {
        INACTIVE,
        THROW,
        WAIT,
        RECIEVE,
        EXIT
    }
    private ScriptState currState = ScriptState.INACTIVE;
    public BTCerebralThrow(BTAgent _agent, GameObject _prefab, float _waitDuration, float _cooldown, float _damage, float _velocity, float _maxDistance, string _throwAnim, string _recieveAnim, Transform _spawnTranform)
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

    public override NodeState Evaluate(BTAgent agent)
    {
        if (Time.time - timeLastActivated < cooldown) { return NodeState.FAILURE; }

        ///////////////
        if (EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight())
        {
            agent.SetCurrentAction(this);
            waitTimer = 0;
            agent.GetComponent<Animator>().Play(throwAnim);
            //agent.SetMovementEnabled(false);
            currState = ScriptState.THROW;
            return NodeState.SUCCESS;
        }
        else
        {

            return NodeState.FAILURE;
        }

        ////////////////////
        
        /*agent.SetCurrentAction(this);
        waitTimer = 0;
        agent.GetComponent<Animator>().Play(throwAnim);
        //agent.SetMovementEnabled(false);
        currState = ScriptState.THROW;
        return NodeState.SUCCESS;*/
    }

    public override void UpdateNode(BTAgent agent)
    {
        switch (currState)
        {
            case ScriptState.INACTIVE:
                break;
            case ScriptState.THROW:
                waitTimer += Time.deltaTime;
                if (waitTimer < waitDuration) { break; }
                projectile = GameObject.Instantiate(prefab, spawnTransform.position, agent.transform.rotation);
                Vector3 pos = projectile.transform.position;
                pos.y = 0.5f;

                ////////
                agent.GetComponent<JockeyActions>().activeProjctile = projectile.gameObject;

                projectile.transform.position = pos;
                projectile.GetComponent<CerebralThrowProjectile>().Init(velocity, 0, 99, damage, agent.gameObject, false);
                projectile.GetComponent<CerebralThrowProjectile>().InitHat(this, 20);
                currState = ScriptState.WAIT;
                agent.GetComponent<JockeyActions>().SetCerebralVisible(false);
                //Instantiate projectile
                break;
            case ScriptState.WAIT:
                //Wait until gameobject is close enough to recieve
                if (projectile != null)
                {
                    if (!projectile.GetComponent<CerebralThrowProjectile>().isReturning()) { break; }
                    if (Vector3.Distance(agent.transform.position, projectile.transform.position) > 4) { break; }
                }
                agent.GetComponent<Animator>().Play(recieveAnim);
                agent.GetComponent<JockeyActions>().SetCerebralVisible(true);
                currState = ScriptState.RECIEVE;
                waitTimer = 0;
                break;
            case ScriptState.RECIEVE:
                waitTimer += Time.deltaTime;
                if (waitTimer <= 1) { break; }
                currState = ScriptState.EXIT;
                //recieve gameobject
                break;
            case ScriptState.EXIT:
                ExitNode();
                break;
        }
    }
    private void ExitNode()
    {
        currState = ScriptState.INACTIVE;
        agent.ClearCurrentAction();
        agent.GetComponent<JockeyActions>().SetCerebralVisible(true);
        //agent.SetMovementEnabled(true);
    }
    public override void DeInitialiseNode(BTAgent agent)
    {
        if (projectile != null)
        {
            GameObject.Destroy(projectile);
        }
    }
}