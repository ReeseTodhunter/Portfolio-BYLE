using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinningScript : MonoBehaviour
{
    public float rotSpeed = 1;
    void Start()
    {
        this.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
    }
}
