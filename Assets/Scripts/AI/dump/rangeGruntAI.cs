using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedGruntAI : GOB_AI
{
    // GOP STUFF, NOT USED // Tom
    public Animator rangedGruntAnimator;
    public void Awake()
    {
        InitialiseAgent();
    }
    public override void InitialiseAgent()
    {
        
    }
    public void Update()
    {
        UpdateAI();
    }
    public override void ChangeAnimState(animState newState)
    {
    }

}
