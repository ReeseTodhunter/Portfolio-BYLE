using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossRoom : RoomEvent
{
    // Update is called once per frame
    private bool animationPlayed = false;

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
        foreach (BTAgent agent in agents)
        {
            //stuff for boss to do
            Debug.Log("Event");
            agent.GetComponent<Animator>().Play("SlimeEyeFlutter");
        }
    }
}
