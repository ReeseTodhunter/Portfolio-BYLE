using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private GameController gameController;
    private GridManager gridManager;
    private GameManager gameManager;
    private GameObject instantiatedObject = null;

    public enum StructureMode { Hall, Room, Hub, Boss, Branch, Shop , Ambulance, Totem, Shopkeeperboss};
    public StructureMode structureGenerated;

    public enum DirectionMode { Left, Right, Up, Down, None }
    public DirectionMode direction;

    public Transform newRoomCentre;

    public bool ableToGenerate = false;

    public GameObject doorCanvas;

    public RoomController parentRoom;

    public int levelCounter;

    public System.Random random;

    // Start is called before the first frame update
    public void Start()
    {
        // Gets reference to the gameObject
        gameController = GameController.instance;
        gridManager = GridManager.instance;
        gameManager = GameManager.GMinstance;

        SpawnCheck();

        parentRoom = gameObject.GetComponentInParent<RoomController>();
    }

    // Checks if there is a colliding tile and if not spawns the specified structure
    public void SpawnCheck()
    {
        levelCounter = gameController.currentLevel - 1;

        random = GameManager.GMinstance.rand;
        
        

        if (structureGenerated == StructureMode.Hub)
        {
            gameController.RoomCounter = gameController.initialRoomCounter;
        }


        // Checks to see if there is still rooms that need to be generated
        if (gameController.RoomCounter == 1)
        {
            if (GameManager.GMinstance.level == 10)
            {
                structureGenerated = StructureMode.Shopkeeperboss;
            }
            else
            {
                structureGenerated = StructureMode.Boss;
            }
            
        }

        // Creates a branch room early on so theres multiple places the player can go 
        if (gameController.RoomCounter == gameController.initialRoomCounter - 4)
        {
            structureGenerated = StructureMode.Branch;
        }

        if (gameController.RoomCounter == gameController.initialRoomCounter - 5)
        {
            structureGenerated = StructureMode.Shop;
        }
        if (gameController.RoomCounter == gameController.initialRoomCounter - 6)
        {
            structureGenerated = StructureMode.Ambulance;
        }
        if (gameController.RoomCounter == gameController.initialRoomCounter - 8)
        {
            structureGenerated = StructureMode.Totem;
        }


        // Checks when all rooms have been generated
        if (gameController.RoomCounter <= 0)
        {
            if (gameController.RoomCounterMet == false)
            {
                gameController.RoomCounterMet = true;
            }
        }

        // Creates the structure based off of the directions
        if (structureGenerated == StructureMode.Room)
        {
            if (direction == DirectionMode.Left)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].Rooms, rot);
            }
            else if (direction == DirectionMode.Right)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].Rooms, rot);
            }
            else if (direction == DirectionMode.Up)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].Rooms, rot);
            }
            else if (direction == DirectionMode.Down)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].Rooms, rot);
            }
        }
        else if (structureGenerated == StructureMode.Hub)
        {
            ableToGenerate = true;
            Vector3 rot = new Vector3(0.0f, 0.0f, 0.0f);
            CreateStructure(gameController.Levels[levelCounter].HubRooms, rot);
        }
        else if (structureGenerated == StructureMode.Boss)
        {
            if (direction == DirectionMode.Left)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].BossRooms, rot);
            }
            else if (direction == DirectionMode.Right)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].BossRooms, rot);
            }
            else if (direction == DirectionMode.Up)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].BossRooms, rot);
            }
            else if (direction == DirectionMode.Down)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].BossRooms, rot);
            }
        }
        else if (structureGenerated == StructureMode.Shopkeeperboss)
        {
            if (direction == DirectionMode.Left)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].ShopKeeperFight, rot);
            }
            else if (direction == DirectionMode.Right)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].ShopKeeperFight, rot);
            }
            else if (direction == DirectionMode.Up)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].ShopKeeperFight, rot);
            }
            else if (direction == DirectionMode.Down)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].ShopKeeperFight, rot);
            }
        }
        else if (structureGenerated == StructureMode.Branch)
        {
            if (direction == DirectionMode.Left)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].BranchRooms, rot);
            }
            else if (direction == DirectionMode.Right)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].BranchRooms, rot);
            }
            else if (direction == DirectionMode.Up)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].BranchRooms, rot);
            }
            else if (direction == DirectionMode.Down)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].BranchRooms, rot);
            }
        }
        else if (structureGenerated == StructureMode.Shop)
        {
            if (direction == DirectionMode.Left)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].ShopRooms, rot);
            }
            else if (direction == DirectionMode.Right)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].ShopRooms, rot);
            }
            else if (direction == DirectionMode.Up)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].ShopRooms, rot);
            }
            else if (direction == DirectionMode.Down)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].ShopRooms, rot);
            }
        }
        else if (structureGenerated == StructureMode.Ambulance)
        {
            if (direction == DirectionMode.Left)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].AmbulanceRooms, rot);
            }
            else if (direction == DirectionMode.Right)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].AmbulanceRooms, rot);
            }
            else if (direction == DirectionMode.Up)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].AmbulanceRooms, rot);
            }
            else if (direction == DirectionMode.Down)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].AmbulanceRooms, rot);
            }
        }
        else if (structureGenerated == StructureMode.Totem)
        {
            if (direction == DirectionMode.Left)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].TotemRooms, rot);
            }
            else if (direction == DirectionMode.Right)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].TotemRooms, rot);
            }
            else if (direction == DirectionMode.Up)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].TotemRooms, rot);
            }
            else if (direction == DirectionMode.Down)
            {
                Vector3 rot = gameObject.transform.rotation.eulerAngles;
                CreateStructure(gameController.Levels[levelCounter].TotemRooms, rot);
            }
        }

        // Decrements the room size counter
        gameController.RoomCounter -= 1;
    }

    // Structure creation function
    void CreateStructure(GameObject[] structure, Vector3 rotation)
    {
        if (ableToGenerate)
        {
            if (gameController.RoomCounter > 0)
            {
                if (gridManager.Grid[((Mathf.RoundToInt(newRoomCentre.transform.position.x)) / (gameController.standardRoomSize / 2)) + gridManager.offset, ((Mathf.RoundToInt(newRoomCentre.transform.position.z)) / (gameController.standardRoomSize / 2)) + gridManager.offset] == 0)
                {
                    
                    // Instnatiates a random structure from the passed in array and gives it the rotation nessasary for the room
                    int rndNo = random.Next(0, structure.Length);

                    if (parentRoom != null)
                    {
                        if (structure[rndNo].GetComponent<RoomController>().roomID == parentRoom.roomID)
                        {
                            CreateStructure(structure, rotation);
                        }
                        else
                        {
                            if(structure[rndNo].GetComponent<RoomController>().specialRoom == true)
                            {
                                if(gameController.specialRoomSpawned == true || gameController.RoomCounter > gameController.initialRoomCounter / 2 )
                                {
                                    CreateStructure(structure, rotation);
                                }
                                else
                                {
                                    instantiatedObject = Instantiate(structure[rndNo], transform.position, Quaternion.identity);
                                }
                            }
                            else
                            {
                                instantiatedObject = Instantiate(structure[rndNo], transform.position, Quaternion.identity);
                            }
                        }
                    }
                    else
                    {
                        // Means its the hubroom
                        instantiatedObject = Instantiate(structure[rndNo], transform.position, Quaternion.identity);
                    }

                    instantiatedObject.transform.eulerAngles = rotation;

                    gameController.previousRoomID = instantiatedObject.GetComponent<RoomController>().roomID;

                    if(instantiatedObject.GetComponent<RoomController>().specialRoom == true)
                    {
                        gameController.specialRoomSpawned = true;
                        
                    }

                    if (rotation == new Vector3(0, 90, 0) || rotation == new Vector3(0, 270, 0))
                    {
                        int tempX = instantiatedObject.GetComponent<RoomController>().roomX;
                        instantiatedObject.GetComponent<RoomController>().roomX = instantiatedObject.GetComponent<RoomController>().roomY;
                        instantiatedObject.GetComponent<RoomController>().roomY = tempX;
                    }

                    instantiatedObject.transform.SetParent(gameController.roomsParent.transform, true);

                    // Instantiates a door between the current room and newly generated room
                    if (structure != gameController.Levels[levelCounter].HubRooms)
                    {
                        if(structureGenerated == StructureMode.Boss)
                        {
                            GameObject door = Instantiate(gameController.Levels[levelCounter].DoorWalls[1], transform.position, Quaternion.identity);
                            door.transform.parent = instantiatedObject.transform;
                            door.transform.eulerAngles = rotation;

                            doorCanvas.SetActive(true);

                            gameController.ActiveDoors.Add(door);
                        }
                        else
                        {
                            GameObject door = Instantiate(gameController.Levels[levelCounter].DoorWalls[0], transform.position, Quaternion.identity);
                            door.transform.parent = instantiatedObject.transform;
                            door.transform.eulerAngles = rotation;

                            doorCanvas.SetActive(true);

                            gameController.ActiveDoors.Add(door);
                        }
                        
                    }


                }
                else
                {
                    if (structureGenerated == StructureMode.Boss)
                    {
                        RoomGenerator[] rooms = FindObjectsOfType<RoomGenerator>();
                        foreach (RoomGenerator roomGenerator in rooms)
                        {
                            if(roomGenerator.structureGenerated != StructureMode.Hub)
                            {
                                Destroy(roomGenerator);
                            }
                            
                        }


                        gameController.RoomCounter = 0;
                        Debug.Log(gameController.RoomCounter);
                        gameController.GenerateMap();

                        
                    }
                }
            }
            if (gameController.RoomCounter <= 0)
            {
                GameObject w = Instantiate(gameController.Levels[levelCounter].Walls[0], new Vector3(transform.position.x,transform.position.y + 1, transform.position.z), Quaternion.identity);
                w.transform.eulerAngles = rotation;

                w.transform.SetParent(parentRoom.transform, true);

                gameController.RoomCounter += 1;
            }
        }
    }
}