using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenModifier : BTModifier
{
    private float regenTimer = 0.0f;
    public float healPerSecond = .66f;

    public override void Initialise(Character _agent)
    {
        title = "Regeneration";
        type = modifierType.onUpdate;
        base.Initialise(_agent);
    }

    public override void ActivateModifier(Character _agent)
    {
        base.ActivateModifier(_agent);
       if(TryGetComponent<BTAgent>(out BTAgent agent))
       {
            if(agent.GetHealth() < agent.GetMaxHealth())
            {
                agent.Heal(Time.deltaTime * healPerSecond);
                regenTimer = 0;
                if(agent.gameObject.GetComponentInChildren<EnemyHealthBar>())
                {
                    agent.gameObject.GetComponentInChildren<EnemyHealthBar>().alwaysVisible = true;
                }
            }
        }
        regenTimer += Time.deltaTime;
    }
}
