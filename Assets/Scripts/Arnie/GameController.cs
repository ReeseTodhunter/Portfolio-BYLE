using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Singleton instance
    public static GameController instance;

    public Levels[] Levels;

    public int currentLevel = 1;

    public List<GameObject> ActiveDoors = new List<GameObject>();
    public List<ShopInteractScript> ActiveShops = new List<ShopInteractScript>();

    public bool RoomCounterMet = false;
    public int standardRoomSize = 50;

    public int previousRoomID = 999;

    public GameObject activeShop;

    // A integer for how many rooms are left to spawn in the scene
    public int RoomCounter;
    public int initialRoomCounter;

    public bool bossRoomGenerated = true;

    public GameObject roomsParent;
    public GameObject weaponParent;

    public RoomGenerator initialRoomGenerator;

    public bool generate = false;

    public bool specialRoomSpawned = false;

    public List<GameObject> blockers = new List<GameObject>();
    public void Awake()
    {
        // Initialise Singleton
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;

        initialRoomCounter = RoomCounter;
    }

    public void Update()
    {
        // Removes NULL items from the active items list
        blockers.RemoveAll(x => !x);

        if (bossRoomGenerated == false)
        {
            RoomGenerator[] rooms = FindObjectsOfType<RoomGenerator>();
            foreach (RoomGenerator roomGenerator in rooms)
            {
                if (roomGenerator.structureGenerated != RoomGenerator.StructureMode.Hub)
                {
                    Destroy(roomGenerator);
                }

            }
            RoomCounter = 0;
            MapGeneration();
        }


        // Resets blockers if player somehow gets on top of them
        if(PlayerController.instance.transform.position.y > 3)
        {
            foreach (GameObject blocker in blockers)
            {
                Debug.Log("Player on top of blocker");
                blocker.SetActive(false);
            }
        }

    }

    public void DoorManagement(bool isRoomLocked)
    {
        // Activates EQS blockers for rooms
        if (isRoomLocked == true)
        {
            foreach (GameObject blocker in blockers)
            {
                blocker.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject blocker in blockers)
            {
                blocker.SetActive(false);
            }
        }

        foreach(GameObject door in ActiveDoors)
        {
            if(door != null)
            {
                door.GetComponent<DoorLogic>().DoorManagement(isRoomLocked);
            }
        }
        foreach (ShopInteractScript shop in ActiveShops)
        {
            if (shop != null)
            {
                shop.CloseShop(isRoomLocked);
            }
        }
    }

    public void GenerateMap()
    {
        GameManager.GMinstance.SetRandomSeed();
        GenerateMapWithSeed();
    }

    public void GenerateMapWithSeed()
    {
        GameManager.GMinstance.SetSeededRandom(GameManager.GMinstance.levelSeed);
        MapGeneration();
    }

    public void MapGeneration()
    {
        ActiveDoors.Clear();
        ActiveShops.Clear();
        CameraController.instance.Start();
        specialRoomSpawned = false;

        foreach (Transform child in roomsParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Removes NULL items from the active items list
        blockers.RemoveAll(x => !x);

        specialRoomSpawned = false;
        RoomCounterMet = false;
        GridManager.instance.ResetGrid();

        initialRoomGenerator.SpawnCheck();
        bossRoomGenerated = true;
    }

    public void ActivateBlockers(bool isActivated)
    {
        // Activates EQS blockers for rooms
        if (isActivated == true)
        {
            foreach (GameObject blocker in blockers)
            {
                blocker.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject blocker in blockers)
            {
                blocker.SetActive(false);
            }
        }
    }
}

[System.Serializable]
public class Levels
{
    public string profileName = null;

    public GameObject[] Rooms;
    public GameObject[] HubRooms;
    public GameObject[] BossRooms;
    public GameObject[] BranchRooms;
    public GameObject[] ShopRooms;
    public GameObject[] AmbulanceRooms;
    public GameObject[] TotemRooms;
    public GameObject[] Walls;
    public GameObject[] DoorWalls;
    public GameObject[] ShopKeeperFight;
}

