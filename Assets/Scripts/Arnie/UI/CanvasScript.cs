using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    // Singleton instance
    public static CanvasScript instance;

    public GameObject GameUi;
    public GameObject ShopUI;
    public GameObject BoxUI;

    public Text ShopText;
    public Text ShopKeeperText;
    public Text InteractText;
    public Text BoxInteractText;
    public ShopBackButton shopBackButton;
    public BoxbackButton boxBackButton;
    public BoxBuyButton boxBuyButton;
    public StealButton stealButton;
    public Text BoxDisplayText;
    public TitleScript roomClear;
    public TitleScript title;

    // Buttons for shop
    public ShopViewButton LeftButton;
    public ShopViewButton MiddleButton;
    public ShopViewButton RightButton;

    public GameObject shopButtonBackgrounds;

    public AmbulanceViewButton AmbulanceButton;

    public GameObject ambulanceButtonBackground;

    public GameObject boxBackgrounds;
    public enum UIMode
    {
        PLAYMODE,
        SHOPMODE,
        BOXMODE,
        CINEMATICMODE
    }

    private UIMode currentUIMode;

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
        SetCanvasMode(UIMode.PLAYMODE);
    }
    public void SetCanvasMode(UIMode mode)
    {
        GameUi.SetActive(false);
        ShopUI.SetActive(false);
        BoxUI.SetActive(false);
        switch(mode)
        {
            case UIMode.PLAYMODE:
            GameUi.SetActive(true);
            currentUIMode = mode;
            break;
            case UIMode.SHOPMODE:
            ShopUI.SetActive(true);
            currentUIMode = mode;
            break;
            case UIMode.BOXMODE:
            currentUIMode = mode;
            BoxUI.SetActive(true);
            break;
            case UIMode.CINEMATICMODE:
            currentUIMode = mode;
            break;
        }
    }

    public UIMode GetCanvasMode()
    {
        return currentUIMode;
    }

}
