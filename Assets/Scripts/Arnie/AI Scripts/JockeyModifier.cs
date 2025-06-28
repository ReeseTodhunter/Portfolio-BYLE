using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JockeyModifier : BTModifier
{
    public GameObject DuplicatedEnemy;
    public override void Initialise(Character _agent)
    {
        title = "the Ride";
        type = modifierType.onDeath;
        base.Initialise(_agent);

    }

    public override void ActivateModifier(Character _agent)
    {
        base.ActivateModifier(_agent);


        
        if (_agent.GetHealth() < _agent.GetMaxHealth())
        {
            if(_agent.GetComponent<JockeyActions>() != null)
            {
                if (_agent.GetComponent<JockeyActions>().activeProjctile == null)
                {
                    _agent.GetComponent<JockeyActions>().SetCerebralVisible(false);
                    GameObject clone = Instantiate(DuplicatedEnemy, transform.position, Quaternion.identity);
                    clone.transform.parent = _agent.gameObject.transform.parent;
                }
                else
                {
                    _agent.GetComponent<JockeyActions>().SetCerebralVisible(false);
                    GameObject clone = Instantiate(DuplicatedEnemy, _agent.GetComponent<JockeyActions>().activeProjctile.transform.position, Quaternion.identity);
                    clone.transform.parent = _agent.gameObject.transform.parent;
                }

            }
            else
            {
                if (_agent.GetComponent<TriplestackActions>().activeProjctile == null)
                {
                    _agent.GetComponent<TriplestackActions>().SetCerebralVisible(false);
                    GameObject clone = Instantiate(DuplicatedEnemy, transform.position, Quaternion.identity);
                    clone.transform.parent = _agent.gameObject.transform.parent;
                }
                else
                {
                    _agent.GetComponent<TriplestackActions>().SetCerebralVisible(false);
                    GameObject clone = Instantiate(DuplicatedEnemy, _agent.GetComponent<TriplestackActions>().activeProjctile.transform.position, Quaternion.identity);
                    clone.transform.parent = _agent.gameObject.transform.parent;
                }
            }

            
           

        }

    }
}