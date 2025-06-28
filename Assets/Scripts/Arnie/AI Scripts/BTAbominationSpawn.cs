using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAbominationSpawn : BTNode
{
    private GameObject[] enemyPrefabs;
    private Transform spawnPos;
    private GameObject spawnEffectPrefab;
    private string animName;
    private float delaytimer, cooldown = 0;

    private float waitDuration, waitTimer;
    public BTAbominationSpawn(GameObject[] _enemyPrefabs, Transform _spawnPos, GameObject _spawnEffect, float _waitDuration, string _animName = "", float _cooldown = 3)
    {
        enemyPrefabs = _enemyPrefabs;
        spawnPos = _spawnPos;
        spawnEffectPrefab = _spawnEffect;
        waitDuration = _waitDuration;
        animName = _animName;
        cooldown = _cooldown;
    }
    public override NodeState Evaluate(BTAgent agent)
    {
        if (EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight())
        {
            agent.SetCurrentAction(this);

            if (animName != "")
            {
                agent.GetComponent<Animator>().Play(animName);
            }
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
    public override void UpdateNode(BTAgent agent)
    {

        waitTimer += Time.deltaTime;
        if (waitTimer < waitDuration)
        {
            //GameObject spawneffect = GameObject.Instantiate(spawnEffectPrefab, spawnPos.position, Quaternion.identity);
            //spawneffect.transform.parent = agent.transform;
            return;
        }
        else
        {
            System.Random rand = new System.Random();

            int random = rand.Next(enemyPrefabs.Length);

            GameObject clone = GameObject.Instantiate(enemyPrefabs[random], spawnPos.position, Quaternion.identity);
            clone.GetComponent<BTAgent>().SetInstantSpawn(true);

            Vector3 playerPos = PlayerController.instance.transform.position;
            playerPos.y = clone.transform.position.y;
            clone.transform.LookAt(playerPos);


            EnemySpawningSystem.instance.SubscribeEnemy(clone.GetComponent<BTAgent>());

            clone.transform.parent = agent.gameObject.transform.parent;


            //Exit node
            waitTimer = 0;
            agent.ClearCurrentAction();
            return;
        }


    }
}
