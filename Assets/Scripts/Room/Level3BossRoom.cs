using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3BossRoom : RoomEvent
{
    // Update is called once per frame
    private bool animationPlayed = false;

    private bool lerpPlayer = false;

    public GameObject playerFinalPos;

    void Update()
    {
        if (GameManager.GMinstance.currentState == GameManager.gameState.cutscene && roomActive)
        {
            foreach (BTAgent agent in agents)
            {
                agent.enabled = true;
                agent.SetSpawningFinished();
                EnemySpawningSystem.instance.DefaultSpawnerVariables();
                EnemySpawningSystem.instance.SubscribeEnemy(agent.GetComponent<BTAgent>());
            }
            /*foreach (BTAgent agent in agents)
            {
                agent.gameObject.SetActive(true);
            }*/

            if (lerpPlayer)
            {
                timeElapsed += Time.deltaTime;
                if (timeElapsed < lerpDuration)
                {
                    PlayerController.instance.transform.position = Vector3.Lerp(PlayerController.instance.transform.position, playerFinalPos.transform.position, timeElapsed / lerpDuration);
                    PlayerController.instance.transform.eulerAngles = Vector3.Lerp(PlayerController.instance.transform.eulerAngles, playerFinalPos.transform.eulerAngles, timeElapsed / lerpDuration);
                }
                else
                {
                    // Once the lerp is nearly at the end it sets the position to the target position so that theres no short movement
                    PlayerController.instance.transform.position = playerFinalPos.transform.position;
                    PlayerController.instance.transform.eulerAngles = playerFinalPos.transform.eulerAngles;
                }
            }

            if (!lerping)
            {
                startCamPos = CameraController.instance.transform.position;
            }
            PlayerController.instance.FreezeGameplayInput(true);
            lerping = true;
            Cutscene(CameraTransforms);

        }
    }

    public override void Event()
    {
        base.Event();
        if (animationPlayed) { return; }
        animationPlayed = true;
        lerpPlayer = true;

        

        foreach (BTAgent agent in agents)
        {
            //stuff for boss to do
            Debug.Log("Event");
            agent.GetComponent<Animator>().Play("SlimeEyeFlutter");
        }
    }
}
