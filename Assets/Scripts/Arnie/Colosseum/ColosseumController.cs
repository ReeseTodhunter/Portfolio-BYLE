using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColosseumController : MonoBehaviour
{
    private bool waveCleared = false;
    private bool shopSpawned = false;

    public int overallWaveCounter = 0;
    public int currentWaveCounter = 0;

    public GameObject[] wavesList;

    public GameObject currentWave;

    private float timer = 0;

    public GameObject ShopPrefab;
    public GameObject itemShopPrefab;
    private GameObject shop;

    // Health bar variables
    public GameObject ColosseumBarRoot;
    public Slider ColosseumBar;
    public TMPro.TextMeshProUGUI WaveCounterText;
    public TMPro.TextMeshProUGUI EnemiesAliveText;

    // Shop bar variables
    public GameObject ShopBarRoot;
    public Slider ShopBar;

    // Sets up Singleton
    public static ColosseumController instance;
    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.GMinstance.currentState = GameManager.gameState.playing;
        
        SpawnNewWave();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        // Puts the time between waves
        if ((timer > 2.5f & waveCleared == true) || (shop == null && shopSpawned == true))
        {
            // Achivement checks 
            if(currentWaveCounter == 10 && !PlayerController.instance.playerHitInRoom)
            {
                AchievementSystem.UnlockAchievement(51);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Surfing_Spirit");
            }
            if (currentWaveCounter == 20 && !PlayerController.instance.playerHitInRoom)
            {
                AchievementSystem.UnlockAchievement(52);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Wave_Phantom");
            }
            if (currentWaveCounter == 30 && !PlayerController.instance.playerHitInRoom)
            {
                AchievementSystem.UnlockAchievement(53);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Tsunami_Ghost");
            }

            if (currentWaveCounter == 30 && PlayerController.instance.secondaryWeapon == null)
            {
                AchievementSystem.UnlockAchievement(58);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Die_Hard");
            }




            // Spawns a different kind of shop every 5 waves
            if (currentWaveCounter % 4 == 0 && !shopSpawned)
            {
                ShopBarRoot.SetActive(true);
                ShopBar.maxValue = 7.5f;
                ShopBar.value = 7.5f;

                timer = -5;
                shop = Instantiate(itemShopPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                shopSpawned = true;
                return;
            }
            else if (currentWaveCounter % 4 == 2 && !shopSpawned)
            {
                ShopBarRoot.SetActive(true);
                ShopBar.maxValue = 7.5f;
                ShopBar.value = 7.5f;

                timer = -5;
                shop = Instantiate(ShopPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                shopSpawned = true;
                return;
            }

            // Checks if something has been bought from the shop 
            if (shop != null) 
            {
                shop.GetComponent<DealerScript>().ShopDestroy();
            }

            SpawnNewWave();
        }
        
        // Checks when a wave has been completed
        if (currentWave.GetComponentInChildren<BTAgent>() == null && !waveCleared)
        {
            ColosseumBarRoot.SetActive(false);

            if (currentWaveCounter != 0)
            {
                CanvasScript.instance.roomClear.clearText.text = "WAVE COMPLETED";
                CanvasScript.instance.roomClear.roomClear = false;
                CanvasScript.instance.roomClear.Start();
            }

            waveCleared = true;
            timer = 0;
        }


        // Sets the health bar values of the enemies
        float tempValueHealth = 0;
        int enemiesAlive = 0;
        foreach (BTAgent agent in currentWave.GetComponent<WaveController>().agents)
        {
            if (agent.GetHealth() > 0)
            {
                tempValueHealth += agent.GetHealth();
                enemiesAlive += 1;
            }
            
        }
        EnemiesAliveText.text = enemiesAlive + " Alive";
        ColosseumBar.value = tempValueHealth;

        if(ShopBarRoot.active == true)
        {
            ShopBar.value -= Time.deltaTime;
        }
    }



    void SpawnNewWave()
    {
        ColosseumBarRoot.SetActive(true);
        ShopBarRoot.SetActive(false);
        currentWaveCounter += 1;

        if(currentWaveCounter == 69)
        {
            AchievementSystem.UnlockAchievement(47);
            AchievementSystem.Init();
            SaveLoadSystem.instance.SaveAchievements();
            GameManager.GMinstance.UnlockSteamAchievement("Nice");
        }

        // Checks if abomination has been beat
        if(currentWaveCounter > 100)
        {
            overallWaveCounter = overallWaveCounter + 100;
            currentWaveCounter = 1;
        }

        if(overallWaveCounter > 99)
        {
            int rand = Random.Range(1, 100);
            currentWave = Instantiate(wavesList[rand - 1], new Vector3(0, 0, 0), Quaternion.identity);

        }
        else
        {
            currentWave = Instantiate(wavesList[currentWaveCounter - 1], new Vector3(0, 0, 0), Quaternion.identity);
        }

        

        WaveCounterText.text = "WAVE " +  (overallWaveCounter + currentWaveCounter);

        currentWave.GetComponent<WaveController>().UpdateAgentsArray();
        ColosseumBar.maxValue = 0;
        foreach (BTAgent agent in currentWave.GetComponent<WaveController>().agents)
        {
            ColosseumBar.maxValue += agent.GetHealth();
            EnemySpawningSystem.instance.SubscribeEnemy(agent);
        }

        
        shopSpawned = false;
        waveCleared = false;
        timer = 0;
    }
}

public enum WaveType
{
    EASY,
    MEDIUM,
    HARD
}

[System.Serializable]
public class Waves
{
    public WaveType difficulty;
    public GameObject wave;
}

