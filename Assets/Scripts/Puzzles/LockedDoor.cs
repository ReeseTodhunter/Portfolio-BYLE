using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Interactable
{
    [SerializeField]
    private bool RequireKey;

    public override void Interact()
    {
        if(/*SelfInRange &&*/ !RequireKey)
        {
            OpenDoor();
        }
    }

    public override void Update()
    {
        DistanceCheck();
    }

    private void OpenDoor()
    {
        transform.Translate(0.0f, -1.0f, 0.0f); //Replace with Unity Animation?
    }
}
