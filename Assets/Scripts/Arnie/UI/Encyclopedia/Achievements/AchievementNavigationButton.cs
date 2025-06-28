using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementNavigationButton : MonoBehaviour
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
            AchievementDisplayController.instance.changePageNumber(number);
            pressed = false;

        }

    }

    public void OnPressTheButton()
    {
        ButtonBlipScript.instance.PlayBlip();
        pressed = true;
    }
}
