using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkByleScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(PlayerController.instance.transform.position, this.gameObject.transform.position) <= this.gameObject.transform.localScale.x * 2)
        {
            if (GameManager.GMinstance.GetInputDown("keyInteract"))
            {
                /*   HUUUUUHHHHHHHHH
                if (PlayerController.instance.playerHitInLevel == false)
                {
                    AchievementSystem.UnlockAchievement(49);
                    AchievementSystem.Init();
                    SaveLoadSystem.instance.SaveAchievements();
                    GameManager.GMinstance.UnlockSteamAchievement("Byleaholic");
                }*/

                AchievementSystem.UnlockAchievement(49);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Byleaholic");

                PlayerController.instance.Damage(9999.0f);

                CanvasScript.instance.title.clearText2.text = "Level " + GameManager.GMinstance.level;
                CanvasScript.instance.title.roomClear = false;
                CanvasScript.instance.title.Start();

                CanvasScript.instance.roomClear.UI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            CanvasScript.instance.InteractText.text = $"PRESS '{GameManager.GMinstance.keyInteract}' TO CONSUME";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            CanvasScript.instance.InteractText.text = "";
        }

    }
}
