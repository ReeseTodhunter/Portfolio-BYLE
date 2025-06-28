using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixParentRotation : MonoBehaviour
{
    // This script is used to prevent a child object from rotating with it's parent
    // Currently being used for the canvas attached to the player (dash cooldown and other future stuff)

    Vector3 fixedRotation;

    void Awake()
    {
        fixedRotation = transform.rotation.eulerAngles;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}
