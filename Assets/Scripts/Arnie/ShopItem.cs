using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public GameObject Weapon;

    private Text gunText;
    public Text shopKeeperText;
    private ShopBackButton backButton;
    private StealButton stealButton;

    public bool interactable = false;

    public int itemPrice = 10;
    private int itemPriceDiscounted = 10;

    public GameObject cameraPos;

    public ShopInteractScript parentShop;

    public bool textChanged = false;

    public bool zoomedIn = false;

    public bool isHealth = false;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate discount, caps at 50%
        itemPriceDiscounted = itemPrice * Mathf.RoundToInt(Mathf.Max(0.5f, 1.0f - PlayerController.instance.GetModifier(ModifierType.Discount)));

        transform.name = transform.name.Replace("(Clone)", "").Trim();
        gunText = CanvasScript.instance.ShopText;
        shopKeeperText = CanvasScript.instance.ShopKeeperText;
        backButton = CanvasScript.instance.shopBackButton;
        stealButton = CanvasScript.instance.stealButton;
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable)
        {
            // Calculate discount, caps at 50%
            itemPriceDiscounted = Mathf.RoundToInt(itemPrice * Mathf.Max(0.5f, 1.0f - PlayerController.instance.GetModifier(ModifierType.Discount)));

            // StealButton
            if (stealButton.pressed)
            {
                if (stealButton.stealMode)
                {
                    shopKeeperText.text = impatientText();


                    foreach(ShopInteractScript shop in GameController.instance.ActiveShops)
                    {
                        shop.stealMode = false;
                    }

                    stealButton.GetComponent<Image>().color = Color.white;
                    stealButton.pressed = false;
                    stealButton.stealMode = false;
                }
                else
                {
                    shopKeeperText.text = stealText();

                    foreach (ShopInteractScript shop in GameController.instance.ActiveShops)
                    {
                        shop.stealMode = true;
                    }

                    stealButton.GetComponent<Image>().color = Color.red;
                    stealButton.pressed = false;
                    stealButton.stealMode = true;
                }

            }
        }

        if (interactable)
        {
            if (parentShop.zoomedInOnItem == true) 
            {
                if (backButton.pressed == true)
                {
                    Debug.Log(110101011);
                    foreach (ShopInteractScript shop in GameController.instance.ActiveShops)
                    {
                        parentShop.activeCameraPos = parentShop.cameraPos;
                        backButton.pressed = false;
                        textChanged = false;
                        shopKeeperText.text = impatientText();

                        parentShop.zoomedInOnItem = false;
                    }

                }
            }
        }


        if (interactable)
        {
            if (parentShop.zoomedInOnItem == false)
            {
                if (backButton.pressed == true)
                {
                    foreach (ShopInteractScript shop in GameController.instance.ActiveShops)
                    {
                        Debug.Log(222121212);
                        shop.activeCameraPos = shop.cameraPos;
                        backButton.pressed = false;
                        textChanged = false;
                        shopKeeperText.text = impatientText();

                        shop.zoomedInOnItem = false;

                        shop.ExitShop();
                    }
                }
            }
        }
        

        
    }

    public void CurrentClickedGameObject(GameObject clickedGameObject)
    {
        if(PlayerController.instance.GetCoinValue() >= itemPriceDiscounted)
        {
            if (clickedGameObject == this.gameObject)
            {
                if (!isHealth)
                {
                    GameObject weaponInstance = Instantiate(Weapon);
                    weaponInstance.transform.position = new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y + 1, PlayerController.instance.transform.position.z);
                    Destroy(this.gameObject);
                    if (!parentShop.stealMode)
                    {
                        PlayerController.instance.ChangeCoinValue(-itemPriceDiscounted);
                    }
                    
                }
                else
                {
                    if(PlayerController.instance.GetHealth() == PlayerController.instance.GetMaxHealth())
                    {
                        shopKeeperText.text = "I would sell you this but your health is already full and I'm a nice woman";
                    }
                    else
                    {
                        shopKeeperText.text = "Thankyou for your buisness, you can buy as much health as you want";
                        PlayerController.instance.ChangeCoinValue(-itemPriceDiscounted);
                        PlayerController.instance.Heal(25);
                    }
                }
                
            }
        }
        else
        {
            if (clickedGameObject == this.gameObject)
            {
                if(parentShop.clickAtempts >= 15 && parentShop.clickAtempts <= 19 && !isHealth)
                {
                    shopKeeperText.text = stealText();
                    parentShop.clickAtempts += 1;
                }
                else if (parentShop.clickAtempts >= 20 && parentShop.clickAtempts <= 24 && !isHealth)
                {
                    shopKeeperText.color = Color.red;
                    shopKeeperText.text = "The path you're choosing ends in pain.";
                    parentShop.clickAtempts += 1;
                }
                else if (parentShop.clickAtempts >= 25 && !isHealth)
                {
                    parentShop.stealMode = true;
                    GameObject weaponInstance = Instantiate(Weapon);
                    weaponInstance.transform.position = new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y + 1, PlayerController.instance.transform.position.z);
                    Destroy(this.gameObject);
                }
                else if (parentShop.stealMode == true && !isHealth)
                {
                    GameObject weaponInstance = Instantiate(Weapon);
                    weaponInstance.transform.position = new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y + 1, PlayerController.instance.transform.position.z);
                    Destroy(this.gameObject);
                }
                else
                {
                    shopKeeperText.color = Color.white;
                    shopKeeperText.text = rejectionText();
                    if (!isHealth)
                    {
                        parentShop.clickAtempts += 1;
                    }
                    
                }
                
            }
        }
        
    }

    public string rejectionText()
    {
        if (!isHealth)
        {
            int rand = Random.Range(1, 6);
            if (rand == 1)
            {
                return "I dont think you have the facilities for that big man";
            }
            if (rand == 2)
            {
                return "I'm not a charity, cough up the cash or you'll be cruising for a bruising ";
            }
            if (rand == 3)
            {
                return "Save your pennies and maybe you can afford some of my merchandise";
            }
            if (rand == 4)
            {
                return "What does this look like to you? A handout? Scram kid";
            }
            if (rand == 5)
            {
                return "This is high quality stock, cough up the dough or get out of here";
            }
            return "The programmmer made a number out of range lol";
        }
        else
        {
            return "Sorry, it looks like you don't have enough money ";
        }
    }

    public string priceText()
    {
        textChanged = true;
        if (!isHealth)
        {
            int rand = Random.Range(1, 9);
            if (rand == 1)
            {
                return "Ahhhh the " + gameObject.name + ". Hmmmm for you I will charge" + " $" + itemPriceDiscounted;
            }
            if (rand == 2)
            {
                return "I too am partial to the " + gameObject.name + ". Lets call it a even" + " $" + itemPriceDiscounted;
            }
            if (rand == 3)
            {
                return "You wont find a " + gameObject.name + " anywhere else for as cheap as" + " $" + itemPriceDiscounted;
            }
            if (rand == 4)
            {
                return "That was my fathers " + gameObject.name + ". Look, I like you, so I will charge only" + " $" + itemPriceDiscounted;
            }
            if (rand == 5)
            {
                return "You can't go wrong with the " + gameObject.name + ". For a weapon like that its gonna cost you " + " $" + itemPriceDiscounted;
            }
            if (rand == 6)
            {
                return gameObject.name + "'s famously are really good against bosses, or so I hear. We will call it " + " $" + itemPriceDiscounted;
            }
            if (rand == 7)
            {
                return "For " + " $" + itemPriceDiscounted + "! Act quick or I might keep this " + gameObject.name + " for myself";
            }
            if (rand == 8)
            {
                return "You can't be sentimental in this world, but it pains me to let this " + gameObject.name + " go for " + " $" + itemPriceDiscounted;
            }
        }
        else
        {
            return "Health packs are a expensive commodity in the apocalypse. It costs " + " $" + itemPriceDiscounted;
        }
        
        return "the programmer made a number out of range lol";
    }

    public string impatientText()
    {
        CanvasScript.instance.ShopKeeperText.color = Color.white;

        if (!isHealth)
        {
            int rand = Random.Range(1, 6);

            if (rand == 1)
            {
                return "I haven't got all day nor have I the patience for your window shopping!";
            }
            if (rand == 2)
            {
                return "I like standing around doing nothing too but if you're not gonna make a purchase then leave now.";
            }
            if (rand == 3)
            {
                return "Look you better make a purchase soon, I'm trying to make a living here";
            }
            if (rand == 4)
            {
                return "You do realise the score is tied to the speed you complete the game right?";
            }
            if (rand == 5)
            {
                return "You're holding up the line pal, either buy something or get lost";
            }
        }
        else
        {
            return "Take all the time you need.";
        }
        
        return "the programmer made a number out of range lol";
    }

    public string stealText()
    {
        return "I wouldn't try to steal from me if I was you.";
    }

}
