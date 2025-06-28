using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCanvas : MonoBehaviour
{

    public GameObject unloadCanvas;
    public GameObject loadCanvas;

    private bool pressed = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // IT IS BUGGING BECAUSE ENCYCLOPEDIA SCRIPT IS A INSTANCE

        if (pressed)
        {
            Debug.Log("loading");
            loadCanvas.SetActive(true);

            if (loadCanvas.TryGetComponent(out EncyclopediaScript encyclopediaScript))
            {
                encyclopediaScript.Awake();
            }

            unloadCanvas.SetActive(false);
            pressed = false;
        }
    }

    public void OnPressTheButton()
    {
        ButtonBlipScript.instance.PlayBlip();
        EncyclopediaScript.instance = null;
        pressed = true;
    }
}
