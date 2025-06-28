using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingScript : MonoBehaviour
{
    public float speed = 1;
    // Update is called once per frame
    public enum axis
    {
        forward,
        right,
        up
    }
    public axis spinAxis = axis.up;
    void Update()
    {
        Vector3 spin = Vector3.zero;
        switch(spinAxis)
        {
            case axis.forward:
                spin += Vector3.forward;
                break;
            case axis.right:
                spin += Vector3.right;
                break;
            case axis.up:
                spin += Vector3.up;
                break;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + spin * 50 * speed * Time.deltaTime);
    }
}
