using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MysteryBox : MonoBehaviour
{
    public List<GameObject> boxItems;

    public GameObject itemPosition;

    private int initialBoxItemsCount;

    private GameObject instantiatedItem;

    public List<GameObject> boxAnimationItems;
    public List<GameObject> potentialBoxItems;

    public List<GameObject> instantiatedBoxItems;

    public float timer = 0;
    public bool timerActive = false;

    public bool inZone;

    public GameObject Lid;

    public bool openBox = false;

    public static bool pressedBackButton = false;
    public static bool pressedBuyButton = false;

    public GameObject cameraPos;

    private bool zoomIn = false;
    private bool interacted = false;
    private bool pressed = false;

    private Vector3 initialCamPos;
    private Vector3 initialCamRot;

    private float timeElapsed = 0;
    public float lerpDuration = 1;

    private int animationNumber = 0;

    public bool itemSpawned = false;
    private bool inAnimation = false;

    public int boxPrice;
    private bool leaveBox = false;

    public GameObject lidEndRot;

    private BoxbackButton backButton;
    private BoxBuyButton buyButton;

    private float leaveBoxTimer = 0;
    private bool finalTextBool = false;

    public GameObject backgrounds;

    // Start is called before the first frame update
    void Start()
    {
        initialBoxItemsCount = boxItems.Count;
        
        pressedBackButton = false;
        pressedBuyButton = false;

        backButton = CanvasScript.instance.boxBackButton;

        buyButton = CanvasScript.instance.boxBuyButton;

        backgrounds = CanvasScript.instance.boxBackgrounds;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            timer += Time.deltaTime;
        }

        // Removes NULL items from the active items list
        instantiatedBoxItems.RemoveAll(x => !x);

        

        if (!itemSpawned)
        {
            if (inZone)
            {
                CanvasScript.instance.BoxInteractText.text = $"PRESS '{GameManager.GMinstance.keyInteract}' TO INTERACT";

                if (!interacted && !pressed && !leaveBox)
                {
                    if (GameManager.GMinstance.GetInput("keyInteract") && GameManager.GMinstance.gamePaused == false)
                    {
                        if(boxPrice == 0)
                        {
                            CanvasScript.instance.BoxDisplayText.text = "HERE IS A FREE WEAPON FOR YOUR TRAVELS";
                        }
                        else
                        {
                            CanvasScript.instance.BoxDisplayText.text = "MYSTERY BOX ITEM FOR $" + boxPrice;
                        }
                        
                        zoomIn = true;
                        CameraController.instance.transform.eulerAngles = new Vector3(CameraController.instance.transform.eulerAngles.x, cameraPos.transform.eulerAngles.y, CameraController.instance.transform.eulerAngles.z);
                        initialCamPos = CameraController.instance.transform.position;
                        initialCamRot = CameraController.instance.transform.eulerAngles;
                        CameraController.instance.GetComponent<Camera>().orthographic = false;
                        CameraController.instance.GetComponent<Camera>().nearClipPlane = 0.01f;
                        CameraController.instance.GetComponent<CameraController>().SetCameraLocked(true);
                        interacted = true;
                        pressed = true;
                        CanvasScript.instance.SetCanvasMode(CanvasScript.UIMode.BOXMODE);

                        PlayerController.instance.FreezeGameplayInput(true);
                    }
                }
                if (interacted && !pressed && !leaveBox) 
                {
                    if (pressedBackButton)
                    {
                        if (!inAnimation)
                        {
                            if (PlayerController.instance.GetCoinValue() >= boxPrice)
                            {
                                backButton.gameObject.SetActive(false);
                                buyButton.gameObject.SetActive(false);
                                backgrounds.SetActive(false);
                                CanvasScript.instance.BoxDisplayText.text = "You're missing out, or are you? it's a mystery!";
                                leaveBox = true;
                                pressed = true;

                                ////////////////
                                finalTextBool = true;
                                leaveBoxTimer = 0;

                            }
                            else if (PlayerController.instance.GetCoinValue() < boxPrice)
                            {
                                backButton.gameObject.SetActive(false);
                                buyButton.gameObject.SetActive(false);
                                backgrounds.SetActive(false);
                                CanvasScript.instance.BoxDisplayText.text = "You dont have the funds anyway.";
                                leaveBox = true;
                                pressed = true;

                                ////////////////
                                finalTextBool = true;
                                leaveBoxTimer = 0;
                            }
                            

                        }
                        
                    }
                    else if (pressedBuyButton)
                    {
                        if (!inAnimation)
                        {
                            
                            if (PlayerController.instance.GetCoinValue() < boxPrice)
                            {
                                buyButton.gameObject.SetActive(false);
                                backButton.gameObject.SetActive(false);
                                backgrounds.SetActive(false);
                                CanvasScript.instance.BoxDisplayText.text = "You are short on funds, bring more money if you want to try your luck.";
                                leaveBox = true;
                                pressed = true;

                                /////////////////
                                finalTextBool = true;
                                leaveBoxTimer = 0;
                            }
                            else if (PlayerController.instance.GetCoinValue() >= boxPrice)
                            {
                                CanvasScript.instance.BoxDisplayText.text = "Here comes a mystery!";
                                buyButton.gameObject.SetActive(false);
                                backButton.gameObject.SetActive(false);
                                backgrounds.SetActive(false);
                                inAnimation = true;
                                timerActive = true;
                                openBox = true;
                                pressed = true;
                                leaveBox = true;
                                PlayerController.instance.ChangeCoinValue(-boxPrice);

                                AchievementSystem.UnlockAchievement(10);
                                AchievementSystem.Init();
                                SaveLoadSystem.instance.SaveAchievements();
                                GameManager.GMinstance.UnlockSteamAchievement("Mysterious_Box");

                                ////////////////
                                finalTextBool = true;
                                leaveBoxTimer = 0;
                            }
                        }
                        
                    }
                }
                if (interacted && !pressed && leaveBox)
                {
                    if (((GameManager.GMinstance.GetInputDown("keyInteract") || GameManager.GMinstance.GetInputDown("keyShoot1")) && GameManager.GMinstance.gamePaused == false))
                    {
                        if (!inAnimation)
                        {
                            pressed = true;
                            ExitBox();

                        }

                    }
                    if (!inAnimation)
                    {
                        if (leaveBoxTimer > 1.5f)
                        {
                            pressed = true;
                            ExitBox();
                        }

                    }
                }
            }

            if (timer > 0.2f)
            {
                if (animationNumber < 15)
                {
                    inAnimation = true;

                    if (instantiatedBoxItems.Count > 0)
                    {
                        Destroy(instantiatedBoxItems[0]);
                    }

                    int rand = Random.Range(0, boxAnimationItems.Count);
                    GameObject instantiatedItem = Instantiate(boxAnimationItems[rand]);
                    instantiatedItem.transform.position = itemPosition.transform.position;
                    instantiatedItem.transform.eulerAngles = itemPosition.transform.eulerAngles;
                    Destroy(instantiatedItem.GetComponent<ShopItem>());
                    instantiatedBoxItems.Add(instantiatedItem);
                    animationNumber += 1;

                    timer = 0;
                }
                else
                {
                    inAnimation = false;

                    if (instantiatedBoxItems.Count > 0)
                    {
                        Destroy(instantiatedBoxItems[0]);
                    }
                    int rand = Random.Range(0, potentialBoxItems.Count);
                    GameObject instantiatedItem = Instantiate(potentialBoxItems[rand]);
                    instantiatedItem.transform.position = itemPosition.transform.position;
                    instantiatedItem.transform.eulerAngles = itemPosition.transform.eulerAngles;
                    itemSpawned = true;
                    ExitBox();
                }
            }
        }

        if (backButton.pressed == true)
        {
            if(pressedBackButton == false)
            {
                pressedBackButton = true;
                backButton.GetComponent<Image>().color = Color.green;
                CanvasScript.instance.boxBackButton.pressed = false;
            }
            else
            {
                pressedBackButton = false;
                backButton.GetComponent<Image>().color = Color.white;
                CanvasScript.instance.boxBackButton.pressed = false;
            }
            
        }

        if (buyButton.pressed == true)
        {
            if (pressedBuyButton == false)
            {
                pressedBuyButton = true;
                buyButton.GetComponent<Image>().color = Color.green;
                CanvasScript.instance.boxBuyButton.pressed = false;
            }
            else
            {
                pressedBuyButton = false;
                buyButton.GetComponent<Image>().color = Color.white;
                CanvasScript.instance.boxBuyButton.pressed = false;
            }

        }

        if (finalTextBool)
        {
            leaveBoxTimer += Time.deltaTime;
        }



        if (openBox)
        {
            Lid.transform.eulerAngles = Vector3.Lerp(Lid.transform.eulerAngles, lidEndRot.transform.eulerAngles, 0.05f);
            Lid.transform.position = Vector3.Lerp(Lid.transform.position, lidEndRot.transform.position, 0.05f);
        }

        if (zoomIn)
        {
            timeElapsed += Time.deltaTime;
            if(timeElapsed < lerpDuration)
            {
                CameraController.instance.transform.position = Vector3.Lerp(CameraController.instance.transform.position, cameraPos.transform.position, timeElapsed / lerpDuration);
                CameraController.instance.transform.eulerAngles = Vector3.Lerp(CameraController.instance.transform.eulerAngles, cameraPos.transform.eulerAngles, timeElapsed / lerpDuration);
            }
            else
            {
                CameraController.instance.transform.position = cameraPos.transform.position;
                CameraController.instance.transform.eulerAngles = cameraPos.transform.eulerAngles;
            }

        }
        else
        {
            timeElapsed = 0;
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
            CanvasScript.instance.BoxInteractText.text = "";
            inZone = false;
        }

    }

    public void ExitBox()
    {
        backButton.GetComponent<Image>().color = Color.white;
        backButton.gameObject.SetActive(true);
        pressedBackButton = false;

        buyButton.GetComponent<Image>().color = Color.white;
        buyButton.gameObject.SetActive(true);
        pressedBuyButton = false;

        backgrounds.SetActive(true);

        zoomIn = false;
        leaveBox = false;
        timerActive = false;
        interacted = false;


        finalTextBool = false;
        leaveBoxTimer = 0;

        CameraController.instance.GetComponent<Camera>().orthographic = true;
        CameraController.instance.GetComponent<Camera>().nearClipPlane = -70;
        CameraController.instance.GetComponent<CameraController>().SetCameraLocked(false);

        CameraController.instance.Start();


        CanvasScript.instance.SetCanvasMode(CanvasScript.UIMode.PLAYMODE);


        PlayerController.instance.FreezeGameplayInput(false);

        buyButton.GetComponent<Button>().interactable = true;
    }
}
