using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicShield : BTModifier
{
    public override void Initialise(Character _agent)
    {
        title = "The Bulwark";
        type = modifierType.onUpdate;
        base.Initialise(_agent);
    }

    public override void ActivateModifier(Character _agent)
    {
        base.ActivateModifier(_agent);
        
    }
}
