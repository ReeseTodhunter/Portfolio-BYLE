using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightningFlash : MonoBehaviour
{
    public float maxTimeBewteenFlashes = 10;
    public float minTimeBetweenFlashes = 2;
    public float maxIntensity = 8;
    public float minIntensity = 0;
    private float timer, waitDuration = 0;
    public Color byleLightning;
    private Light lightSource;
    private enum ScriptState
    {
        idle,
        waiting,
        flash,
        flashWindDown
    }
    private ScriptState currState = ScriptState.idle;
    void Start()
    {   
        currState = ScriptState.waiting;
        lightSource = this.GetComponent<Light>();
    }
    void Update()
    {
        switch(currState)
        {
            case ScriptState.idle:
                break;
            case ScriptState.waiting:
                timer += Time.deltaTime;
                if(timer < waitDuration){break;}
                timer = 0;
                currState = ScriptState.flash;
                break;
            case ScriptState.flash:
                lightSource.intensity = maxIntensity;
                lightSource.enabled = true;
                currState = ScriptState.flashWindDown;
                break;
            case ScriptState.flashWindDown:
                timer += Time.deltaTime;
                lightSource.intensity = Mathf.Lerp(maxIntensity,0,timer);
                if(timer < 1f){break;}
                lightSource.enabled = false;
                currState = ScriptState.waiting;
                waitDuration = Random.Range(minTimeBetweenFlashes,maxTimeBewteenFlashes);
                break;
        }
    }
    public void SetLightningEnabled(bool _enabled, bool _bileFlash = false)
    {
        lightSource.color = _bileFlash ? byleLightning : Color.white;
        lightSource.enabled = _enabled;
        if(_enabled)
        {
            currState = ScriptState.waiting;
        }
        else
        {
            currState = ScriptState.idle;
        }
    }
}
