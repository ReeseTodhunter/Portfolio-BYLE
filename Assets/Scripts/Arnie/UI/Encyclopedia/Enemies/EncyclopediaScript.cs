using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaScript : MonoBehaviour
{
    public GameObject background;

    // Singleton instance
    public static EncyclopediaScript instance;

    public GameObject EncyclopediaUI;
    public GameObject ViewUI;

    public Text titleText;
    public Text bioText;
    public Text pageText;
    public enum UIMode
    {
        ENCYCLOPEDIA,
        VIEW,
    }

    private UIMode currentUIMode;

    public GameObject HologramLight;
    public bool hologramLightActivate = false;
    public bool hologramLightDeactivate = false;
    private float ActivateTimer = 0;
    private float DeactivateTimer = 0;
    public float lerpDuration = 1;
    private Vector3 InitialHologramLightTransform;

    public List<GameObject> Pages = new List<GameObject>();
    public int pageNumber = 1;

    public Sprite small;
    public Sprite medium;
    public Sprite large;

    public Image sizeImage;

    public bool enableLockedEnemies = false;

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
        SetCanvasMode(UIMode.ENCYCLOPEDIA);
    }

    public void Start()
    {
        InitialHologramLightTransform = HologramLight.transform.localScale;
        changePageNumber(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (hologramLightActivate)
        {
            ActivateTimer += Time.deltaTime;
            if (ActivateTimer < lerpDuration)
            {
                HologramLight.transform.localScale = Vector3.Lerp(HologramLight.transform.localScale, new Vector3(HologramLight.transform.localScale.x, InitialHologramLightTransform.y + 8, HologramLight.transform.localScale.z), ActivateTimer / lerpDuration);
            }
            else
            {
                hologramLightDeactivate = true;
                hologramLightActivate = false;
            }
        }
        else
        {
            ActivateTimer = 0;
        }
        if (hologramLightDeactivate)
        {
            DeactivateTimer += Time.deltaTime;
            if (DeactivateTimer < lerpDuration)
            {
                HologramLight.transform.localScale = Vector3.Lerp(HologramLight.transform.localScale, new Vector3(HologramLight.transform.localScale.x, InitialHologramLightTransform.y, HologramLight.transform.localScale.z), DeactivateTimer / lerpDuration);
            }
            else
            {
                hologramLightDeactivate = false;
                HologramLight.transform.localScale = InitialHologramLightTransform;
                hologramLightActivate = false;
            }
        }
        else
        {
            DeactivateTimer = 0;
        }

    }

    public void SetCanvasMode(UIMode mode)
    {
        EncyclopediaUI.SetActive(false);
        ViewUI.SetActive(false);
        switch (mode)
        {
            case UIMode.ENCYCLOPEDIA:
                background.SetActive(true);
                EncyclopediaUI.SetActive(true);
                currentUIMode = mode;
                break;
            case UIMode.VIEW:
                background.SetActive(false);
                ViewUI.SetActive(true);
                currentUIMode = mode;
                break;

        }
    }

    public void changePageNumber(int number)
    {
        if (pageNumber + number != 0 && pageNumber + number != Pages.Count + 1)
        {
            pageNumber = pageNumber + number;
        }

        foreach(GameObject page in Pages)
        {
            page.SetActive(false);
        }
        Pages[pageNumber - 1].SetActive(true);
        pageText.text = "PAGE " + pageNumber + "/" + Pages.Count;
    }

    public void SetSizeImage(int size)
    {
        if(sizeImage != null)
        {
            if (size == 0)
            {
                sizeImage.sprite = small;
            }
            if (size == 1)
            {
                sizeImage.sprite = medium;
            }
            if (size == 2)
            {
                sizeImage.sprite = large;
            }
        }
        
    }
}
