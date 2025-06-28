using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldModifier : BTModifier
{
    /*
     * This modifier allows the riot shield enemy to have a seperate riot shield, causing an animation when the shield is destroyed
     * -Tom
    */
    public ArmorEntity armorEntity; //shield entity
    public bool causesAnimation =false; //whether or not it causes an animation
    public string animationName = "";
    public override void Initialise(Character _agent)
    {
        if(armorEntity != null){armorEntity.SetListeningModifier(this);}
    }
    public void ShieldDestroyed()
    {
        if(causesAnimation)
        {
            this.GetComponent<Animator>().Play(animationName); //playing animation
        }
    }
}
