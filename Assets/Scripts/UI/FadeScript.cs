using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : TitleScript
{
    bool freezePlayer;
    void Start()
    {
        freezePlayer = true;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
        base.Update();
        if(PlayerController.instance == null){return;} //make sure to null ref stuff guys
        if (!GameManager.GMinstance.gamePaused)
        {
            PlayerController.instance.FreezeGameplayInput(freezePlayer);
        }
    }

    protected override void DeactivateUI()
    {
        freezePlayer = false;
        base.DeactivateUI();
    }
}
