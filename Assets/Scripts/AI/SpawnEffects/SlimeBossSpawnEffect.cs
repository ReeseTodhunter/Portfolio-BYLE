using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossSpawnEffect : SpawnEffect
{
    //NOT USED // ABANDONED CODE // TOM
    private enum state
    {
        cameraPan,
        byleRise,
        bubbles,
        BossSpawn,
        exit
    }
    private state currState;
    private Vector3 startCamPos,endCamPos;
    public GameObject upperCinematicBar, lowerCinematicBar;
    public Vector3 startUpper,endUpper,startLower,endLower;
    public float cameraPanDuration = 4, byleRiseDuration = 0, bubblesDuration = 0, bossSpawnDuration = 0;
    private float cameraPanTimer =0, byleRiseTimer, bubblesTimer = 0, bossSpawnTimer;
    public float camSpeed = 5;
    public override void Initialise(BTAgent parentAgent)
    {
        agent = parentAgent;
        currState = state.cameraPan;
        upperCinematicBar.SetActive(true);
        lowerCinematicBar.SetActive(true);
        CameraController.instance.SetCameraLocked(true);
        startCamPos = CameraController.instance.transform.position;
        Vector3 pos = transform.position;
        pos.y = PlayerController.instance.transform.position.y;
        PlayerController.instance.FreezeGameplayInput(true);
        endCamPos = pos + CameraController.instance.GetOffset();
        CanvasScript.instance.SetCanvasMode(CanvasScript.UIMode.CINEMATICMODE);
        Debug.Log("anim started");
    }
    void Update()
    {
        switch(currState)
        {
            case state.cameraPan:
                CameraPanUpdate();
                break;
            case state.byleRise:
                ByleRiseUpdate();
                break;
            case state.bubbles:
                BubbleUpdate();
                break;
            case state.BossSpawn:
                BossSpawnUpdate();
                break;
            case state.exit:
                EndSpawnEffect();
                break;
        }
    }
    private void CameraPanUpdate()
    {
        cameraPanTimer += Time.deltaTime;
        if(cameraPanTimer >= cameraPanDuration)
        {
            upperCinematicBar.GetComponent<RectTransform>().anchoredPosition = endUpper;
            lowerCinematicBar.GetComponent<RectTransform>().anchoredPosition = endLower;
            currState = state.byleRise; 
            return;
        }
        upperCinematicBar.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startUpper,endUpper,cameraPanTimer / cameraPanDuration);
        lowerCinematicBar.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startLower,endLower,cameraPanTimer / cameraPanDuration);
        CameraController.instance.transform.position = Vector3.Lerp(startCamPos,endCamPos,cameraPanTimer / cameraPanDuration);
    }
    private void ByleRiseUpdate()
    {
        byleRiseTimer += Time.deltaTime;
        if(byleRiseTimer >= byleRiseDuration){currState = state.bubbles; return;}
    }
    private void BubbleUpdate()
    {
        bubblesTimer+= Time.deltaTime;
        if(bubblesTimer >= bubblesDuration){currState = state.BossSpawn; return;}
    }
    private void BossSpawnUpdate()
    {
        bossSpawnTimer += Time.deltaTime;
        if(bossSpawnTimer >= bossSpawnDuration){currState = state.exit; return;}
    }
    private void EndSpawnEffect()
    {
        agent.SetSpawningFinished();
    }
}
