using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicatorModifier : BTModifier
{
    public GameObject DuplicatedEnemy;
    public GameObject ShinyEnemy;
    public override void Initialise(Character _agent)
    {
        title = "the Duplicator";
        type = modifierType.onDamage;
        base.Initialise(_agent);

    }

    public override void ActivateModifier(Character _agent)
    {
        base.ActivateModifier(_agent);

        int rand = Random.Range(1, 169);

        if(rand == 9)
        {
            if (_agent.GetHealth() < _agent.GetMaxHealth())
            {
                if (!_agent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
                {
                    _agent.GetComponent<Animator>().Play("Duplicate");
                }

                GameObject clone = Instantiate(ShinyEnemy, transform.position, Quaternion.identity);
                clone.GetComponent<Character>().AddModifier(ModifierType.MaxHealth, -_agent.GetMaxHealth());
            }
        }
        else
        {
            if (_agent.GetHealth() < _agent.GetMaxHealth())
            {
                if (!_agent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
                {
                    _agent.GetComponent<Animator>().Play("Duplicate");
                }

                GameObject clone = Instantiate(DuplicatedEnemy, transform.position, Quaternion.identity);
                clone.GetComponent<Character>().AddModifier(ModifierType.MaxHealth, -_agent.GetMaxHealth());
            }
        }
        
        

    }
}