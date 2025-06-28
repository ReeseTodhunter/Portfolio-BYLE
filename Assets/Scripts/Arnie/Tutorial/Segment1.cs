using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Segment1 : MonoBehaviour
{
    public GameObject Segement2;
    public Text tutorialText;
    public GameObject interactText;
    public DoorLogic Door;
    public GameObject rollCoolDown;
    public TitleScript ObjectiveText;
    public GameObject Ammo;
    public GameObject Dummy;
    public ProjectileWeapon gun;
    public GameObject PlayerHealth;

    public bool actionPhase = false;

    float timer = 0;
    public int TextStage = 0;

    private bool w = false;
    private bool a = false;
    private bool s = false;
    private bool d = false;

    private bool space = false;
    public int spaceCounter = 0;

    private bool gunSet = false;

    public GameObject shotgun;
    public GameObject shotgunParent;

    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform pos4;
    public GameObject EnemyPrefab;
    public bool enemiesSpawned = false;
    public List<GameObject> activeEnemies;


    // Update is called once per frame
    void Update()
    {

        if(PlayerController.instance.GetHealth() <= 0)
        {
            SceneManager.LoadScene("CharacterSelectionScreen");
        }


        timer += Time.deltaTime;

        if (!actionPhase)
        {
            
            if (timer > 1.0f)
            {

                if (GameManager.GMinstance.GetInputDown("keyInteract") && GameManager.GMinstance.gamePaused == false)
                {
                    TextStage += 1;
                    timer = 0;
                    interactText.SetActive(false);
                }
            }
        }

        if (!actionPhase)
        {
            if (timer > 3.0f)
            {
                interactText.SetActive(true);
            }
        }


        if (TextStage == 0)
        {
            GameManager.GMinstance.currentState = GameManager.gameState.playing;

            tutorialText.text = "With the aid of this state of the art simulation we will have you out in the field in no time";
        }
        if (TextStage == 1)
        {
            tutorialText.text = "I'll be honest with you, we are losing men fast, makes you question the quality of the training...";
        }
        if (TextStage == 2)
        {
            tutorialText.text = "I'm sure it'll be different for you though... Anyway, it's time you learnt the basics ";
        }
        if (TextStage == 3)
        {
            tutorialText.text = $"USE THE '{GameManager.GMinstance.keyUp}{GameManager.GMinstance.keyLeft}{GameManager.GMinstance.keyDown}{GameManager.GMinstance.keyRight}' KEYS TO MOVE AROUND ";
            actionPhase = true;

            if (GameManager.GMinstance.GetInputDown("keyUp") && GameManager.GMinstance.gamePaused == false)
            {
                w = true;
            }
            if (GameManager.GMinstance.GetInputDown("keyLeft") && GameManager.GMinstance.gamePaused == false)
            {
                a = true;
            }
            if (GameManager.GMinstance.GetInputDown("keyDown") && GameManager.GMinstance.gamePaused == false)
            {
                s = true;
            }
            if (GameManager.GMinstance.GetInputDown("keyRight") && GameManager.GMinstance.gamePaused == false)
            {
                d = true;
            }

            if(w && a && s && d)
            {
                actionPhase = false;
                TextStage = 4;
                ObjectiveText.roomClear = false;
                ObjectiveText.Start();
                timer = 0;
            }
        }
        if (TextStage == 4)
        {
            tutorialText.text = "The way you move, its incredible, you're different to the rest";
        }
        if (TextStage == 5)
        {
            tutorialText.text = "Its all well and good just moving, but you'll need to be able to roll to make it out alive!";
        }
        if(TextStage == 6)
        {
            actionPhase = true;
            rollCoolDown.SetActive(true);
            tutorialText.text = $"PRESS '{GameManager.GMinstance.keyRoll}' TO ROLL, do it a few times for good measure";

            if (GameManager.GMinstance.GetInput("keyRoll") && GameManager.GMinstance.gamePaused == false && (GameManager.GMinstance.GetMovementVector2() != Vector2.zero))
            {
                /*if (PlayerController.instance.GetRollCooldown() == 1f)
                {
                    spaceCounter += 1;
                }*/
                spaceCounter += 1;
            }
            if (spaceCounter > 3)
            {
                actionPhase = false;
                ObjectiveText.roomClear = false;
                ObjectiveText.Start();
                TextStage = 7;
                timer = 0;
            }
            
        }
        if(TextStage == 7)
        {
            tutorialText.text = "I've never seen someone roll as well as you, I am filled with hope";
        }
        if (TextStage == 8)
        {
            tutorialText.text = "Remember use rolling to dodge through enemy bullets without taking damage!";
        }
        if (TextStage == 9)
        {
            Dummy.transform.position = new Vector3(0, 1.5f, 25);
            tutorialText.text = "I guess I should teach you how to shoot, this is the making of a soilder";
        }
        if (TextStage == 10)
        {
            
            tutorialText.text = "Point at the decoy and shoot at it, fire a few to really do some damage";
        }
        if(TextStage == 11)
        {
            
            actionPhase = true;
            if(gunSet == false)
            {
                gun.SetMagazineSize(18);
                gunSet = true;
            }
            Ammo.SetActive(true);
            tutorialText.text = $"LOOK AT THE DUMMY AND CLICK TO FIRE WEAPON, PRESS '{GameManager.GMinstance.keyReload}' IF YOU NEED TO RELOAD";

            if(Dummy == null)
            {
                gunSet = false;
                actionPhase = false;
                ObjectiveText.roomClear = false;
                ObjectiveText.Start();
                TextStage = 12;
                timer = 0;
            }
        }
        if(TextStage == 12)
        {
            tutorialText.text = "Woah you killed that dummy with ease, you are a natural";
        }
        if (TextStage == 13)
        {
            tutorialText.text = "It's worth noting you can carry more than one weapon, how about you pick one up and use it";
        }
        if (TextStage == 14)
        {
            tutorialText.text = "That shotgun will do nicely ";
            shotgun.transform.position = new Vector3(0, 1.5f, 25);
        }
        if(TextStage == 15)
        {
            actionPhase = true;
            tutorialText.text = $"PRESS '{GameManager.GMinstance.keyPickup}' WHILE NEXT TO A WEAPON TO PICK IT UP, USE SCROLL WHEEL TO CHANGE WEAPON ";

            if (PlayerController.instance.secondaryWeapon != null)
            {
                if(Input.mouseScrollDelta.y != 0)
                {
                    actionPhase = false;
                    ObjectiveText.roomClear = false;
                    ObjectiveText.Start();
                    TextStage = 16;
                    timer = 0;
                }
            }
        }
        if(TextStage == 16)
        {
            tutorialText.text = "Well, that is all the basics, you'll pick up the rest while you go at least I hope";
        }
        if(TextStage == 17)
        {
            tutorialText.text = "Now lets put it all together under pressure";
        }
        if(TextStage == 18)
        {
            tutorialText.text = "I'm gonna spawn some enemies into the simulation now, but don't be scared";
        }
        if (TextStage == 19)
        {
            tutorialText.text = "Even if you die in the simulation we will still send you into the field, so just have fun!";
        }
        if (TextStage == 20)
        {
            activeEnemies.RemoveAll(x => !x);

            actionPhase = true;
            PlayerHealth.SetActive(true);
            tutorialText.text = "They're here use what you've learnt, stop them all and prove this training is useful";

            if (!enemiesSpawned)
            {
                GameObject enemy1 = Instantiate(EnemyPrefab);
                enemy1.transform.position = pos1.position;
                activeEnemies.Add(enemy1);

                GameObject enemy2 = Instantiate(EnemyPrefab);
                enemy2.transform.position = pos2.position;
                activeEnemies.Add(enemy2);

                GameObject enemy3 = Instantiate(EnemyPrefab);
                enemy3.transform.position = pos3.position;
                activeEnemies.Add(enemy3);

                GameObject enemy4 = Instantiate(EnemyPrefab);
                enemy4.transform.position = pos4.position;
                activeEnemies.Add(enemy4);

                enemiesSpawned = true;
            }
            if (enemiesSpawned)
            {
                if(activeEnemies.Count == 0)
                {
                    actionPhase = false;
                    ObjectiveText.roomClear = false;
                    ObjectiveText.Start();
                    TextStage = 21;
                    timer = 0;
                }
            }
        }
        if(TextStage == 21)
        {
            tutorialText.text = "Wow you actually did it, maybe there is hope for this world yet...";
            AchievementSystem.UnlockAchievement(0);
            AchievementSystem.Init();
            SaveLoadSystem.instance.SaveAchievements();
            if (SteamManager.Initialized)
            {
                Steamworks.SteamUserStats.GetAchievement("Straight_Outta_Basic", out bool steamAchievement);
                if (!steamAchievement)
                {
                    SteamUserStats.SetAchievement("Straight_Outta_Basic");
                    SteamUserStats.StoreStats();
                }
            }

        }
        if (TextStage == 22)
        {
            tutorialText.text = $"Don't forget you can access the map with '{GameManager.GMinstance.keyMap}', and also look out for power ups";
        }
        if (TextStage == 23)
        {
            tutorialText.text = $"Some powerups you can use by pressing '{GameManager.GMinstance.keyAbility}', as they are active abilities";
        }
        if (TextStage == 24)
        {
            tutorialText.text = "Well I guess I will open the door so you can leave this simulation";
        }
        if (TextStage == 25)
        {
            Door.DoorManagement(false);
            tutorialText.text = "The door is a metaphor for you being set out into the world. Godspeed soldier.";
        }
    }
}
