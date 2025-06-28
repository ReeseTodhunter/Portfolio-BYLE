using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevenantSpawnEffect : SpawnEffect
{
    /*
     * Spawn effect used by the Revenant mini-boss
     */
    private enum ScriptState //state of the animation
    {
        IDLE,
        ACTIVE,
        SPAWN,
        LINGER,
        EXIT
    }
    private float timer =0;
    public float activeDuration = 4, lingerDuration = 2; //duration of the animation, and how long it lingers for after the anim is done
    private ScriptState currState = ScriptState.IDLE; //current state
    public ParticleSystem activeSystem,lingerSystem; //Particle systems used in the animation
    public override void Initialise(BTAgent parentAgent) //init
    {
        agent = parentAgent;
        currState = ScriptState.ACTIVE;
        activeSystem.Play();
        agent.GetComponent<Collider>().enabled = false;
        agent.GetComponent<Rigidbody>().isKinematic = true;
    }
    void Update()
    {
        switch(currState)
        {
            case ScriptState.IDLE: //do nothing
                break;
            case ScriptState.ACTIVE: //wait until timer finished
                //Do stuff
                timer += Time.deltaTime;
                if(timer < activeDuration){break;}
                activeSystem.Stop(); //stop active effect
                lingerSystem.Play(); //play linger effect
                currState = ScriptState.SPAWN;
                timer = 0;
                break;
            case ScriptState.SPAWN: //spawn agent
                agent.SetSpawningFinished();
                agent.GetComponent<Collider>().enabled = true;
                agent.GetComponent<Rigidbody>().isKinematic = false;
                currState = ScriptState.LINGER;
                break;
            case ScriptState.LINGER: //linger a while
                timer += Time.deltaTime;
                if(timer < lingerDuration){break;}
                lingerSystem.Stop();
                currState = ScriptState.EXIT;
                break;
            case ScriptState.EXIT: //destroy this gameobject
                Destroy(this.gameObject);
                break;
        }
    }
}
