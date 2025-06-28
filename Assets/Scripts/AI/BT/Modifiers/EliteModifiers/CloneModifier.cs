using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneModifier : BTModifier
{
    public GameObject self;
    private float cloneTimer = 0;
    
    public override void Initialise(Character _agent)
    {
        title = "Decoy clones";
        self = _agent.gameObject;
        type = modifierType.onDamage;
        base.Initialise(_agent);
        //self = this.gameObject;

    }

    public override void ActivateModifier(Character _agent)
    {
        base.ActivateModifier(_agent);
        //if agent is below half health, when they take damage they spawn clones with 5 hp
        //Rand chance
        float rnd = Random.Range(1, 10);
        //if (rnd < 7) { return; }
        
        if(_agent.GetHealth() < _agent.GetMaxHealth() * 0.75f && cloneTimer > rnd)
        {
            GameObject clone = Instantiate(self,transform.position,Quaternion.identity);
            clone.GetComponent<Character>().AddModifier(ModifierType.MaxHealth,-_agent.GetMaxHealth());
            Destroy(clone.GetComponent<CloneModifier>());
            clone.GetComponent<BTAgent>().enemyType = BTAgent.EnemyType.standard;
            EnemySpawningSystem.instance.SubscribeEnemy(clone.GetComponent<BTAgent>());
            cloneTimer = 0;

        }

    }

    private void Update()
    {
        cloneTimer += Time.deltaTime;
    }

}
