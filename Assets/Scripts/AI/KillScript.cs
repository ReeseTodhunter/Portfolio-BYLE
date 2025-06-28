using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScript : MonoBehaviour
{
    /*
     * This kill script sees a lot of use when things need to be removed after a certain duration.
     * -Tom
     */
    public float lifeTime = 2;
    private float timer = 0;
    public bool decayScale = false;
    public float decayStart = 0;
    private float decayDuration;
    private Vector3 startScale;
    void Start()
    {
        if(decayScale)
            startScale = transform.localScale;
            decayDuration = lifeTime - decayStart;
    }
    void Update()
    {
        timer += Time.deltaTime;
        if(decayScale && timer >= decayStart)
        {
            float percentage = timer - decayStart;
            percentage /= decayDuration;
            transform.localScale = Vector3.Lerp(startScale,Vector3.zero, percentage);
        }
        if(timer > lifeTime){Destroy(this.gameObject);}
    }
}
