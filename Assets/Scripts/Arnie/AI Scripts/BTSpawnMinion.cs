using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSpawnMinion : BTNode
{
    /*
        Basic projectile firing behaviour for Behaviour tree agents
    */
    private GameObject enemyPrefab;
    private GameObject specialEnemyPrefab;
    private GameObject spawnEffectPrefab;
    private int specialSpawnChance;
    private string animName;
    private float delay, delayTimer = 0;

    private float waitDuration, waitTimer;
    public BTSpawnMinion(GameObject _minionPrefab, GameObject _specialMinionPrefab, GameObject _spawnEffect, float _waitDuration, int _specialSpawnChance, string _animName = "", float _delay = 0)
    {
        enemyPrefab = _minionPrefab;
        specialEnemyPrefab = _specialMinionPrefab;
        spawnEffectPrefab = _spawnEffect;
        waitDuration = _waitDuration;
        specialSpawnChance = _specialSpawnChance;
        animName = _animName;
        delay = _delay;
    }
    public override NodeState Evaluate(BTAgent agent)
    {
        if (EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight())
        {
            agent.SetCurrentAction(this);
            delayTimer = 0;
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
            GameObject spawneffect = GameObject.Instantiate(spawnEffectPrefab, agent.gameObject.transform.position, Quaternion.identity);
            spawneffect.transform.parent = agent.transform;
            return;
        }
        else
        {
            //int rand = Random.Range(1, specialSpawnChance);

            System.Random rand = new System.Random();

            int random = rand.Next(specialSpawnChance);

            if (random == 2)
            {

                GameObject clone = GameObject.Instantiate(specialEnemyPrefab, agent.projectileSpawn.gameObject.transform.position, Quaternion.identity);
                clone.GetComponent<Character>().AddModifier(ModifierType.MaxHealth, -agent.GetMaxHealth());
                
                EnemySpawningSystem.instance.SubscribeEnemy(clone.GetComponent<BTAgent>());

                clone.transform.parent = agent.gameObject.transform.parent;
            }
            else
            {
                GameObject clone = GameObject.Instantiate(enemyPrefab, agent.projectileSpawn.gameObject.transform.position, Quaternion.identity);
                clone.GetComponent<Character>().AddModifier(ModifierType.MaxHealth, -agent.GetMaxHealth());
                EnemySpawningSystem.instance.SubscribeEnemy(clone.GetComponent<BTAgent>());

                clone.transform.parent = agent.gameObject.transform.parent;
            }


            //Exit node
            waitTimer = 0;
            agent.ClearCurrentAction();
            return;
        }

        
    }
}
