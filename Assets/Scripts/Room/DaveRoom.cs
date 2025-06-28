using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaveRoom : RoomEvent
{
    // Update is called once per frame
    void Update()
    {
        if (GameManager.GMinstance.currentState == GameManager.gameState.cutscene && roomActive)
        {
            foreach(BTAgent agent in agents)
            {
                agent.enabled = true;

                EnemySpawningSystem.instance.DefaultSpawnerVariables();
                EnemySpawningSystem.instance.SubscribeEnemy(agent.GetComponent<BTAgent>());
            }
            foreach (BTAgent agent in agents)
            {
                agent.gameObject.SetActive(true);
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
            agent.GetComponent<Animator>().Play("Throw");
        }

        foreach (BTAgent agent in agents)
        {
            agent.enabled = true;

            EnemySpawningSystem.instance.DefaultSpawnerVariables();
            EnemySpawningSystem.instance.SubscribeEnemy(agent.GetComponent<BTAgent>());
        }
    }
}
