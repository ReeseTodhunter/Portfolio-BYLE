using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEvent : MonoBehaviour
{
    int cameraIndex = 0;
    public List<CameraPositions> CameraTransforms = new List<CameraPositions>();
    public Vector3 playerCameraPos;

    public BTAgent[] agents;
    public RoomController roomController;
    public bool roomActive = false;

    // Interpolation variables
    public float timeElapsed = 0;
    public float lerpDuration = 1;
    public float initialLerpDuration = 0;
    public bool lerping = false;
    public Vector3 startCamPos;
    public bool cutsceneTimerActive = false;
    public float cutsceneTimer = 0;


    // Start is called before the first frame update
    void Start()
    {
        foreach (BTAgent agent in agents)
        {
            agent.enabled = false;

            /////////
            //Debug.Log(agent.gameObject.name);
            agent.GetComponent<Collider>().enabled = false;
            agent.GetComponent<Rigidbody>().isKinematic = true;
        }

        roomController = this.gameObject.GetComponent<RoomController>();


        CameraTransforms[CameraTransforms.Count - 1].cameraPosition = PlayerController.instance.transform;
    }


    public virtual void Event()
    {
        //Code to do something in your rooms
        //return;
    }

    public void Cutscene(List<CameraPositions> cameraPositions)
    {
        Vector3 _nextPos = cameraPositions[cameraIndex].cameraPosition.position;
        lerpDuration = cameraPositions[cameraIndex].lerpDuration;


        // Interpolation makes the camera zoom in  on the desired object
        if (lerping && !cutsceneTimerActive)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed < lerpDuration)
            {
                CameraController.instance.transform.position = Vector3.Lerp(startCamPos, _nextPos, timeElapsed / lerpDuration);
            }
            else
            {
                // Once the lerp is nearly at the end it sets the position to the target position so that theres no short movement
                lerping = false;
                CameraController.instance.transform.position = _nextPos;
                cameraIndex += 1;
                timeElapsed = 0;
                if (cameraPositions.Count <= cameraIndex)
                {
                    CameraController.instance.transform.position = playerCameraPos;
                    CameraController.instance.SetCameraLocked(false);
                    PlayerController.instance.FreezeGameplayInput(false);
                    cameraIndex = 0;
                    
                    GameManager.GMinstance.currentState = GameManager.gameState.playing;
                    roomActive = false;
                    return;
                }
                else
                {
                    cutsceneTimerActive = true;
                }
            }

        }
        else
        {
            timeElapsed = 0;
        }

        if (cutsceneTimerActive)
        {
            cutsceneTimer += Time.deltaTime;
            Event();

            if (cutsceneTimer >= cameraPositions[cameraIndex - 1].cutsceneTimer)
            {
                cutsceneTimer = 0;
                cutsceneTimerActive = false;
            }

            foreach (BTAgent agent in agents)
            {
                agent.enabled = false;

                /////////
                agent.GetComponent<Collider>().enabled = true;
                agent.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
}

[System.Serializable]
public class CameraPositions
{
    public Transform cameraPosition;
    public float lerpDuration;
    public float cutsceneTimer;
}
