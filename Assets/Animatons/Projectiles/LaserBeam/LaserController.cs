using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    private LineRenderer rnd;
    public float width = 1;
    void Start()
    {
        rnd = this.GetComponent<LineRenderer>();
    }
    void Update()
    {
        rnd.startWidth = width; 
        rnd.endWidth =  width;
    }
}
