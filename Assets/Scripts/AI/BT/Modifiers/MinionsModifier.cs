using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionsModifier : BTModifier
{
    /*
     * This modifier allows the agent to spawn minions after they reach a certain health percentage
     * -Tom
    */

    [Range(0,1)] public float healthPercentage;  //percentage at which they spawn enemies
    private bool activated = false;
    public GameObject minionPrefab; //prefab of spawned minions
    public int minionCount = 1; //current minion count
    [SerializeField] float maxSpawnDistance = 10; //max distance from agent minions can spawn
    private List<BTAgent> minions = new List<BTAgent>(); //list of minions
    public bool spawnOnParent = false;
    public override void Initialise(Character _agent)
    {
        base.Initialise(_agent);
        activated = false;
    }
    public override void ActivateModifier(Character _agent)
    {
        if(activated){return;} //if already activated, return
        if((_agent.GetHealth() / _agent.GetMaxHealth()) > healthPercentage){return;} //check if threshold reached
        activated = true;
        EQSNode[,] nodes = EQS.instance.GetNodes();
        List<Vector3> validSpawns = new List<Vector3>();
        foreach(EQSNode node in nodes) //get all valid nodes to spawn
        {
            if(node.GetTraversable() && Vector3.Distance(transform.position, node.GetWorldPos()) < maxSpawnDistance)
            {
                validSpawns.Add(node.GetWorldPos());
            }
        }
        for(int i = 0; i < minionCount; i++) //instantiate all minions
        {
            Vector3 spawn;
            if (!spawnOnParent)
            {
                int index = Random.Range(0, validSpawns.Count - 1);
                spawn = validSpawns[index];
                validSpawns.RemoveAt(index);
                spawn.y = 1;

            }
            else
            {
                spawn = _agent.transform.position;
            }
            GameObject minion = Instantiate(minionPrefab,spawn,Quaternion.identity);
            minion.GetComponent<BTAgent>().enabled = true;
            minion.GetComponent<BTAgent>().SubscribeAgent(this);

            EnemySpawningSystem.instance.DefaultSpawnerVariables();
            EnemySpawningSystem.instance.SubscribeEnemy(minion.GetComponent<BTAgent>());

        }
    }
    public void SubscribeMinion(BTAgent minion) //sub minion
    {
        if(minions.Contains(minion)){return;}
        minions.Add(minion);
    }
    public void UnsubscribeMinion(BTAgent minion) //unsub minion
    {
        if(!minions.Contains(minion)){return;}
        minions.Remove(minion);
    }
}
