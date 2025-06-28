using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossModifier : BTModifier
{
    /*
     * Boss modifier for agents, allows the game manager to recognise when a boss dies
     * - Tom
     */
    public override void Initialise(Character _agent)
    {
        type = modifierType.onDeath;
    }
    public override void ActivateModifier(Character _agent)
    {
        GameManager.GMinstance.bossDead = true;
        
        Debug.Log("Boss Died");
    }
}
