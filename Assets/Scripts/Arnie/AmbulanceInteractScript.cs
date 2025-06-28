using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmbulanceInteractScript : MonoBehaviour
{
    // Interaction varaibles
    private bool inZone = false;
    private bool interacted = false;
    public bool interactable = true;
    private bool pressed = false;
    private bool zoomIn = false;
    private bool weaponBought = false;
    public bool stealMode = false;
    public bool shopClosed = false;
    public bool zoomedInOnItem = false;

    // Camera variables 
    private Vector3 initialCamPos;
    private Vector3 initialCamRot;
    public GameObject cameraPos;
    public GameObject activeCameraPos;

    // Item varaibles and game objects
    public List<GameObject> shopItems;
    public List<GameObject> potentialShopItems;
    private int initialShopItemsCount;
    public GameObject itemParent;

    // Door variables
    private int speed = 3;
    public GameObject door;
    public GameObject doorEndPos;
    private Vector3 doorStartPos;

    // Interpolation variables
    private float timeElapsed = 0;
    public float lerpDuration = 1;
    public float initialLerpDuration = 0;
    public bool zoomedInLerp = false;
    public float initialZoomTimer = 0;
    private bool updateInitialZoomTimer = false;

    // Shopkeeper variables
    public GameObject ShopKeeper;
    public int clickAtempts = 0;
    public GameObject shopKeeperSpawnPos;
    public GameObject ExplosionPrefab;
    public GameObject VanPrefab;

    public bool isAmbulance = false;

    public void Start()
    {
        // Sets this shop instance to the shop view buttons refrence
        /*if(CanvasScript.instance.AmbulanceButton.parentShop != null)
        {
            Debug.Log("Hey you have already done this.");
            CanvasScript.instance.AmbulanceButton.parentShop = this;
        }*/
        CanvasScript.instance.AmbulanceButton.parentShop = this;


        gameObject.transform.eulerAngles = new Vector3(0, 180, 0);

        // Creates a random array of shop items and adds them to the scene 
        /*for (int i = 0; i < shopItems.Count; i++)
        {
            int rand = Random.Range(0, 1);
            GameObject instantiatedItem = Instantiate(potentialShopItems[0]);
            instantiatedItem.transform.position = shopItems[i].transform.position;
            instantiatedItem.GetComponent<AmbulanceItem>().parentShop = gameObject.GetComponent<AmbulanceInteractScript>();
            instantiatedItem.transform.eulerAngles = new Vector3(0, 0, 0);
            instantiatedItem.transform.SetParent(itemParent.transform, true);

            potentialShopItems.Remove(potentialShopItems[rand]);
            shopItems[i] = instantiatedItem;

            CanvasScript.instance.AmbulanceButton.viewObject = instantiatedItem.GetComponent<AmbulanceItem>();
        }*/
        int rand = Random.Range(0, 1);
        GameObject instantiatedItem = Instantiate(potentialShopItems[0]);
        instantiatedItem.transform.position = shopItems[0].transform.position;
        instantiatedItem.GetComponent<AmbulanceItem>().parentShop = gameObject.GetComponent<AmbulanceInteractScript>();
        instantiatedItem.transform.eulerAngles = new Vector3(0, 0, 0);
        instantiatedItem.transform.SetParent(itemParent.transform, true);

        potentialShopItems.Remove(potentialShopItems[rand]);
        shopItems[0] = instantiatedItem;

        CanvasScript.instance.AmbulanceButton.viewObject = instantiatedItem.GetComponent<AmbulanceItem>();


        initialCamRot = CameraController.instance.transform.eulerAngles;

        doorStartPos = door.transform.position;

        activeCameraPos = cameraPos;

        initialShopItemsCount = shopItems.Count;

        initialCamPos = CameraController.instance.transform.position;

        initialLerpDuration = lerpDuration;
    }

    public void Update()
    {
        // A timer to make sure the player doesnt zoom in on a weapon too fast
        if (updateInitialZoomTimer)
        {
            initialZoomTimer += Time.deltaTime;
        }

        // Removes NULL items from the active items list
        shopItems.RemoveAll(x => !x);

        // Checks if somethings been bought
        if (shopItems.Count != initialShopItemsCount && !stealMode)
        {
            ShutUpShop();
            weaponBought = true;
            activeCameraPos = cameraPos;

        }

        // Gets the shoper keeper out his van when stealing takes place
        if (shopItems.Count != initialShopItemsCount && stealMode)
        {
            zoomIn = false;
            weaponBought = true;

            GameObject explosion = Instantiate(ExplosionPrefab);
            explosion.transform.position = VanPrefab.transform.position;
            Destroy(VanPrefab);
            Destroy(itemParent);

            gameObject.GetComponentInParent<RoomController>().shopKeeperSpawned = true;

            stealMode = false;
            Destroy(gameObject);
        }


        // Checks if the shop is currently accessible 
        if (interactable)
        {
            // Only allows to get this far if a weapon is yet to be bought
            if (inZone && !weaponBought)
            {
                // Shows this on the UI so the player knows theyre in range of the shop
                CanvasScript.instance.InteractText.text = $"PRESS '{GameManager.GMinstance.keyInteract}' TO INTERACT";
                if (!interacted)
                {
                    // Pressed bool exists just so two actions dont get ran on the same frame 
                    if (!pressed)
                    {
                        // when interacted, shop is zoomed in on and all the nessasary values are set so that the shop is in essentially shop mode
                        if (GameManager.GMinstance.GetInputDown("keyInteract") && GameManager.GMinstance.gamePaused == false)
                        {
                            CanvasScript.instance.ShopKeeperText.text = "Welcome back, please consider buying some health, its dangerous out there";
                            CanvasScript.instance.stealButton.gameObject.SetActive(false);

                            updateInitialZoomTimer = true;
                            zoomIn = true;
                            CameraController.instance.transform.eulerAngles = new Vector3(CameraController.instance.transform.eulerAngles.x, cameraPos.transform.eulerAngles.y, CameraController.instance.transform.eulerAngles.z);
                            initialCamPos = CameraController.instance.transform.position;
                            initialCamRot = CameraController.instance.transform.eulerAngles;
                            CameraController.instance.GetComponent<Camera>().orthographic = false;
                            CameraController.instance.GetComponent<Camera>().nearClipPlane = 0.01f;
                            CameraController.instance.GetComponent<CameraController>().SetCameraLocked(true);
                            interacted = true;
                            pressed = true;
                            CanvasScript.instance.SetCanvasMode(CanvasScript.UIMode.SHOPMODE);

                            foreach (GameObject shopItem in shopItems)
                            {
                                shopItem.GetComponent<AmbulanceItem>().interactable = true;
                            }

                            PlayerController.instance.FreezeGameplayInput(true);
                        }
                    }

                }

                // Exits the shop and resets variables to initial states
                if (interacted)
                {
                    if (!pressed)
                    {

                        if (GameManager.GMinstance.GetInputDown("keyInteract") && GameManager.GMinstance.gamePaused == false)
                        {
                            ExitShop();
                        }
                    }
                }
            }
            else
            {
                //CanvasScript.instance.InteractText.text = "";
            }
        }

        // Resets the interpolation (bool is changed in the shopItem script)
        if (zoomedInLerp)
        {
            timeElapsed = 0;
            zoomedInLerp = false;
        }


        // Interpolation makes the camera zoom in  on the desired object
        if (zoomIn)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed < lerpDuration)
            {
                CameraController.instance.transform.position = Vector3.Lerp(CameraController.instance.transform.position, activeCameraPos.transform.position, timeElapsed / lerpDuration);
                CameraController.instance.transform.eulerAngles = Vector3.Lerp(CameraController.instance.transform.eulerAngles, activeCameraPos.transform.eulerAngles, timeElapsed / lerpDuration);
            }
            else
            {
                // Once the lerp is nearly at the end it sets the position to the target position so that theres no short movement
                CameraController.instance.transform.position = activeCameraPos.transform.position;
                CameraController.instance.transform.eulerAngles = activeCameraPos.transform.eulerAngles;
            }
            if (timeElapsed > 1.0f)
            {
                CanvasScript.instance.AmbulanceButton.gameObject.SetActive(true);
                CanvasScript.instance.ambulanceButtonBackground.gameObject.SetActive(true);
            }
        }
        else
        {
            CanvasScript.instance.AmbulanceButton.gameObject.SetActive(false);
            CanvasScript.instance.ambulanceButtonBackground.gameObject.SetActive(false);
            timeElapsed = 0;
        }


        // Closes the shop once a player has bought a weapon 
        if (weaponBought)
        {
            ShutUpShop();
            if (!shopClosed)
            {
                PlayerController.instance.FreezeGameplayInput(false);
                shopClosed = true;
            }
            zoomIn = false;
        }

        // Checks if the doors of the shop need to be closed and not accessible to player
        if (shopClosed == false)
        {
            if (door != null)
            {
                door.transform.localScale = new Vector3(door.transform.localScale.x, 1.5f, door.transform.localScale.z);
                door.transform.position = Vector3.Lerp(door.transform.position, doorStartPos, speed * Time.deltaTime);
                interactable = !shopClosed;
            }

        }
        else
        {
            if (door != null)
            {
                door.transform.localScale = new Vector3(door.transform.localScale.x, 4f, door.transform.localScale.z);
                door.transform.position = Vector3.Lerp(door.transform.position, doorEndPos.transform.position, speed * Time.deltaTime);
                interactable = !shopClosed;
            }
        }

        pressed = false;
    }

    // Checks if the player is in range to interact with the shop
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            inZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            inZone = false;
            CanvasScript.instance.InteractText.text = "";
        }

    }

    // Function that hard closes the shop so that it cant be accessed again
    void ShutUpShop()
    {
        CanvasScript.instance.stealButton.gameObject.SetActive(true);
        CanvasScript.instance.stealButton.stealMode = false;
        CanvasScript.instance.stealButton.GetComponent<Image>().color = Color.white;
        stealMode = false;


        foreach (GameObject shopItem in shopItems)
        {
            Destroy(shopItem);
            zoomIn = false;

            CameraController.instance.Start();

            CameraController.instance.GetComponent<CameraController>().SetCameraLocked(false);
            CameraController.instance.GetComponent<Camera>().nearClipPlane = -70;
            CameraController.instance.GetComponent<Camera>().orthographic = true;

            CanvasScript.instance.SetCanvasMode(CanvasScript.UIMode.PLAYMODE);
        }

        if (door != null)
        {
            door.transform.localScale = new Vector3(door.transform.localScale.x, 4, door.transform.localScale.z);
            door.transform.position = Vector3.Lerp(door.transform.position, doorEndPos.transform.position, speed * Time.deltaTime);
        }



        weaponBought = false;
        zoomIn = false;

        CanvasScript.instance.ShopKeeperText.color = Color.white;


    }


    // Function that lets the player exit the shop without closing it
    public void ExitShop()
    {
        zoomIn = false;
        updateInitialZoomTimer = false;
        initialZoomTimer = 0;

        CameraController.instance.Start();

        CameraController.instance.GetComponent<Camera>().orthographic = true;
        CameraController.instance.GetComponent<Camera>().nearClipPlane = -70;
        CameraController.instance.GetComponent<CameraController>().SetCameraLocked(false);
        interacted = false;
        pressed = true;


        CanvasScript.instance.stealButton.gameObject.SetActive(true);
        CanvasScript.instance.stealButton.stealMode = false;
        CanvasScript.instance.stealButton.GetComponent<Image>().color = Color.white;
        stealMode = false;


        CanvasScript.instance.SetCanvasMode(CanvasScript.UIMode.PLAYMODE);

        foreach (GameObject shopItem in shopItems)
        {
            shopItem.GetComponent<AmbulanceItem>().interactable = false;
        }

        PlayerController.instance.FreezeGameplayInput(false);

        activeCameraPos = cameraPos;



        CanvasScript.instance.ShopKeeperText.color = Color.white;
        clickAtempts = 0;





    }
}
