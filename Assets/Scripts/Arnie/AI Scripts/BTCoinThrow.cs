using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BTCoinThrow : BTNode
{
    //Used for charger enemy, do not use on other stuffs
    private float decoupleTime, speed, timeBetweenEffects, totalDuration, timer, damageRadius, damage;
    private GameObject throwVFX, impactVFX;
    private string animationName;
    private GameObject projectilePrefab;
    private GameObject projectile;

    private List<GameObject> impacts = new List<GameObject>();
    private List<Vector3> spawns = new List<Vector3>();

    private enum scriptState
    {
        throwing,
        flying,
        falling,
        exit
    }
    private scriptState currState = scriptState.exit;
    public BTCoinThrow(BTAgent _agent, GameObject Projectile, float _decoupleTime, float _speed, float _timeBeforeFall, float _totalDuration = 6, float _damageRadius = 3, float _damage = 20, string _animName = "Throw")
    {
        agent = _agent;
        decoupleTime = _decoupleTime;
        speed = _speed;
        timeBetweenEffects = _timeBeforeFall;
        damageRadius = _damageRadius;
        damage = _damage;
        totalDuration = _totalDuration;
        impactVFX = Resources.Load("VFX/ShopKeeper/CoinImpact") as GameObject;
        animationName = _animName;
    }

    public override NodeState Evaluate(BTAgent agent)
    {
        CreateVFX();
        if (impacts[0] != null)
        {
            foreach (GameObject impact in impacts)
            {
                impact.SetActive(false);
            }
        }

        timer = 0;
        agent.SetCurrentAction(this);
        agent.GetComponent<Animator>().Play("ShopKeeperCoinShower");
        agent.SetMovementEnabled(false);
        currState = scriptState.throwing;
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent)
    {
        timer += Time.deltaTime;
        switch (currState)
        {
            case scriptState.throwing:
                if (timer < decoupleTime)
                {
                    break;
                }

                currState = scriptState.flying;
                agent.SetMovementEnabled(true);
                break;
            case scriptState.flying:


                if (timer >= timeBetweenEffects)
                {
                    if (impacts[0] != null)
                    {
                        foreach (GameObject impact in impacts)
                        {
                            impact.SetActive(true);
                        }
                    }

                    currState = scriptState.falling;
                }
                break;
            case scriptState.falling:
                if (timer >= totalDuration)
                {
                    currState = scriptState.exit;
                }
                break;
            case scriptState.exit:
                ExitNode();
                break;
        }
    }
    public override void DeInitialiseNode(BTAgent agent)
    {
        DestroyVFX();
    }
    private void ExitNode()
    {
        agent.ClearCurrentAction();
        DestroyVFX();
        return;
    }
    private void CreateVFX()
    {
        DestroyVFX();

        for (int i = 0; i < 20; i++)
        {
            int rndSpawn = Random.Range(-50, 50);
            int rndSpawn2 = Random.Range(-50, 50);
            Vector3 randSpawn = new Vector3(agent.transform.position.x + rndSpawn, agent.transform.position.y, agent.transform.position.z + rndSpawn2);

            GameObject impact = GameObject.Instantiate(impactVFX, randSpawn, Quaternion.identity);
            impacts.Add(impact);
        }

    }
    private void DestroyVFX()
    {
        foreach (GameObject impact in impacts)
        {
            GameObject.Destroy(impact);
        }
    }
}