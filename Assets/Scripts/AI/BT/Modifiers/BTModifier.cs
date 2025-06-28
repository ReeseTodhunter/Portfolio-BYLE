using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTModifier : MonoBehaviour
{
    /*
     * These modifiers can be added to BT agents to give extra functionality
     * -Tom
    */
    public enum modifierType //when the modifier is updated
    {
        onDeath,
        onDissapear,
        onDamage,
        onUpdate,
        passive
    }
    public string title = ""; //what title is given to elite enemies that have this modifier
    void Awake()
    {
        if(this.TryGetComponent<BTAgent>(out BTAgent _agent)) //Add modifier to agent
        {
            _agent.AddBTModifier(this);
        }
    }
    public modifierType type; //current type
    public virtual void Initialise(Character _agent){}
    public virtual void ActivateModifier(Character _agent){}
}
