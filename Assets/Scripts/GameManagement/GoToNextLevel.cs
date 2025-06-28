using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNextLevel : MonoBehaviour
{
    public GameObject ropeLadder;
    public GameObject teleportPos;
    private bool lerping = true;
    private float timer;
    private float lerpDuration = 6.0f;
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = transform.parent.position + new Vector3(0,20,0) ;
        ropeLadder.GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.GMinstance.bossDead )
        {
            ropeLadder.GetComponent<Renderer>().enabled = true;
            if (lerping)
            {
                timer += Time.unscaledDeltaTime;
                if (timer < lerpDuration)
                {
                    EnemySpawningSystem.instance.KillAllEnemies();
                    transform.position = Vector3.Lerp(transform.position, teleportPos.transform.position, timer / lerpDuration);
                }
                else
                {
                    lerping = false;
                    //transform.position = 
                }
            }
            if(Vector3.Distance(PlayerController.instance.transform.position,this.gameObject.transform.position) <= this.gameObject.transform.localScale.x)
            {
                if (GameManager.GMinstance.GetInputDown("keyInteract"))
                {
                    if(PlayerController.instance.secondaryWeapon == null)
                    {
                        AchievementSystem.UnlockAchievement(39);
                        AchievementSystem.Init();
                        SaveLoadSystem.instance.SaveAchievements();
                        GameManager.GMinstance.UnlockSteamAchievement("Pistol_Connoisseur");
                    }


                    if (PlayerController.instance.playerHitInLevel == false)
                    {
                        AchievementSystem.UnlockAchievement(17);
                        AchievementSystem.Init();
                        SaveLoadSystem.instance.SaveAchievements();
                        GameManager.GMinstance.UnlockSteamAchievement("Part-time_Ghost");
                    }
                    PlayerController.instance.playerHitInLevel = false;
                    GameManager.GMinstance.OnWin();
			        CanvasScript.instance.InteractText.text = "";

                    if(GameManager.GMinstance.level % 3 == 0)
                    {
                        CanvasScript.instance.title.clearText.text = "The Sewer";
                    }
                    else if (GameManager.GMinstance.level % 3 == 2)
                    {
                        CanvasScript.instance.title.clearText.text = "The Park";
                    }
                    else if (GameManager.GMinstance.level % 3 == 1)
                    {
                        CanvasScript.instance.title.clearText.text = "The City";
                    }

                    CanvasScript.instance.title.clearText2.text = "Level " + GameManager.GMinstance.level;
                    CanvasScript.instance.title.roomClear = false;
                    CanvasScript.instance.title.Start();

                    CanvasScript.instance.roomClear.UI.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            CanvasScript.instance.InteractText.text = $"PRESS '{GameManager.GMinstance.keyInteract}' FOR NEXT LEVEL";
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
