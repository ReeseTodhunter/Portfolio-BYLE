using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissapearModifier : BTModifier
{
    /*
     * This modifier causes the agent to dissapear after a certain duration
     */
    [SerializeField] float duration = 10;
    float timer =0;
    public override void ActivateModifier(Character _agent)
    {
        timer += Time.deltaTime;
        if(timer < duration){return;}
        Destroy(this.gameObject);
    }
}
