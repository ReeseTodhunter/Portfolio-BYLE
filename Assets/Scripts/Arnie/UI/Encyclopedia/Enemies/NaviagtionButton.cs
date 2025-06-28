using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaviagtionButton : MonoBehaviour
{
    public int number = 1;
    public bool pressed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            ButtonBlipScript.instance.PlayBlip();
            EncyclopediaScript.instance.changePageNumber(number);
            pressed = false;

        }
        
    }

    public void OnPressTheButton()
    {
        pressed = true;
    }
}
