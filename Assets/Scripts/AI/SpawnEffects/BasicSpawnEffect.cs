using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpawnEffect : SpawnEffect
{ 
    /*
     * Basic spawn effect used by most enemies 
     * -Tom
     */
    [SerializeField] float duration = 2; //duration of effet
    public float lingerDuration = 2;
    private float timer = 0;
    [SerializeField] SpriteRenderer ring; //ring sprite
    private bool isEnabled = false;
    [SerializeField] float startRadius = 0.4f; //start radius of ring
    [SerializeField] float endRadius = 0.05f; //end radius of ring
    [SerializeField] float startEmissive = 0; //start emissive value of ring color
    [SerializeField] float endEmissive = 2; //end emissive value of ring color
    public override void Initialise(BTAgent parentAgent) //start
    {
        agent = parentAgent;
        isEnabled = true;
        agent.GetComponent<Collider>().enabled = false;
        agent.GetComponent<Rigidbody>().isKinematic = true;
    }
    public void Update()
    {
        if(!isEnabled){return;}
        timer += Time.deltaTime;
        //Lerp values in anim
        float radius = Mathf.Lerp(startRadius,endRadius, timer / duration);
        float emission = Mathf.Lerp(startEmissive,endEmissive, timer / duration);
        ring.material.SetFloat("_radius", radius);
        ring.material.SetFloat("_emission", emission);
        if(timer >= duration) //spawn agent
        {
            agent.SetSpawningFinished();
            agent.GetComponent<Collider>().enabled = true;
            agent.GetComponent<Rigidbody>().isKinematic = false;
        }
        if(timer >= duration + lingerDuration)
        {
            Destroy(this.gameObject);
        }
    }
}
