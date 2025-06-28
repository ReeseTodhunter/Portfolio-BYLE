using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    public GameObject player;
    [SerializeField]
    public GameObject Self;


    public abstract void Interact();

    public void Start()
    {
        Self = this.gameObject;
        player = PlayerController.instance.gameObject;
    }

    // Update is called once per frame
    public abstract void Update();

    public void DistanceCheck()
    {
        
    }
}
