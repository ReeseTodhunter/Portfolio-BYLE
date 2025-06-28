using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPlayer : Action
{
    // GOP STUFF, NOT USED // Tom
    private float duration = 2;
    private float minDistance = 1.9f;
    private float speed = 4;
    bool isActive = false;
    private float timer = 0;
    public void Initialise(GOB_AI _agent)
    {
        agent = _agent;
    }
    public override bool IsActionPossible()
    {
        return true;
    }
    public override float GetActionScore()
    {
        float distance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        return distance;
    }
    public override void BeginAction()
    {
        agent.ChangeAnimState(GOB_AI.animState.running);
        isActive = true;
        timer = 0;
        agent.SetPerformingAction(true);
    }
    public override void UpdateAction()
    {
        if(!isActive){return;}
        timer += Time.deltaTime;
        if(timer >= duration){ExitAction(); return;}

        //Reset xz velocity of rb
        {
            Vector3 rbVelocity = this.GetComponent<Rigidbody>().velocity;
            rbVelocity.x = 0; rbVelocity.z = 0;
            this.GetComponent<Rigidbody>().velocity = rbVelocity;
        }

        Vector3 velocity = PlayerController.instance.transform.position - transform.position;
        velocity.y = 0;
        velocity.Normalize();
        this.GetComponent<Rigidbody>().velocity += velocity * speed;

        transform.LookAt(transform.position + velocity);
        if(Vector3.Distance(transform.position, PlayerController.instance.transform.position) < minDistance)
        {
            ExitAction();
        }
    }
    public override void ExitAction()
    {
        agent.SetPerformingAction(false);
        isActive = false;
    }
}
