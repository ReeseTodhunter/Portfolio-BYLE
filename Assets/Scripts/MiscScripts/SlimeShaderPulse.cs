using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeShaderPulse : MonoBehaviour
{
    public float pulseRadius = 0.25f, pulseVelocity = 1f, pulseInterval = 1f;
    public float maxDistance = -.25f, minDistance = 2;
    public bool repeat = false;
    private bool enabled = true;
    private Material mat;
    private float distance = 2;
    void Awake()
    {
        mat = this.GetComponent<SpriteRenderer>().material;
        distance = maxDistance;
        enabled = true;
    }
    void Update()
    {
        if(!enabled){return;}
        mat.SetFloat("_radius", pulseRadius);
        distance -= pulseVelocity * Time.deltaTime;
        if(distance <= minDistance)
        {
            distance = maxDistance;
            enabled = repeat ? true : false;
        }
        mat.SetFloat("_distance", distance);
    }
}
