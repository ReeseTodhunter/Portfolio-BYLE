using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    public GameObject selectedCharacter;
    public GameObject characterParent;
    public GameObject mediumCharacterParent;
    public GameObject largeCharacterParent;

    public Text buttonText;

    private GameObject InstantiatedCharacter;

    private bool pressed = false;

    [TextArea(15,20)]
    public string bio = "This is the default text, it means something needs to be typed in the inspector";

    public Camera smallCamera;
    public Camera mediumCamera;
    public Camera largeCamera;
    public Camera weaponCamera;

    public int ID;

    
    public GameObject lockedImage;

    private bool buttonLocked = false;

    public enum EnemySize
    {
        SMALL,
        MEDIUM,
        LARGE
            
    }
    public EnemySize enemySize;

    public bool isBoss = false;


    // Start is called before the first frame update
    void Start()
    {
        // Instantiates the prefab that coincides with the button
        InstantiatedCharacter = Instantiate(selectedCharacter, characterParent.transform);
        InstantiatedCharacter.transform.position = new Vector3(1000, 1000, -1000);


        // Sets the name differently deppending on if its a enemy, weapon or powerup
        gameObject.GetComponentInChildren<TMP_Text>().text = "\n\n\n\n\n" + selectedCharacter.name.ToString();
        if(isBoss == true)
        {
            gameObject.GetComponentInChildren<TMP_Text>().text = "\n\n\n\n\n\n\n\n\n" + selectedCharacter.name.ToString();
        }

        if (InstantiatedCharacter.TryGetComponent(out BaseActivePowerup activePower))
        {
            gameObject.GetComponentInChildren<TMP_Text>().text = "\n\n\n\n\n" + InstantiatedCharacter.GetComponent<BaseActivePowerup>().GetTitle();
        }
        if (InstantiatedCharacter.GetComponent<BasePowerup>() != null)
        {
            gameObject.GetComponentInChildren<TMP_Text>().text = "\n\n\n\n\n" + InstantiatedCharacter.GetComponent<BasePowerup>().GetTitle();
        }

        if (InstantiatedCharacter.GetComponent<BTAgent>() != null)
        {
            if (EncyclopediaScript.instance.enableLockedEnemies)
            {
                if (ID != 0)
                {
                    if (GameManager.GMinstance.enemiesEncountered[ID] == false)
                    {
                        gameObject.GetComponent<Image>().color = new Color32(30, 30, 30, 255);
                        gameObject.GetComponentInChildren<TMP_Text>().text = "\n\n\n\n\n" + "?????";

                        GameObject locked = Instantiate(lockedImage, gameObject.transform);
                        locked.transform.position = transform.position;

                        buttonLocked = true;
                    }
                }
            }
        }

        if (InstantiatedCharacter.GetComponent<ShopItem>() != null)
        {
            if (EncyclopediaScript.instance.enableLockedEnemies)
            {
                if (GameManager.GMinstance.weaponsPickedUp[ID] == false)
                {
                    gameObject.GetComponent<Image>().color = new Color32(30, 30, 30, 255);
                    gameObject.GetComponentInChildren<TMP_Text>().text = "\n\n\n\n\n" + "?????";

                    GameObject locked = Instantiate(lockedImage, gameObject.transform);
                    locked.transform.position = transform.position;

                    buttonLocked = true;
                }
            }
        }

        if (InstantiatedCharacter.GetComponent<BasePowerup>() != null || InstantiatedCharacter.GetComponent<BaseActivePowerup>() != null)
        {
            if (EncyclopediaScript.instance.enableLockedEnemies)
            {
                if (GameManager.GMinstance.powerupsPickedUp[ID] == false)
                {
                    gameObject.GetComponent<Image>().color = new Color32(30, 30, 30, 255);
                    gameObject.GetComponentInChildren<TMP_Text>().text = "\n\n\n\n\n" + "?????";

                    GameObject locked = Instantiate(lockedImage, gameObject.transform);
                    locked.transform.position = transform.position;

                    buttonLocked = true;
                }
            }
        }



        // Destroys this version of the character (it was only needed to retrieve the name)
        Destroy(InstantiatedCharacter);

        

    }

    // Update is called once per frame
    void Update()
    {
        
        if (!buttonLocked)
        {
            if (pressed)
            {
                weaponCamera.gameObject.SetActive(false);

                // Checks the size of the displayed item and check what type it is (enemy/powerup/weapon)
                if (enemySize == EnemySize.SMALL)
                {
                    // Activates the nessasary camera
                    smallCamera.gameObject.SetActive(true);
                    mediumCamera.gameObject.SetActive(false);
                    largeCamera.gameObject.SetActive(false);

                    characterParent.transform.eulerAngles = new Vector3(0, 180, 0);
                    EncyclopediaScript.instance.bioText.text = bio;
                    EncyclopediaScript.instance.titleText.text = selectedCharacter.name.ToString();

                    EncyclopediaScript.instance.hologramLightActivate = true;
                    EncyclopediaScript.instance.SetCanvasMode(EncyclopediaScript.UIMode.VIEW);
                    InstantiatedCharacter = Instantiate(selectedCharacter, characterParent.transform);
                    if (InstantiatedCharacter.TryGetComponent(out BTAgent agent))
                    {
                        agent.enabled = false;
                        InstantiatedCharacter.transform.position = new Vector3(0, 0.75f, 0);
                        InstantiatedCharacter.transform.eulerAngles = new Vector3(0, -90, 0);



                    }
                    if (InstantiatedCharacter.TryGetComponent(out Animator animator))
                    {
                        animator.enabled = false;
                    }
                    if (InstantiatedCharacter.TryGetComponent(out ShopItem weapon))
                    {
                        weapon.enabled = false;
                        InstantiatedCharacter.transform.position = new Vector3(0, 1.75f, 0);
                        InstantiatedCharacter.transform.eulerAngles = new Vector3(0, -90, 0);

                        bio = weapon.Weapon.GetComponent<BaseWeapon>().GetDescription();
                        EncyclopediaScript.instance.bioText.text = bio;

                        smallCamera.gameObject.SetActive(false);
                        weaponCamera.gameObject.SetActive(true);
                    }
                    if (InstantiatedCharacter.TryGetComponent(out BasePowerup powerUp))
                    {
                        powerUp.enabled = false;
                        InstantiatedCharacter.transform.position = new Vector3(0, 1.75f, 0);
                        InstantiatedCharacter.transform.eulerAngles = new Vector3(0, 90, 0);

                        string title = powerUp.GetComponent<BasePowerup>().GetTitle();
                        EncyclopediaScript.instance.titleText.text = title;

                        bio = powerUp.GetComponent<BasePowerup>().GetDescriptionEncyclopedia();
                        EncyclopediaScript.instance.bioText.text = bio;

                        smallCamera.gameObject.SetActive(false);
                        weaponCamera.gameObject.SetActive(true);
                    }
                    if (InstantiatedCharacter.TryGetComponent(out BaseActivePowerup activePowerUp))
                    {
                        activePowerUp.enabled = false;
                        InstantiatedCharacter.transform.position = new Vector3(0, 1.75f, 0);
                        InstantiatedCharacter.transform.eulerAngles = new Vector3(0, 90, 0);

                        string title = activePowerUp.GetComponent<BaseActivePowerup>().GetTitle();
                        EncyclopediaScript.instance.titleText.text = title;

                        bio = activePowerUp.GetComponent<BaseActivePowerup>().GetDescription();
                        EncyclopediaScript.instance.bioText.text = bio;

                        smallCamera.gameObject.SetActive(false);
                        weaponCamera.gameObject.SetActive(true);
                    }

                    EncyclopediaScript.instance.SetSizeImage(0);

                    pressed = false;
                }
                else if (enemySize == EnemySize.MEDIUM)
                {
                    // Activates the nessasary camera
                    smallCamera.gameObject.SetActive(false);
                    mediumCamera.gameObject.SetActive(true);
                    largeCamera.gameObject.SetActive(false);

                    characterParent.transform.eulerAngles = new Vector3(0, 180, 0);
                    EncyclopediaScript.instance.bioText.text = bio;
                    EncyclopediaScript.instance.titleText.text = selectedCharacter.name.ToString();

                    EncyclopediaScript.instance.hologramLightActivate = true;
                    EncyclopediaScript.instance.SetCanvasMode(EncyclopediaScript.UIMode.VIEW);
                    InstantiatedCharacter = Instantiate(selectedCharacter, mediumCharacterParent.transform);
                    if (InstantiatedCharacter.TryGetComponent(out BTAgent agent))
                    {
                        agent.enabled = false;
                    }
                    if (InstantiatedCharacter.TryGetComponent(out Animator animator))
                    {
                        //animator.enabled = false;
                    }
                    if (InstantiatedCharacter.TryGetComponent(out Weapon weapon))
                    {
                        weapon.enabled = false;
                    }

                    InstantiatedCharacter.transform.position = new Vector3(-74.1f, 2.75f, 0);
                    InstantiatedCharacter.transform.eulerAngles = new Vector3(0, -90, 0);

                    EncyclopediaScript.instance.SetSizeImage(1);

                    pressed = false;
                }
                else if (enemySize == EnemySize.LARGE)
                {
                    // Activates the nessasary camera
                    smallCamera.gameObject.SetActive(false);
                    mediumCamera.gameObject.SetActive(false);
                    largeCamera.gameObject.SetActive(true);

                    characterParent.transform.eulerAngles = new Vector3(0, 180, 0);
                    EncyclopediaScript.instance.bioText.text = bio;
                    EncyclopediaScript.instance.titleText.text = selectedCharacter.name.ToString();

                    EncyclopediaScript.instance.hologramLightActivate = true;
                    EncyclopediaScript.instance.SetCanvasMode(EncyclopediaScript.UIMode.VIEW);
                    InstantiatedCharacter = Instantiate(selectedCharacter, largeCharacterParent.transform);
                    if (InstantiatedCharacter.TryGetComponent(out BTAgent agent))
                    {
                        agent.enabled = false;
                    }
                    if (InstantiatedCharacter.TryGetComponent(out Animator animator))
                    {
                        //animator.enabled = false;
                    }
                    if (InstantiatedCharacter.TryGetComponent(out Weapon weapon))
                    {
                        weapon.enabled = false;
                    }

                    InstantiatedCharacter.transform.position = new Vector3(63, 2.75f, 0);
                    InstantiatedCharacter.transform.eulerAngles = new Vector3(0, -90, 0);

                    EncyclopediaScript.instance.SetSizeImage(2);

                    pressed = false;
                }

            }
        }
        
    }
    public void OnPressTheButton(GameObject Character)
    {
        // Plays the blip noise
        ButtonBlipScript.instance.PlayBlip();

        selectedCharacter = Character;
        pressed = true;
        
    }
}
