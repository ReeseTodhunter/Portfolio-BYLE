using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Forces a gameobject to always face the main camera
    void Start()
    {
        transform.rotation = Quaternion.Euler(45.0f, 0.0f, 0.0f);
    }
}
