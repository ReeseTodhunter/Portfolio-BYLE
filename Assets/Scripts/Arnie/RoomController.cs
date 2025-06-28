using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//remove this v
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviour
{
    // Arrays holding all the nessasary components for room collision detection
    public GameObject[] Centres;
    public RoomGenerator[] roomGenerators;

    public GameObject[] EQSPositions;
    public GameObject EQSFinalPos;

    public int roomID;

    public GameObject mapCanvas;

    //Enemy stuff
    public int budget = 6;
    public int rounds = 1;
    // Wall prefab to close off rooms
    public GameObject wall;

    // Room dimesnions for calculations
    public int roomX;
    public int roomY;

    // Enemy variables
    private bool enemiesAlive = false;

    // Chest prefab
    public GameObject Plane;

    public bool shopKeeperSpawned = false;

    public GameObject[] blockers;

    public bool shopRoom = false;

    //Unique rooms
    public bool cutscene;
    public RoomEvent roomEvent;

    public float timer = 0;

    // Booleon that checks whether room can be spawned 
    bool roomCanBeSpawned = true;
    public enum RoomType
    {
        COMBAT,
        UNIQUE_ROOM,
        SHOP,
        TOTEM,
        BOSS
    }
    public RoomType roomType;
    public EnemySpawner.UnqiueEnemies enemyType;
    public GameObject shopKeeperPrefab;
    public Transform UniqueEnemySpawnPos;

    // Reference to the grid manager
    public GridManager gridManager;

    public GameController gameController;

    public bool totemActive = false;

    public EnemySpawningSystem.EnemyWaveType EnemyWaveType;
    public EnemySpawningSystem.EnemyWaveSize EnemyWaveSize;

    public bool specialRoom = false;
    void Start()
    {
        foreach (GameObject blocker in blockers)
        {
            blocker.transform.localScale = new Vector3(blocker.transform.localScale.x, blocker.transform.localScale.y + 50, blocker.transform.localScale.z);
        }

        // Assigns refrence to the grid manager
        gridManager = GridManager.instance;

        // Assigns refrence to the game controller
        gameController = GameController.instance;

        // Initially sets all the room generaters to not be able to generate so that a check can be done
        for (int j = 0; j < roomGenerators.Length; j++)
        {
            roomGenerators[j].ableToGenerate = false;
        }

        // Loops through the tile size of each room (defined by the room centres) 
        for (int i = 0; i < Centres.Length; i++)
        {
            // Checks if the centre would spawn into a currently occupied tile
            if (gridManager.Grid[((Mathf.RoundToInt(Centres[i].transform.position.x)) / (gameController.standardRoomSize / 2)) + gridManager.offset, ((Mathf.RoundToInt(Centres[i].transform.position.z)) / (gameController.standardRoomSize / 2)) + gridManager.offset] == 1)
            {
                // Sets variable to false to stop rooms from spawning
                roomCanBeSpawned = false;

                // Make a wall instantiate to block off the dead ends
                for (int j = 0; j < roomGenerators.Length; j++)
                {
                    roomGenerators[j].ableToGenerate = false;
                    GameObject w = Instantiate(wall, transform.position, Quaternion.identity);
                    w.transform.position = new Vector3(w.transform.position.x, 1.5f, w.transform.position.z);
                    w.transform.eulerAngles = gameObject.transform.rotation.eulerAngles;
                    w.transform.parent = gameObject.transform;
                }

                if (roomType == RoomType.BOSS)
                {
                    gameController.bossRoomGenerated = false;
                }
                gameController.bossRoomGenerated = false;

                Debug.Log("AHHHHHHHHHHHHH WE ARE REGENEERATING");
                // Destroys the room as it has failed the check
                Destroy(this.gameObject);
            }
        }

        // Only excutes code if the room has passed the checks
        if (roomCanBeSpawned)
        {
            // Sets all the tiles this room occupies to full and allows the room generators attached to this room to generate rooms themselves
            for (int i = 0; i < Centres.Length; i++)
            {
                gridManager.Grid[((Mathf.RoundToInt(Centres[i].transform.position.x)) / (gameController.standardRoomSize / 2)) + gridManager.offset, ((Mathf.RoundToInt(Centres[i].transform.position.z)) / (gameController.standardRoomSize / 2)) + gridManager.offset] = 1;
                for (int j = 0; j < roomGenerators.Length; j++)
                {
                    roomGenerators[j].ableToGenerate = true;
                }
            }
        }


        // Checks which EQS pos has the correct rotatiom to set the EQS from
        for (int i = 0; i < EQSPositions.Length; i++)
        {
            if (EQSPositions[i].transform.localEulerAngles == gameObject.transform.eulerAngles)
            {
                EQSFinalPos = EQSPositions[i];
            }
        }


        foreach (GameObject blocker in blockers)
        {
            GameController.instance.blockers.Add(blocker);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerController.instance.playerHitInRoom = false;
        // Checks which EQS pos has the correct rotatiom to set the EQS from
        for (int i = 0; i < EQSPositions.Length; i++)
        {
            if (EQSPositions[i].transform.localEulerAngles == gameObject.transform.eulerAngles)
            {
                EQSFinalPos = EQSPositions[i];
            }
        }

        if (EQSFinalPos == null)
        {
            EQSFinalPos = EQSPositions[0];
        }

        if (other.GetComponent<PlayerController>() != null)
        {
            GameController.instance.ActivateBlockers(true);
            foreach (GameObject blocker in blockers)
            {
                blocker.SetActive(false);
            }

            EQS.instance.Initialise(EQSFinalPos.transform.position, new Vector2Int(roomX * 50, roomY * 50));
            GameManager.GMinstance.roomsEntered += 1;
            switch (roomType)
            {
                case RoomType.COMBAT:
                    gameController.DoorManagement(true);

                    GetComponent<Collider>().enabled = false;
                    foreach (GameObject blocker in blockers)
                    {
                        blocker.SetActive(false);
                    }

                    EnemySpawningSystem.instance.ActivateSpawner(rounds, EnemyWaveSize, EnemyWaveType);
                    enemiesAlive = true;
                    break;

                case RoomType.UNIQUE_ROOM:
                    GameManager.GMinstance.currentState = GameManager.gameState.cutscene;
                    roomEvent.roomActive = true;
                    gameController.DoorManagement(true);

                    foreach (GameObject blocker in blockers)
                    {
                        blocker.SetActive(false);
                    }

                    GetComponent<Collider>().enabled = false;
                    
                    CameraController.instance.SetCameraLocked(true);
                    roomEvent.playerCameraPos = CameraController.instance.transform.position;
                    enemiesAlive = true;

                    break;

                case RoomType.SHOP:
                    GameController.instance.ActivateBlockers(false);
                    break;
                case RoomType.TOTEM:
                    GameController.instance.ActivateBlockers(false);
                    break;
                case RoomType.BOSS:
                    GameManager.GMinstance.currentState = GameManager.gameState.cutscene;
                    gameController.DoorManagement(true);

                    foreach (GameObject blocker in blockers)
                    {
                        blocker.SetActive(false);
                    }

                    GetComponent<Collider>().enabled = false;

                    if (this.TryGetComponent<BossSpawner>(out BossSpawner spawner))
                    {
                        Debug.Log("SpawningBoss");
                        spawner.SpawnBoss();
                    }


                    CameraController.instance.SetCameraLocked(true);
                    roomEvent.roomActive = true;
                    roomEvent.playerCameraPos = CameraController.instance.transform.position;
                    enemiesAlive = true;
                   
                    break;
            }

            
            mapCanvas.SetActive(true);
        }
        
    }

    public void Update()
    {
        if(enemiesAlive == true)
        {
            timer += Time.deltaTime;
            if (timer > 4.5f)
            {
                if (EnemySpawningSystem.instance.isRoomCleared() == true && EnemySpawningSystem.instance.AllEnemiesSpawned() == true && roomType != RoomType.SHOP && roomType != RoomType.TOTEM) 
                {

                    PlayerController.instance.AddModifier(ModifierType.MaxHealth, 0);
                    if (roomType != RoomType.BOSS)
                    {
                        CanvasScript.instance.roomClear.clearText.text = "Room Cleared";
                        CanvasScript.instance.roomClear.roomClear = false;
                        CanvasScript.instance.roomClear.Start();

                        if (PlayerController.instance.playerHitInRoom == false)
                        {
                            AchievementSystem.UnlockAchievement(16);
                            AchievementSystem.Init();
                            SaveLoadSystem.instance.SaveAchievements();
                            GameManager.GMinstance.UnlockSteamAchievement("Untouchable");
                        }

                        int rand = Random.Range(1, 3);
                        if (rand == 1)
                        {
                            if (!shopRoom)
                            {
                                GameObject chestInstance = Instantiate(Plane);
                                chestInstance.transform.position = new Vector3(PlayerController.instance.transform.position.x + 25, PlayerController.instance.transform.position.y + 60, PlayerController.instance.transform.position.z - 40);
                                chestInstance.transform.parent = gameObject.transform;
                            }

                        }

                        /*if (blocker != null)
                        {
                            blocker.SetActive(false);
                        }*/
                    }
                    gameController.DoorManagement(false);
                    PlayerController.instance.score += 100;
                    enemiesAlive = false;
                }

                if (EnemySpawningSystem.instance.isRoomCleared() == true &&  roomType != RoomType.SHOP && roomType != RoomType.TOTEM && roomType != RoomType.BOSS && shopRoom )
                {
                    CanvasScript.instance.roomClear.clearText.text = "Room Cleared";
                    CanvasScript.instance.roomClear.roomClear = false;
                    CanvasScript.instance.roomClear.Start();

                    gameController.DoorManagement(false);
                    PlayerController.instance.score += 250;
                    enemiesAlive = false;
                }

                if (EnemySpawningSystem.instance.isRoomCleared() == true && roomType != RoomType.SHOP && roomType != RoomType.TOTEM && roomType == RoomType.BOSS && shopRoom)
                {
                    CanvasScript.instance.roomClear.clearText.text = "Boss Room Cleared";
                    CanvasScript.instance.roomClear.roomClear = false;
                    CanvasScript.instance.roomClear.Start();

                    gameController.DoorManagement(false);
                    PlayerController.instance.score += 1000;
                    enemiesAlive = false;
                }
            }

        }


        if (shopKeeperSpawned)
        {
            EQS.instance.Initialise(EQSFinalPos.transform.position, new Vector2Int(roomX * 50, roomY * 50));
            roomType = RoomType.COMBAT;
            gameController.DoorManagement(true);

            foreach (GameObject blocker in blockers)
            {
                blocker.SetActive(false);
            }

            EnemySpawningSystem.instance.DefaultSpawnerVariables();
            GameObject shopKeeper = Instantiate(shopKeeperPrefab);
            shopKeeper.transform.position = UniqueEnemySpawnPos.transform.position;
            EnemySpawningSystem.instance.SubscribeEnemy(shopKeeper.GetComponent<BTAgent>());
            timer = 0;

            GetComponent<Collider>().enabled = false;
            enemiesAlive = true;
            shopKeeperSpawned = false;
        }

        if (totemActive)
        {
            EQS.instance.Initialise(EQSFinalPos.transform.position, new Vector2Int(roomX * 50, roomY * 50));
            roomType = RoomType.COMBAT;
            gameController.DoorManagement(true);

            foreach (GameObject blocker in blockers)
            {
                blocker.SetActive(false);
            }

            GetComponent<Collider>().enabled = false;
            enemiesAlive = true;
            totemActive = false;
        }
    }
}

