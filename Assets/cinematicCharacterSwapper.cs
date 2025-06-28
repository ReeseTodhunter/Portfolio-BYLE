using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cinematicCharacterSwapper : MonoBehaviour
{
    public float timeBetweenCycles = 3;
    public float RamboDuration,OneEyeDuration,MikeDuration,VladDuration;
    public GameObject rambo,oneEye,Mike,Vlad;
    private float timer = 0;
    private enum scriptState
    {
        rambo,
        oneEye,
        mike,
        Vlad,
        inactive
    }
    private scriptState currState = scriptState.rambo;
    public void Update()
    {
        timer += Time.deltaTime;
        switch(currState)
        {
            case scriptState.rambo:
                if(timer < RamboDuration){break;}
                ResetLook();
                oneEye.SetActive(true);
                currState = scriptState.oneEye;
                timer = 0;
                break;
            case scriptState.oneEye:
                if(timer < OneEyeDuration){break;}
                ResetLook();
                Mike.SetActive(true);
                currState = scriptState.mike;
                timer = 0;
                break;
            case scriptState.mike:
                if(timer < MikeDuration){break;}
                ResetLook();
                Vlad.SetActive(true);
                currState = scriptState.Vlad;
                timer = 0;
                break;
            case scriptState.Vlad:
                if(timer < VladDuration){break;}
                ResetLook();
                rambo.SetActive(true);
                currState = scriptState.inactive;
                timer = 0;
                break;
            case scriptState.inactive:
                if(timer < timeBetweenCycles){break;}
                timer = 0;
                //ResetLook();
                currState = scriptState.rambo;
                break;
        }
    }
    private void ResetLook()
    {
        rambo.SetActive(false);
        oneEye.SetActive(false);
        Mike.SetActive(false);
        Vlad.SetActive(false);
    }
}
