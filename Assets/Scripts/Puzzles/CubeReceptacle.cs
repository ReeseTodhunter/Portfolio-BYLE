using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeReceptacle : MonoBehaviour
{
    public GameObject cubeReceptacle;

    public GameObject Cube;

    //Colour must always be in lowercase, no spaces
    public string ReceptacleColour = "red";

    public bool CubeReceived = false;

    // Start is called before the first frame update
    void Start()
    {
        cubeReceptacle = this.gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Cube != null)
        {
            if (Vector3.Distance(Cube.transform.position, cubeReceptacle.transform.position) <= 0.5f)
            {
                if (ReceptacleColour == Cube.GetComponent<PuzzleCube>().Colour
                && Cube.GetComponent<PuzzleCube>().IsGrabbed == false)
                {
                    CubeReceived = true;
                }
            }
        }
        if(CubeReceived)
        {
            Cube.transform.position = this.transform.position;
            Debug.Log("Puzzle Complete");
            //Do a function like Opening a door or something
            //Use a enumerator or something so there are multiple functions that you can call to
            //depending on the use case
            //Locked Door
            //Stops enemies from spawning
            //
        }
        
    }

    private void OnTriggerEnter(Collider puzzleCube)
    {
        if (puzzleCube.GetComponent<PuzzleCube>() != null)
        {
            Cube = puzzleCube.gameObject;
        }
    }
}
