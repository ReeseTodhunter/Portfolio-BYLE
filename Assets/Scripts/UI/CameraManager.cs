using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera gameCamera, minimapCamera;
    private bool isViewingMinimap = false;
    public Canvas mainCanvas;
    void Awake()
    {
        gameCamera.gameObject.SetActive(true);
        minimapCamera.gameObject.SetActive(false);
    }
    void Update()
    {
        if (GameManager.GMinstance.GetInputDown("keyMap") && (!PlayerController.instance.GetFreezeGameplayInput() || isViewingMinimap) && !PlayerController.instance.IsRolling())
        {
            SwapCameraStates();
            GameManager.GMinstance.Pause(isViewingMinimap);
        }
        if (isViewingMinimap && PlayerController.instance.IsInvulnerable())
        {
            SwapCameraStates();
            //GameManager.GMinstance.Pause(isViewingMinimap);
        }
    }
    private void SwapCameraStates()
    {
        //Flip bool
        isViewingMinimap = isViewingMinimap == true ? false : true;
        mainCanvas.gameObject.SetActive(!isViewingMinimap);
        PlayerController.instance.FreezeGameplayInput(isViewingMinimap);
        gameCamera.gameObject.SetActive(!isViewingMinimap);
        minimapCamera.gameObject.SetActive(isViewingMinimap);
    }
}
