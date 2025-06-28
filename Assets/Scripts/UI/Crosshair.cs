using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Texture2D cursorCrosshair;
    bool isWeaponHeldInPistolPos;
    //public Texture2D mouseIcon;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.one * 64f;

    private bool crosshairChanged = true;

    public void SetCrosshair()
    {
        if (PlayerController.instance)
        {
            cursorCrosshair = PlayerController.instance.primaryWeapon.weaponCursor;
            isWeaponHeldInPistolPos = PlayerController.instance.primaryWeapon.weaponIsHeldInPistolPos;
            PlayerController.instance.model.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

            if (isWeaponHeldInPistolPos) { Cursor.SetCursor(cursorCrosshair, new Vector2(64, 64), cursorMode); }
            else
            {
                Cursor.SetCursor(cursorCrosshair, new Vector2(64, 64), cursorMode);
                PlayerController.instance.model.transform.localPosition = new Vector3(-0.5f, 0.0f, 0.0f);
            }
            crosshairChanged = false;
        }
        
    }
    private void Update()
    {
        if (GameManager.GMinstance == null) return;

        if (GameManager.GMinstance.currentState == GameManager.gameState.cutscene)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
        if (GameManager.GMinstance.currentState == GameManager.gameState.playing && crosshairChanged)
        {
            SetCrosshair();
        }
        
        else if(!crosshairChanged && (GameManager.GMinstance.currentState != GameManager.gameState.playing))
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            crosshairChanged = true; 
        }
    }
}
