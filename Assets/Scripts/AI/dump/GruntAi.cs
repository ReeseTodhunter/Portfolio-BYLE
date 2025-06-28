using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntAi : GOB_AI
{
    // GOP STUFF, NOT USED // Tom
    public Animator gruntAnimator;
    public void Awake()
    {
        gruntAnimator.SetBool("isAttacking",false);
        gruntAnimator.SetBool("isRunning",false);
        InitialiseAgent();
    }
    public override void InitialiseAgent()
    {
        MoveToPlayer move = gameObject.AddComponent<MoveToPlayer>();
        move.Initialise(this);
        actions.Add(move);
        AttackPlayer attack = gameObject.AddComponent<AttackPlayer>();
        attack.Initialise(this, 1.5f, 2f);
        actions.Add(attack);
        Initialised = true;
    }
    public void Update()
    {
        UpdateAI();
    }
    public override void ChangeAnimState(animState newState)
    {
        switch(newState)
        {
            case animState.idle:
                gruntAnimator.SetBool("isAttacking",false);
                gruntAnimator.SetBool("isRunning",false);
                break;
            case animState.running:
                gruntAnimator.SetBool("isRunning",true);
                gruntAnimator.SetBool("isAttacking",false);
                break;
            case animState.attack:
                gruntAnimator.SetBool("isRunning",false);
                gruntAnimator.SetBool("isAttacking",true);
                break;
        }
    }
}
