using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPulseScript : MonoBehaviour
{
    /*
    * Script that makes the bomb on the back of kamikaze enemies glow brighter the 
    * closer it is to the player
    */
    public float maxDistance = 20;
    public MeshRenderer targetRenderer;
    public Transform agent;
    public void Update()
    {
        float distance =  Vector3.Distance(agent.position, PlayerController.instance.transform.position);
        //Percentage closeness to max distance   
        float percentage = 1 - (distance / maxDistance);

        percentage = percentage >= 1 ? 1 : percentage;
        percentage = percentage <= 0 ? 0 : percentage;
        targetRenderer.material.SetFloat("_FlashPercentage",percentage);
    }
}
