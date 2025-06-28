using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectCameraController : MonoBehaviour
{
    public Transform startOrient, characterOrient;
    private Transform startTransform, targetTransform;
    public GameObject menuUI, characterSelectUI;
    private enum scriptState
    {
        Idle,
        Active
    }
    private scriptState currState = scriptState.Idle;
    private float timer = 0;
    void Start()
    {
        transform.position = startOrient.position;
        transform.rotation = startOrient.rotation;
    }
    void Update()
    {
        Camera cam = this.GetComponent<Camera>();
        //Ensure field of view doesn't drop bellow 90
        cam.fieldOfView = 60 * (16f / 9f) / ((float)cam.pixelWidth / cam.pixelHeight);
        
        switch(currState)
        {
            case scriptState.Idle:
                break;
            case scriptState.Active:
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(startTransform.position,targetTransform.position,timer);
                transform.rotation = Quaternion.Lerp(startTransform.rotation,targetTransform.rotation,timer);
                if(timer <= 1){break;}
                transform.position = targetTransform.position;
                transform.rotation = targetTransform.rotation;
                currState = scriptState.Idle;
                break;
        }

    }
    public void OnCharacterSelect()
    {
        timer = 0;
        targetTransform = characterOrient;
        startTransform = startOrient;
        currState = scriptState.Active;
        menuUI.SetActive(false);
        characterSelectUI.SetActive(true);
    }
    public void OnMainMenu()
    {
        timer = 0;
        targetTransform = startOrient;
        startTransform = characterOrient;
        currState = scriptState.Active;
        menuUI.SetActive(true);
        characterSelectUI.SetActive(false);
    }
    public void SkipToMenu()
    {
        transform.position = startOrient.position;
        transform.rotation = startOrient.rotation;
    }
}
