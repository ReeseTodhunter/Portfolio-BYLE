using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvasAttach : MonoBehaviour
{
    Vector3 offset = Vector3.zero;
    bool active = false;

    private void Start()
    {
        if (PlayerController.instance != null) {
            active = true;
            offset = transform.position - PlayerController.instance.transform.position;
            transform.SetParent(null);
        }
    }
    private void Update()
    {
        if (active) transform.position = PlayerController.instance.transform.position + offset; 
    }
}
