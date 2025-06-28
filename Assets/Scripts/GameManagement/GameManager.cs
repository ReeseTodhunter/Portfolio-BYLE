using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    public static GameManager GMinstance;

    public GameObject canvas;

    public GameObject deathScreen;
    public GameObject ColosseumDeathScreen;
    public GameObject winScreen;

    private GameObject fadeScreen;
    private GameObject loseScreen = null;

    [SerializeField]
    public PlayerController player;

    [SerializeField]
    private int playerSpeed;

    [SerializeField]
    private int playerHealth;

    public bool gamePaused = false;
    public gameState currentState;

    //Settings
    public int startingDifficulty = 1;
    public int maxDifficulty = 1;

    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip encyclopediaMusic;
    public AudioClip colosseumMusic;

    [Range(0, 1)]
    protected float setMasterVolume;
    [Range(0, 100)]
    protected float setFXVolume;
    public float FXVolume { get => setFXVolume * setMasterVolume; }
    [Range(0, 100)]
    protected float setMusicVolume;
    public float MusicVolume { get => setMusicVolume * setMasterVolume; }


    private SaveData saveData;
    public SaveLoadSystem SaveLoadSystem;

    public PlayerController.SelectedCharacter selectedCharacter;


    public int startingLevel = 1; //Which level to start with

    public System.Random rand = new System.Random();
    public int levelSeed = 0;

    public List<GameObject> weapons = null;
    public List<GameObject> activePowerups = null;
    private bool dataLoaded = false;


    public List<HighScoreEntry> scoreList = new List<HighScoreEntry>();
    public int GameScore;

    public float gameTimer;

    private bool playerDead;
    public bool bossDead;

    public bool saveGame = false;
    public bool loadGame = false;

    //Variables that track data over the total playtime
    AchievementData achievementData;
    public int enemiesKilled;
    public int coinsCollected;
    public int roomsEntered;
    public float totalPlayTime;
    public bool[] weaponsPickedUp = new bool[30];
    public bool[] enemiesEncountered = new bool[69];
    public bool[] powerupsPickedUp = new bool[30];

    //Current run data
    public int level = 1;

    public float healthDifficultyScale = 0.0f;

    // Keybinds (no swap weapons since scroll wheel is an axis and therefore not a keycode)
    public KeyCode keyUp { get; private set; }
    public KeyCode keyDown { get; private set; }
    public KeyCode keyLeft { get; private set; }
    public KeyCode keyRight { get; private set; }
    public KeyCode keyRoll { get; private set; }
    public KeyCode keyAbility { get; private set; }
    public KeyCode keyPickup { get; private set; }
    public KeyCode keyInteract { get; private set; }
    public KeyCode keyShoot1 { get; private set; }
    public KeyCode keyShoot2 { get; private set; }
    public KeyCode keyReload { get; private set; }
    public KeyCode keyPause { get; private set; }
    public KeyCode keyMap { get; private set; }
    public KeyCode keyHUD { get; private set; }


    private void Awake()
    {
        
        if (PlayerPrefs.GetInt("chosenResolutionX") < 100|| PlayerPrefs.GetInt("chosenResolutionY") < 100)
        {
            PlayerPrefs.SetInt("chosenResolutionX", 1920);
            PlayerPrefs.SetInt("chosenResolutionY", 1080);
            PlayerPrefs.SetInt("resolutionArrayChoice", 13);
            PlayerPrefs.SetInt("isFullscreen", 1);
            Screen.SetResolution(PlayerPrefs.GetInt("chosenResolutionX"), PlayerPrefs.GetInt("chosenResolutionY"), true);
        }
        if (!PlayerPrefs.HasKey("masterVolume"))
        {
            PlayerPrefs.SetFloat("masterVolume", 1.0f);
        }
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1.0f);
        }
        if (!PlayerPrefs.HasKey("fxVolume"))
        {
            PlayerPrefs.SetFloat("fxVolume", 1.0f);
        }
            if (GMinstance != null)
        {
            if (GMinstance != this)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        GMinstance = this;
        DontDestroyOnLoad(GMinstance);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        scoreList = XMLScore.instance.LoadScores();
        player = PlayerController.instance;
        //If there is no level seed setup a random levelSeed
        if (levelSeed == 0)
        {
            SetRandomSeed();
        }
        else
        {
            SetSeededRandom(levelSeed);
        }
        UpdateSettings();
        AchievementData achievementData = SaveLoadSystem.instance.LoadAchievementData();

        Debug.Log("Achievement Data");
        Debug.Log(achievementData);

        SaveLoadSystem.instance.LoadAchievements();
        if (achievementData != null)
        {
            enemiesKilled = achievementData.enemiesKilled;
            coinsCollected = achievementData.coinsCollected;
            roomsEntered = achievementData.roomsEntered;
            totalPlayTime = achievementData.totalPlayTime;
            weaponsPickedUp = achievementData.weaponsPickedUp;
            enemiesEncountered = achievementData.enemiesEncountered;
            powerupsPickedUp = achievementData.powerupsPickedUp;
        }
        else
        {
            enemiesKilled = 0;
            coinsCollected = 0;
            roomsEntered = 0;
            totalPlayTime = 0;
            weaponsPickedUp = new bool[30];
            enemiesEncountered = new bool[69];
            powerupsPickedUp = new bool[30];
        }
        //Set the level to the startingLevel
        level = startingLevel;
        // Set keybinds
        LoadInputs();
    }
    
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "BUILD" && player == null)
        {
            player = PlayerController.instance;
            
            if (loadGame)
            {
                LoadPlayerData();
                LoadMapData();
            }
            else
            {
                player.currCharacter = selectedCharacter;
            }
        }

        if (SceneManager.GetActiveScene().name != "BUILD" && SceneManager.GetActiveScene().name != "Encyclopedia" && SceneManager.GetActiveScene().name != "Colosseum")
        {
            if(gameObject.GetComponent<AudioSource>().clip != menuMusic)
            {
                gameObject.GetComponent<AudioSource>().volume = MusicVolume;
                gameObject.GetComponent<AudioSource>().clip = menuMusic;
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else if (SceneManager.GetActiveScene().name == "BUILD" && SceneManager.GetActiveScene().name != "Encyclopedia")
        {
            if (gameObject.GetComponent<AudioSource>().clip != gameMusic)
            {
                gameObject.GetComponent<AudioSource>().volume = MusicVolume;
                gameObject.GetComponent<AudioSource>().clip = gameMusic;
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else if (SceneManager.GetActiveScene().name != "BUILD" && SceneManager.GetActiveScene().name == "Encyclopedia")
        {
            if (gameObject.GetComponent<AudioSource>().clip != encyclopediaMusic)
            {
                gameObject.GetComponent<AudioSource>().volume = MusicVolume;
                gameObject.GetComponent<AudioSource>().clip = encyclopediaMusic;
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else if (SceneManager.GetActiveScene().name != "BUILD" && SceneManager.GetActiveScene().name == "Colosseum")
        {
            if (gameObject.GetComponent<AudioSource>().clip != colosseumMusic)
            {
                gameObject.GetComponent<AudioSource>().volume = MusicVolume;
                gameObject.GetComponent<AudioSource>().clip = colosseumMusic;
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
        gameObject.GetComponent<AudioSource>().volume = MusicVolume;

        if (SceneManager.GetActiveScene().name == "BUILD" || SceneManager.GetActiveScene().name == "Colosseum" || SceneManager.GetActiveScene().name == "TutorialLevel")
        {
                totalPlayTime += Time.deltaTime;
            if (GetInputDown("keyPause") && CanvasScript.UIMode.PLAYMODE == CanvasScript.instance.GetCanvasMode() && !playerDead && !PauseMenu.instance.rebindUI.activeSelf && currentState != gameState.cutscene)
            {
                Debug.Log("Attempting Pause");
                bool active = PauseMenu.instance.GetUIEnabled() ? false : true;
                
                PauseMenu.instance.ToggleUI(active);
                PauseMenu.instance.settingsUI.SetActive(false);
                PauseMenu.instance.rebindUI.SetActive(false);
                gamePaused = active;
                Pause(active);
            }
            //Why was this here and in the pause menu!?!?!
            //if ((Input.GetKeyUp(KeyCode.P) || Input.GetKeyUp(KeyCode.Escape)) && currentState == gameState.paused)
            //{
            //    gamePaused = true;
            //}
            if (canvas == null)
            {
                canvas = GameObject.Find("Canvas"); //Really??? <--- this comment is so funny
                fadeScreen = GameObject.Instantiate(winScreen, canvas.transform);
                if (fadeScreen != null)
                {
                    //Place the fade screen
                    fadeScreen.transform.SetSiblingIndex(PauseMenu.instance.transform.GetSiblingIndex() - 1);
                }
            }
            if (!playerDead && currentState == gameState.playing)
            {
                if (gameTimer < 600.0f)
                {
                    gameTimer += Time.unscaledDeltaTime;
                }
                else
                {
                    gameTimer = 600.0f;
                }
            }

            

            //Achievements
            #region Achievements
                #region Enemy Kill
                if (enemiesKilled > 0 && !AchievementSystem.achievements[12])
                {
                    AchievementSystem.UnlockAchievement(12);
                    AchievementSystem.Init();
                    SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Welcome_To_The_Top");

                }
                if(enemiesKilled >= 100 && !AchievementSystem.achievements[13])
                {
                    AchievementSystem.UnlockAchievement(13);
                    AchievementSystem.Init();
                    SaveLoadSystem.instance.SaveAchievements();
                    UnlockSteamAchievement("Certified_Danger");

                }
                if (enemiesKilled >= 1000 && !AchievementSystem.achievements[14])
                {
                    AchievementSystem.UnlockAchievement(14);
                    AchievementSystem.Init();
                    SaveLoadSystem.instance.SaveAchievements();
                    UnlockSteamAchievement("Classified_Lethal_Weapon");
                }
                if(enemiesKilled >= 10000 && !AchievementSystem.achievements[15])
                {
                    AchievementSystem.UnlockAchievement(15);
                    AchievementSystem.Init();
                    SaveLoadSystem.instance.SaveAchievements();
                    UnlockSteamAchievement("This_Is_Getting_Crazy");
                }
            #endregion
            #region Enemy Encounters
            #region    Enemy ID List

            /* Basic Enemies With Guns:
             * 10 - Basic Shooter
             * 11 - Basic Auto Shooter
             * 12 - Better Auto Grunt
             * 13 - Basic Shotgunner
             * 14 - Best Shotgunner
             * 15 - Sniper
             * 16 - Riot Shield
             * 17 - Kamikaze
             * 18 - Soldier
             * 19 - Simulation Grunt
              
             * Enemies without Guns
             * 20 - Red Cultist
             * 21 - Purple Cultist
             * 22 - Green Cultist
             * 23 - Summoner/Sorcerer
             * 24 - Minion
             * 25 - Charger
             * 26 - Bruiser
             * 27 - Consoomer
             * 28 - Jockey
             * 29 - Cerebral
             * 30 - Hooligan
             * 31 - Hoarder
             * 32 - Large Slime
             * 33 - Medium Slime
             * 34 - Small Slime
              
             * Unique Enemies
             * 35 - Big Dave
             * 36 - Consoomer
             * 37 - Ruku
             * 38 - Revenant
              
             * NPCs
             * 39 - Shopkeeper
             * 40 - Shopkeeper FinalForm
             * 41 - Nurse
             * 
             * Bosses
             * 42 - Ruku
             * 43 - Revenant
             * 44 - Big Boris
             * 45 - SlimeBoss
             * 
             * Extra Enemies
             * 46 General
             * 47 Arsonist
             * 48 Mega Cultist
             * 49 Juggernaut
             * 50 Incinerator
             * 51 Armada
             */
            #endregion
            if (enemiesEncountered[20] == true && enemiesEncountered[21] == true && enemiesEncountered[22] == true && !AchievementSystem.achievements[22])
            {
                //Look, I got the whole set
                AchievementSystem.UnlockAchievement(22);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Look_I_Got_The_Whole_Set");
            }
            if (!AchievementSystem.achievements[23] && enemiesEncountered[26] 
                && enemiesEncountered[25] && enemiesEncountered[21] && enemiesEncountered[22]
                && enemiesEncountered[20] && enemiesEncountered[31] && enemiesEncountered[35]
                && enemiesEncountered[29] && enemiesEncountered[36] && enemiesEncountered[54]
                && enemiesEncountered[30] && enemiesEncountered[50] && enemiesEncountered[28] 
                && enemiesEncountered[49] && enemiesEncountered[48] && enemiesEncountered[24] 
                && enemiesEncountered[52] && enemiesEncountered[41] && enemiesEncountered[23]
                && enemiesEncountered[53] && enemiesEncountered[37])
            {
                //Achievement: Fantastical Warfare
                AchievementSystem.UnlockAchievement(23);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Fantastical_Warfare");
            }
            #endregion
            #region Weapon Collection
            if (!AchievementSystem.achievements[24])
            {

                weaponsPickedUp[19] = true;

                bool allWeaponsPickedUp = true;
                foreach(bool i in weaponsPickedUp)
                {
                    //Debug.Log(i);
                    if(!i)
                    {
                        allWeaponsPickedUp = false;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                if(allWeaponsPickedUp)
                {
                    AchievementSystem.UnlockAchievement(24);
                    AchievementSystem.Init();
                    SaveLoadSystem.instance.SaveAchievements();
                    UnlockSteamAchievement("Man_At_Arms");
                }
            }
            #endregion
            #region Powerup Collection
            if (PlayerController.instance.GetModifier(ModifierType.Speed) >= 1.25f && !AchievementSystem.achievements[55])
            {
                AchievementSystem.UnlockAchievement(55);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Speedy_Gonzales");
            }

            if (PlayerController.instance.GetModifier(ModifierType.Damage) >= 0.75f && !AchievementSystem.achievements[56])
            {
                AchievementSystem.UnlockAchievement(56);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Power_House");
            }

            if (PlayerController.instance.GetMaxHealth() >= 210f && !AchievementSystem.achievements[57])
            {
                AchievementSystem.UnlockAchievement(57);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Brick_Wall");
            }
            #endregion
            #region Coin Collection

            if (coinsCollected >= 10)
            {
                //Penny Pincher
                AchievementSystem.UnlockAchievement(25);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Penny_Pincher");
            }
            if(coinsCollected >= 1000)
            {
                //Stonks
                AchievementSystem.UnlockAchievement(26);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Stonks");
            }
            if(coinsCollected >= 10000)
            {
                //Byllionaire
                AchievementSystem.UnlockAchievement(27);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Byllionaire");
            }
            #endregion
            #endregion

            
        }
    }
    public enum gameState
    {
        //Readable enum so anything can tell what the gamestate is
        playing,
        paused,
        cutscene,
        loading

    }

    public void SetRandomSeed() //Reese 
    {
        //Using the current random create a new randomised seed to use
        levelSeed = rand.Next();
        //Update the random used
        rand = new System.Random(levelSeed);
    }

    public void SetSeededRandom(int seed = 0) //Reese
    {
        //Using a preset seed make a new random
        rand = new System.Random(seed);
    }
    
    //Pause Gameplay
    public void Pause(bool _isPaused)
    {
        //if paused set the gameState to paused otherwise update game state to playing
        if (_isPaused){currentState = gameState.paused;}
        else{currentState = gameState.playing;}

        //Freeze player and camera inputs 
        PlayerController.instance.FreezeGameplayInput(_isPaused);
        CameraController.instance.SetCameraLocked(_isPaused);

        //Set the time scale to 0
        Time.timeScale = _isPaused ? 0 : 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene("BUILD");
        GameManager.GMinstance.currentState = gameState.playing;
        playerDead = false;
    }

    public void ReturnToMainMenu()
    {
        level = startingLevel;
        GMinstance.Pause(false);
        currentState = gameState.loading;
        gamePaused = false;
        SceneManager.LoadScene("CharacterSelectionScreen");
        AchievementData achievementData = new AchievementData(enemiesKilled, coinsCollected, roomsEntered, totalPlayTime, weaponsPickedUp, enemiesEncountered, powerupsPickedUp);
        SaveLoadSystem.instance.SaveAchievementData(achievementData);
    }

    //Win/Lose Actions
    public void OnWin()
    {
        if(level == 3)
        {
            if (PlayerController.instance.currCharacter == PlayerController.SelectedCharacter.MIKE)
            {
                AchievementSystem.UnlockAchievement(29);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                //Check achievement condition
                UnlockSteamAchievement("Mike_Main");
                
            }
            if (PlayerController.instance.currCharacter == PlayerController.SelectedCharacter.OL_ONE_EYE)
            {
                AchievementSystem.UnlockAchievement(32);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("One_Eye_Optimist");

            }
            if (PlayerController.instance.currCharacter == PlayerController.SelectedCharacter.VLAD)
            {
                AchievementSystem.UnlockAchievement(31);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Vlad_Visionary");

            }
            if (PlayerController.instance.currCharacter == PlayerController.SelectedCharacter.DART)
            {
                AchievementSystem.UnlockAchievement(33);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Dart_Director");

            }
            if (PlayerController.instance.currCharacter == PlayerController.SelectedCharacter.RAMBO)
            {
                AchievementSystem.UnlockAchievement(30);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Rambo_Realist");
            }
            if (PlayerController.instance.currCharacter == PlayerController.SelectedCharacter.KARLOS)
            {
                AchievementSystem.UnlockAchievement(48);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Karlos_Conquest");
            }
            if (PlayerController.instance.currCharacter == PlayerController.SelectedCharacter.XERXES)
            {
                AchievementSystem.UnlockAchievement(50);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                UnlockSteamAchievement("Xerxes_Extremist");
            }
        }

        if (level == 20)
        {
            AchievementSystem.UnlockAchievement(34);
            AchievementSystem.Init();
            SaveLoadSystem.instance.SaveAchievements();
            UnlockSteamAchievement("Pure_Commitment");
        }

        if (player.GetHealthRatio() < 0.1f)
        {
            AchievementSystem.UnlockAchievement(35);
            AchievementSystem.Init();
            SaveLoadSystem.instance.SaveAchievements();
            UnlockSteamAchievement("Narrow_Escape");
        }


        //Debug.Log("Early Player Score: " + PlayerController.instance.score);
        PlayerController.instance.FreezeGameplayInput(true);
        
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<FadeScript>().Start();

        //Delete floor items
        List<GameObject> floorItems = new List<GameObject>();
        GameObject[] gameObjectsTaggedPlayer = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject gameObject in gameObjectsTaggedPlayer)
        {
            if((gameObject.GetComponent<ProjectileWeapon>() != null && gameObject.GetComponent<ProjectileWeapon>().currState == BaseWeapon.WeaponState.DROPPED) || (gameObject.GetComponent<BaseWeapon>() != null && gameObject.GetComponent<BaseWeapon>().currState == BaseWeapon.WeaponState.DROPPED))
            {
                Debug.Log(gameObject.name);
                floorItems.Add(gameObject);
            }
            if(gameObject.GetComponent<BasePowerup>() != null || gameObject.GetComponent<BaseActivePowerup>() != null && gameObject.transform.root.GetComponent<PlayerController>() == null)
            {
                Debug.Log(gameObject.name);
                floorItems.Add(gameObject);
            }

        }
        for(int i = 0; i < floorItems.Count; i++)
        {
            Destroy(floorItems[i]);
        }

        //Apply score for speed
        if (gameTimer < 300)
        {
            GameScore = PlayerController.instance.score + 900;
        }
        else if (gameTimer < 600 && gameTimer > 300)
        {
            GameScore = PlayerController.instance.score + 600;
        }
        else if (gameTimer < 900 && gameTimer > 600)
        {
            GameScore = PlayerController.instance.score + 300;
        }
        else
        {
            GameScore = PlayerController.instance.score;
        }

        //Reset timer
        gameTimer = 0.0f;
        PlayerController.instance.score = GameScore;

        //============================================================================ Reese ============================================================================
        WeatherSystem weather = WeatherSystem.instance;
        if(level%3 == 0) healthDifficultyScale++;
        level++; //increment the level
        

        //Check Weather
        if ((level % 3) != 0)
        {
            weather.RandomiseTime();
            weather.RandomiseWeather();
        }
        else
        {
            if(Random.Range(0,2) == 0) weather.SetWeather(WeatherSystem.WeatherType.byleDust);
            else weather.SetWeather(WeatherSystem.WeatherType.Fog);
            weather.SetTimeOfDay(WeatherSystem.DayTime.Day);
            
        }

        if (level == 1) //if on the opening level
        {
            GameController.instance.currentLevel = 2; //Set the level to the next level
            EnemySpawningSystem.instance.ResetDifficulty(); //Reset the difficulty back to base
        }
        else
        {
            int newLevel = level % 3; //Get the modulus of the new level
            if (newLevel == 0) newLevel += 3; //If the modulus is 0 set the level to the 3rd level
            GameController.instance.currentLevel = newLevel; //Update the current level
        }

        //Reset the player to the starting hub position
        player.transform.position = new Vector3(0.0f, 1.0f, 10.0f);

        SetRandomSeed(); //On completing a level make a new level seed using the current seed for debugging later
        GameController.instance.MapGeneration(); //Re-generate the map with the new seed
        //===============================================================================================================================================================


        SaveGameData();

        bossDead = false;
        
        player.FreezeGameplayInput(false);

        AchievementSystem.UnlockAchievement(6);
        AchievementSystem.Init();
        SaveLoadSystem.instance.SaveAchievements();
        UnlockSteamAchievement("Explorer");

        AchievementData achievementData = new AchievementData(enemiesKilled, coinsCollected, roomsEntered, totalPlayTime,weaponsPickedUp,enemiesEncountered,powerupsPickedUp);
        SaveLoadSystem.instance.SaveAchievementData(achievementData);
    }

    public void OnLose()
    {
        if (SceneManager.GetActiveScene().name == "Colosseum")
        {
            if (loseScreen == null)
            {
                //Clear the Save Data
                saveData = null;
                SaveLoadSystem.instance.ResetSave();
                //Make new function to delete the save data

                // Display Score,
                loseScreen = GameObject.Instantiate(ColosseumDeathScreen, canvas.transform);
                GameScore = PlayerController.instance.score;
                //Reduce Player Score to zero
                PlayerController.instance.score = 0;
                //Ask to type name in
                //Pass player score into highscore table if it's high enough
                playerDead = true;
                PlayerController.instance.FreezeGameplayInput(true);
                PlayerController.instance.Protect(9999.0f);
                //Reset back to the startingLevel
                level = startingLevel;
                levelSeed = 0;
            }
        }
        else
        {
            if (loseScreen == null)
            {
                //Clear the Save Data
                saveData = null;
                SaveLoadSystem.instance.ResetSave();
                //Make new function to delete the save data

                // Display Score,
                loseScreen = GameObject.Instantiate(deathScreen, canvas.transform);
                GameScore = PlayerController.instance.score;
                //Reduce Player Score to zero
                PlayerController.instance.score = 0;
                //Ask to type name in
                //Pass player score into highscore table if it's high enough
                playerDead = true;
                PlayerController.instance.FreezeGameplayInput(true);
                PlayerController.instance.Protect(9999.0f);
                //Reset back to the startingLevel
                level = startingLevel;
                levelSeed = 0;
            }
        }
        
        AchievementData achievementData = new AchievementData(enemiesKilled, coinsCollected, roomsEntered, totalPlayTime, weaponsPickedUp, enemiesEncountered, powerupsPickedUp);
        SaveLoadSystem.instance.SaveAchievementData(achievementData);
    }

    //Updates the leaderboard saved on disk, adding the user's name with their score to the list
    public void UpdateScore(string userName)
    {
        if (GameScore > 0)
        {
            if(userName == "")
            {
                userName = "N/A";
            }
            SortScore(GameScore, userName);
            XMLScore.instance.SaveScores(scoreList);
            GameScore = 0;
            gameTimer = 0.0f;
        }
        playerDead = false;
        
    }

    //Iterates through the leaderboard, sorting highscores from highest to lowest
    public void SortScore(int score, string name)
    {
        string tempName;
        int tempScore;
        if (scoreList.Count > 0)
        {
            //Sorts through the highscore list, order from highest to lowest
            //Checks this player's score against each entry
            for (int i = 0; i < scoreList.Count; i++)
            {
                if (scoreList[i].score < score)
                {
                    tempName = scoreList[i].name;
                    tempScore = scoreList[i].score;
                    scoreList[i].name = name;
                    scoreList[i].score = score;
                    name = tempName;
                    score = tempScore;
                }
            }
            if (scoreList.Count < 10)
            {
                //Uses the name and score, adds it to a struct in order to display on the leaderboard
                scoreList.Add((new HighScoreEntry { name = name, score = score }));
            }
        }
        else
        {
            scoreList.Add((new HighScoreEntry { name = name, score = score }));
        }
    }

    //Saves current player progress including: Current weapons, current powerups, current abilities, their selected characters
    //  what level they're on and the level seed - so that the game can regenerate the level to be the same when they continue
    //This function is only called when a player completes a level
    public void SaveGameData()
    {
        PlayerController player = PlayerController.instance;

        List<int> savedModTypes = new List<int>();
        List<float> savedModAmounts = new List<float>();

        //Gets a list of all the player powerups and how much each pickup modifies their stats
        for(int i = 0; i < player.GetModifierList().Count; i++)
        {
            savedModTypes.Add((int)player.GetModifierList()[i].type);
            savedModAmounts.Add(player.GetModifierList()[i].value);
        }

        string weapon2 = "";
        if(player.secondaryWeapon != null)
        {
            //All weapons spawned are instatiated so have (Clone) at the end of their names,
            //we save the name of the weapon and match it to the list of weapons on the GameManager prefab
            //so we need to remove the (Clone) so that it can be found
            weapon2 = player.secondaryWeapon.name.Replace("(Clone)", "").Trim();
        }

        string ability = "";

        if (player.currentAbility != null)
        {
            //Same problem as the weapons above. (Clone) needs to be removed from the name so it can be found
            ability = player.currentAbility.name.Replace("(Clone)", "").Trim();
        }

        //Save data takes the Current weapons, current powerups, current abilities, their selected characters
        //  what level they're on and the level seed
        // And stores it in a binary file
        saveData = new SaveData("test", GameScore, player.GetHealth(),
                        player.primaryWeapon.name.Replace("(Clone)", "").Trim(), weapon2, 
                        savedModTypes, savedModAmounts,
                        player.currCharacter, ability,
                        level, levelSeed);

        SaveLoadSystem.instance.SaveGame(saveData);
        //Unlocked achievements are also saved, in a different file so if player data is deleted the achievements are persistent
        SaveLoadSystem.instance.SaveAchievements();
        //Achievement Data is the intermediary data that tracks overall progress towards achievements like:
        // total enemy kills, total coins collected and total playtime.
        AchievementData achievementData = new AchievementData(enemiesKilled, coinsCollected, roomsEntered, totalPlayTime, weaponsPickedUp, enemiesEncountered, powerupsPickedUp);
        SaveLoadSystem.instance.SaveAchievementData(achievementData);
    }

    public void LoadMapData() //Reese
    {
        //Find the save data file
        saveData = SaveLoadSystem.LoadGame();
        //If there is a saveData
        if (saveData != null)
        {
            levelSeed = saveData.levelSeed; //Load in the level seed
            level = saveData.currentLevel; //Load in the level the player made it to
            int newLevel = level % 3; //Get the modulus of the level
            if (newLevel == 0) newLevel += 3; //If the modulus is 0 set the level to the 3rd level
            GameController.instance.currentLevel = newLevel; //Update the current level
            GameController.instance.GenerateMapWithSeed(); //Generate the map using the levelSeed
        }
    }

    public void LoadPlayerData()
    {
        //Find the save data file.
        saveData = SaveLoadSystem.LoadGame();
        SaveLoadSystem.instance.LoadAchievements();
        if (saveData != null)
        {
            if (player != null)
            {
                //If the saveData exists, and when the player exists
                //Pass in all previous progress from their last session
                //Essentially allowing them to continue from where they left off
                loadGame = false;

                selectedCharacter = saveData.savedCharacter;

                for (int i = 0; i < saveData.modifierType.Count; i++)
                {
                    //Adds all saved powerups to the player
                    player.AddModifier((ModifierType)saveData.modifierType[i], saveData.modAmount[i]);
                    Debug.Log((ModifierType)saveData.modifierType[i] + " " + saveData.modAmount[i]);
                }

                //Set the player's character to the saved character that they selected last time
                player.SetPlayerCharacter();

                GameObject PrimaryWeapon = null;
                GameObject SecondaryWeapon = null;
                foreach (var weapon in GMinstance.weapons)
                {
                    //Matches the saved weapon to one in the Game Manager's list to give to the player
                    if (saveData.weapon1 == weapon.name)
                    {
                        PrimaryWeapon = weapon;
                    }
                    if (saveData.weapon2 == weapon.name)
                    {
                        SecondaryWeapon = weapon;
                    }
                }
                //Null check in case the player never picks up 2 weapons
                if(SecondaryWeapon != null)
                {
                    //If the player has 2 weapons give them both
                    player.SetSecondaryWeapon(Instantiate(SecondaryWeapon));
                }
                
                player.SetMainWeapon(Instantiate(PrimaryWeapon));

                GameObject savedPowerup = null;
                foreach (var powerUp in GMinstance.activePowerups)
                {
                    //Matches the saved ability to one in the Game Manager's list to give to the player
                    if (saveData.activeAbility == powerUp.name)
                    {
                        savedPowerup = powerUp;
                    }
                }

                Destroy(player.currentAbility.gameObject);
                savedPowerup = Instantiate(savedPowerup);
                player.PickupAbility(savedPowerup.GetComponent<BaseActivePowerup>());

                //Give the player their saved score
                player.score = saveData.score;

                //Summary
                //Loads all the data into the player character
                //Pushes the level seed to the Room Generator
                //Game has all necessary data to allow the player to continue from their last play session
            }
        }
        else
        {
            Debug.Log("No Save File");
            loadGame = false;
            //If there is no save file/it cannot be found
            //stop attempting to load data into player
        }
    }

    public void UpdateSettings()
    {
        //Update the game settings to any previously saved settings
        setMasterVolume = PlayerPrefs.GetFloat("masterVolume");
        setFXVolume = PlayerPrefs.GetFloat("fxVolume");
        setMusicVolume = PlayerPrefs.GetFloat("musicVolume");

        startingDifficulty = PlayerPrefs.GetInt("difficulty");        
        bool fullscreen = true;
        if (PlayerPrefs.GetInt("isFullscreen") == 0) fullscreen = false;
        Screen.SetResolution(PlayerPrefs.GetInt("chosenResolutionX"),PlayerPrefs.GetInt("chosenResolutionY"),fullscreen);
    }

    public void LoadStatistics()
    {
        AchievementData achievementData = SaveLoadSystem.instance.LoadAchievementData();
        SaveLoadSystem.instance.LoadAchievements();
        if (achievementData != null)
        {
            enemiesKilled = achievementData.enemiesKilled;
            coinsCollected = achievementData.coinsCollected;
            roomsEntered = achievementData.roomsEntered;
            totalPlayTime = achievementData.totalPlayTime;
            weaponsPickedUp = achievementData.weaponsPickedUp;
            enemiesEncountered = achievementData.enemiesEncountered;
            powerupsPickedUp = achievementData.powerupsPickedUp;
        }
        else
        {
            enemiesKilled = 0;
            coinsCollected = 0;
            roomsEntered = 0;
            totalPlayTime = 0;
            weaponsPickedUp = new bool[30];
            enemiesEncountered = new bool[69];
            powerupsPickedUp = new bool[30];
        }
    }

    public void ClearAllData()
    {
        SaveLoadSystem.instance.ResetSave();
        SaveLoadSystem.instance.ResetData();
        SaveLoadSystem.instance.ResetAchievements();
        LoadStatistics();
        SaveLoadSystem.instance.SaveAchievements();
    }

    public void UnlockSteamAchievement(string nameOfAchievementStoredOnSteam)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement(nameOfAchievementStoredOnSteam, out bool steamAchievement);
            if (!steamAchievement)
            {
                SteamUserStats.SetAchievement(nameOfAchievementStoredOnSteam);
                SteamUserStats.StoreStats();
            }
        }
    }

    public void IncrementSteamStatsForAchievements(string nameOfStatForSteam, int amountToIncrementBy)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.GetStat(nameOfStatForSteam, out int StatToBeIncremented);
            SteamUserStats.SetStat(nameOfStatForSteam, StatToBeIncremented + amountToIncrementBy);
            SteamUserStats.StoreStats();
        }
    }

    // INPUT
    public bool GetInput(string keybind)
    {
        switch (keybind)
        {
            case "keyUp":
                return Input.GetKey(keyUp);
            case "keyDown":
                return Input.GetKey(keyDown);
            case "keyLeft":
                return Input.GetKey(keyLeft);
            case "keyRight":
                return Input.GetKey(keyRight);
            case "keyRoll":
                return Input.GetKey(keyRoll);
            case "keyAbility":
                return Input.GetKey(keyAbility);
            case "keyPickup":
                return Input.GetKey(keyPickup);
            case "keyInteract":
                return Input.GetKey(keyInteract);
            case "keyShoot1":
                return Input.GetKey(keyShoot1);
            case "keyShoot2":
                return Input.GetKey(keyShoot2);
            case "keyReload":
                return Input.GetKey(keyReload);
            case "keyPause":
                return Input.GetKey(keyPause);
            case "keyMap":
                return Input.GetKey(keyMap);
            case "keyHUD":
                return Input.GetKey(keyHUD);
            default:
                Debug.Log($"Invalid input '{keybind}' was used");
                return false;
        }
    }
    public bool GetInputDown(string keybind)
    {
        switch (keybind)
        {
            case "keyUp":
                return Input.GetKeyDown(keyUp);
            case "keyDown":
                return Input.GetKeyDown(keyDown);
            case "keyLeft":
                return Input.GetKeyDown(keyLeft);
            case "keyRight":
                return Input.GetKeyDown(keyRight);
            case "keyRoll":
                return Input.GetKeyDown(keyRoll);
            case "keyAbility":
                return Input.GetKeyDown(keyAbility);
            case "keyPickup":
                return Input.GetKeyDown(keyPickup);
            case "keyInteract":
                return Input.GetKeyDown(keyInteract);
            case "keyShoot1":
                return Input.GetKeyDown(keyShoot1);
            case "keyShoot2":
                return Input.GetKeyDown(keyShoot2);
            case "keyReload":
                return Input.GetKeyDown(keyReload);
            case "keyPause":
                return Input.GetKeyDown(keyPause);
            case "keyMap":
                return Input.GetKeyDown(keyMap);
            case "keyHUD":
                return Input.GetKeyDown(keyHUD);
            default:
                Debug.Log($"Invalid input '{keybind}' was used");
                return false;
        }
    }
    public bool GetInputUp(string keybind)
    {
        switch (keybind)
        {
            case "keyUp":
                return Input.GetKeyUp(keyUp);
            case "keyDown":
                return Input.GetKeyUp(keyDown);
            case "keyLeft":
                return Input.GetKeyUp(keyLeft);
            case "keyRight":
                return Input.GetKeyUp(keyRight);
            case "keyRoll":
                return Input.GetKeyUp(keyRoll);
            case "keyAbility":
                return Input.GetKeyUp(keyAbility);
            case "keyPickup":
                return Input.GetKeyUp(keyPickup);
            case "keyInteract":
                return Input.GetKeyUp(keyInteract);
            case "keyShoot1":
                return Input.GetKeyUp(keyShoot1);
            case "keyShoot2":
                return Input.GetKeyUp(keyShoot2);
            case "keyReload":
                return Input.GetKeyUp(keyReload);
            case "keyPause":
                return Input.GetKeyUp(keyPause);
            case "keyMap":
                return Input.GetKeyUp(keyMap);
            case "keyHUD":
                return Input.GetKeyUp(keyHUD);
            default:
                Debug.Log($"Invalid input '{keybind}' was used");
                return false;
        }
    }
    public void ResetInputs()
    {
        keyUp = KeyCode.W;
        keyDown = KeyCode.S;
        keyLeft = KeyCode.A;
        keyRight = KeyCode.D;
        keyRoll = KeyCode.Space;
        keyAbility = KeyCode.LeftShift;
        keyPickup = KeyCode.E;
        keyInteract = KeyCode.F;
        keyShoot1 = KeyCode.Mouse0;
        keyShoot2 = KeyCode.Mouse1;
        keyReload = KeyCode.R;
        keyPause = KeyCode.Escape;
        keyMap = KeyCode.M;
        keyHUD = KeyCode.Tab;

        SaveInputs();
    }
    public void RebindInput(string keybind, KeyCode newKey)
    {
        switch (keybind)
        {
            case "keyUp":
                keyUp = newKey;
                PlayerPrefs.SetInt("keyUp", (int)keyUp);
                break;
            case "keyDown":
                keyDown = newKey;
                PlayerPrefs.SetInt("keyDown", (int)keyDown);
                break;
            case "keyLeft":
                keyLeft = newKey;
                PlayerPrefs.SetInt("keyLeft", (int)keyLeft);
                break;
            case "keyRight":
                keyRight = newKey;
                PlayerPrefs.SetInt("keyRight", (int)keyRight);
                break;
            case "keyRoll":
                keyRoll = newKey;
                PlayerPrefs.SetInt("keyRoll", (int)keyRoll);
                break;
            case "keyAbility":
                keyAbility = newKey;
                PlayerPrefs.SetInt("keyAbility", (int)keyAbility);
                break;
            case "keyPickup":
                keyPickup = newKey;
                PlayerPrefs.SetInt("keyPickup", (int)keyPickup);
                break;
            case "keyInteract":
                keyInteract = newKey;
                PlayerPrefs.SetInt("keyInteract", (int)keyInteract);
                break;
            case "keyShoot1":
                keyShoot1 = newKey;
                PlayerPrefs.SetInt("keyShoot1", (int)keyShoot1);
                break;
            case "keyShoot2":
                keyShoot2 = newKey;
                PlayerPrefs.SetInt("keyShoot2", (int)keyShoot2);
                break;
            case "keyReload":
                keyReload = newKey;
                PlayerPrefs.SetInt("keyReload", (int)keyReload);
                break;
            case "keyPause":
                keyPause = newKey;
                PlayerPrefs.SetInt("keyPause", (int)keyPause);
                break;
            case "keyMap":
                keyMap = newKey;
                PlayerPrefs.SetInt("keyMap", (int)keyMap);
                break;
            case "keyHUD":
                keyHUD = newKey;
                PlayerPrefs.SetInt("keyHUD", (int)keyHUD);
                break;
            default:
                Debug.Log($"Invalid keybind '{keybind}' was attempted to be saved");
                break;
        }
    }
    public void SaveInputs()
    {
        PlayerPrefs.SetInt("keyUp", (int)keyUp);
        PlayerPrefs.SetInt("keyDown", (int)keyDown);
        PlayerPrefs.SetInt("keyLeft", (int)keyLeft);
        PlayerPrefs.SetInt("keyRight", (int)keyRight);
        PlayerPrefs.SetInt("keyRoll", (int)keyRoll);
        PlayerPrefs.SetInt("keyAbility", (int)keyAbility);
        PlayerPrefs.SetInt("keyPickup", (int)keyPickup);
        PlayerPrefs.SetInt("keyInteract", (int)keyInteract);
        PlayerPrefs.SetInt("keyShoot1", (int)keyShoot1);
        PlayerPrefs.SetInt("keyShoot2", (int)keyShoot2);
        PlayerPrefs.SetInt("keyReload", (int)keyReload);
        PlayerPrefs.SetInt("keyPause", (int)keyPause);
        PlayerPrefs.SetInt("keyMap", (int)keyMap);
        PlayerPrefs.SetInt("keyHUD", (int)keyHUD);
    }
    public void LoadInputs()
    {
        // If no prefs saved then set defaults
        if (PlayerPrefs.GetInt("keyUp") == 0) ResetInputs();
        
        keyUp = (KeyCode)PlayerPrefs.GetInt("keyUp");
        keyDown = (KeyCode)PlayerPrefs.GetInt("keyDown");
        keyLeft = (KeyCode)PlayerPrefs.GetInt("keyLeft");
        keyRight = (KeyCode)PlayerPrefs.GetInt("keyRight");
        keyRoll = (KeyCode)PlayerPrefs.GetInt("keyRoll");
        keyAbility = (KeyCode)PlayerPrefs.GetInt("keyAbility");
        keyPickup = (KeyCode)PlayerPrefs.GetInt("keyPickup");
        keyInteract = (KeyCode)PlayerPrefs.GetInt("keyInteract");
        keyShoot1 = (KeyCode)PlayerPrefs.GetInt("keyShoot1");
        keyShoot2 = (KeyCode)PlayerPrefs.GetInt("keyShoot2");
        keyReload = (KeyCode)PlayerPrefs.GetInt("keyReload");
        keyPause = (KeyCode)PlayerPrefs.GetInt("keyPause");
        keyMap = (KeyCode)PlayerPrefs.GetInt("keyMap");
        keyHUD = (KeyCode)PlayerPrefs.GetInt("keyHUD");
    }
    public Vector2 GetMovementVector2()
    {
        Vector2 vector = Vector2.zero;

        if (Input.GetKey(keyUp)) vector.y += 1;
        if (Input.GetKey(keyDown)) vector.y -= 1;
        if (Input.GetKey(keyLeft)) vector.x -= 1;
        if (Input.GetKey(keyRight)) vector.x += 1;

        return vector;
    }
    public Vector3 GetMovementVector3()
    {
        Vector3 vector = Vector3.zero;

        if (Input.GetKey(keyUp)) vector.z += 1;
        if (Input.GetKey(keyDown)) vector.z -= 1;
        if (Input.GetKey(keyLeft)) vector.x -= 1;
        if (Input.GetKey(keyRight)) vector.x += 1;

        return vector;
    }
}
