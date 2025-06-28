using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : Action
{
    // GOP STUFF, NOT USED // Tom
    private float minAttackDistance;
    private float maxAttackDistance;
    private float attackDuration = .5f;
    private bool isActive = false;
    private float timer = 0;
    public void Initialise(GOB_AI _agent, float minDistance, float maxDistance)
    {
        minAttackDistance = minDistance;
        maxAttackDistance = maxDistance;
        agent = _agent;
    }
    public override bool IsActionPossible()
    {
        float distance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        return (distance < maxAttackDistance) ? true : false;
    }
    public override float GetActionScore()
    {
        float distance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        return (distance < maxAttackDistance) ? 10 : 0;
    }
    public override void BeginAction()
    {
        agent.ChangeAnimState(GOB_AI.animState.attack);
        agent.SetPerformingAction(true);
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        isActive = true;
        timer = 0;
    }
    public override void UpdateAction()
    {
        if(!isActive){return;}
        timer += Time.deltaTime;
        if(timer > attackDuration)
        {
            ExitAction();
        }
    }
    public override void ExitAction()
    {
        isActive = false;
        agent.SetPerformingAction(false);
    }
}
