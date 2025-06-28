using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCube : Interactable
{

    public PlayerController playerScript;

    public GameObject Cube;
    public bool IsGrabbed;
    public bool UpdateCubePos = true;

    //This puzzle will be based off the colour of the cube and the receptacle
    //Colour must always be in lowercase, no spaces
    public string Colour = "red";
    // Start is called before the first frame update

    public override void Interact()
    {
        IsGrabbed = !IsGrabbed;
        if (IsGrabbed)
        {
            //Change this to be the direction the player is facing in
            //Do once
            if (UpdateCubePos)
            {
                //Currently Broken; Slapdash fix in place to allow me to progress
                //Self.transform.position = player.transform.position + (PlayerController.instance.targetPosition / 10);
                Self.transform.position = player.transform.position + new Vector3(0.3f,0.0f,0.3f);
            }
            Self.transform.SetParent(PlayerController.instance.transform, true);

            Debug.Log("Grabbed");
            //SelfInRange = true;
            UpdateCubePos = false;
        }
        else
        {
            Self.transform.parent = null;
            //SelfInRange = false;
            UpdateCubePos = true;
        }
    }
    // Update is called once per frame
    public override void Update()
    {
        DistanceCheck();
    }
}
