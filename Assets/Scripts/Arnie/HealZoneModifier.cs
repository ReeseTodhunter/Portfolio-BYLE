using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealZoneModifier : BTModifier
{
    bool activated;
    public GameObject slowZone;
    public float bulletSlowAmount = 10.0f;
    public override void Initialise(Character _agent)
    {
        title = "projectile-slower";
        type = modifierType.onUpdate;
        base.Initialise(_agent);
        slowZone = Resources.Load("Elite/HealZone") as GameObject;
    }

    public override void ActivateModifier(Character _agent)
    {
        base.ActivateModifier(_agent);

        if (!activated)
        {
            slowZone = Instantiate(slowZone, _agent.gameObject.transform.position, Quaternion.identity, _agent.gameObject.transform);

            activated = true;
        }
        slowZone.transform.position = _agent.gameObject.transform.position;
    }

    
}

