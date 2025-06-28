using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBackButton : MonoBehaviour
{
    public bool pressed = false;
    public GameObject characterParent;
    public GameObject LargeCharacterParent;
    public GameObject MediumCharacterParent;

    public Camera largeCamera;
    public Camera mediumCamera;
    public Camera smallCamera;


    private void Update()
    {
        if (pressed)
        {

            EncyclopediaScript.instance.SetCanvasMode(EncyclopediaScript.UIMode.ENCYCLOPEDIA);

            largeCamera.gameObject.SetActive(false);
            mediumCamera.gameObject.SetActive(false);
            smallCamera.gameObject.SetActive(true);

            foreach (Transform child in characterParent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in MediumCharacterParent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in LargeCharacterParent.transform)
            {
                Destroy(child.gameObject);
            }
            pressed = false;
        }


    }


    public void OnPressTheButton()
    {
        ButtonBlipScript.instance.PlayBlip();
        pressed = true;
    }
}
