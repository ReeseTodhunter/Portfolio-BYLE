using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBorisMinigunController : MonoBehaviour
{
    /*
    * Controller for the big boris boss's weapon, allowing it to spin during animations
    * - Tom
    */
    public float desiredSpinSpeed = 0; 
    public float spinDecaySpeed = 90;
    public float spinWindUpSpeed = 90;
    public bool spinning = false;
    private float spinSpeed = 0;
    void Update()
    {
        if(spinning) //increase or decrese spin speed
        {
            WindUp();
        }
        else
        {
            WindDown();
        }
        this.transform.localEulerAngles += Vector3.forward * spinSpeed * Time.deltaTime; //apply spin speed
    }

    private void WindUp() //increase spin speed
    {
        spinSpeed += spinWindUpSpeed * Time.deltaTime;
        if(spinSpeed >= desiredSpinSpeed)
        {
            spinSpeed = desiredSpinSpeed;
        }
    }
    private void WindDown() //decrease spin speed
    {
        spinSpeed -= spinDecaySpeed * Time.deltaTime;
        if(spinSpeed <= 0)
        {
            spinSpeed = 0;
        }
    }
}
