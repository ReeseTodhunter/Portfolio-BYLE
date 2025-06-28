using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballRoom : RoomEvent
{
    // Update is called once per frame
    void Update()
    {
        if (GameManager.GMinstance.currentState == GameManager.gameState.cutscene && roomActive)
        {
            foreach (BTAgent agent in agents)
            {
                agent.enabled = true;

                EnemySpawningSystem.instance.DefaultSpawnerVariables();
                EnemySpawningSystem.instance.SubscribeEnemy(agent.GetComponent<BTAgent>());
            }

            if (!lerping)
            {
                startCamPos = CameraController.instance.transform.position;
            }

            Debug.Log("Cutscene Started");
            PlayerController.instance.FreezeGameplayInput(true);
            lerping = true;
            Cutscene(CameraTransforms);

        }
    }

    public override void Event()
    {
        base.Event();

        foreach (BTAgent agent in agents)
        {
            agent.gameObject.transform.LookAt(PlayerController.instance.transform.position);
        }
    }
}
