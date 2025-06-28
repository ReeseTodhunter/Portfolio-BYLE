using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PowerupUI : MonoBehaviour
{
    public static PowerupUI instance; // :troll_face:
    public GameObject iconParent; // Parent object holding all the objects with the image component
    public GameObject subtitle; // HUD subtitle containing the key to show post-5 powerups
    public GameObject hoverBox; // Box that displays the powerup's item and description
    IconHolder[] iconHolders; // Array of all the image components

    int counter = 0; // Counts how many icons are active

    public float animationSpeed = 4f; // Speed of animation (total length is equal to 1 / speed)
    float animationTimer = 0f; // Timer used for animating between full and minimised view

    float subtitleHeightOffset; // Height above the highest row of icons that the subtitle sits at. Set in Start()
    float minimisedTransparency = 0.65f; // Transparency of the UI when not maximised

    // Instance
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }
        instance = this;
    }

    private void Start()
    {
        if (iconParent == null) iconParent = gameObject; // Assume gameobject of this script is the icon parent if none is assigned in editor
        Image[] allIcons = iconParent.GetComponentsInChildren<Image>();
        iconHolders = new IconHolder[allIcons.Length];

        // Set up icon holder array
        for (int i = 0; i < iconHolders.Length; ++i)
        {
            // Set icon
            iconHolders[i].icon = allIcons[i];

            // Make inactive, as icons with NULL sprites are just white boxes
            iconHolders[i].icon.gameObject.SetActive(false);

            // First 5 icons are semi transparent by default, rest are invisible
            if (i < 5) iconHolders[i].baseTransparency = minimisedTransparency;
            else iconHolders[i].baseTransparency = 0f;
            iconHolders[i].icon.color = new Color(1f, 1f, 1f, iconHolders[i].baseTransparency);

            // Set names
            iconHolders[i].icon.gameObject.name = "Icon " + i.ToString();

            // Default hover text
            iconHolders[i].title = "";
            iconHolders[i].description = "";

            // Set local y positions for animation
            iconHolders[i].yPos = iconHolders[i].icon.transform.localPosition.y;
            iconHolders[i].icon.transform.localPosition = new Vector3(iconHolders[i].icon.transform.localPosition.x, 0f, iconHolders[i].icon.transform.localPosition.z);

            // Intialise icons for description box calls
            iconHolders[i].icon.GetComponent<PowerupIconUI>().Initialise(this, i);
        }

        // Set up subtitle
        if (subtitle != null)
        {
            subtitle.SetActive(false); // Hide
            subtitle.GetComponent<Text>().color = new Color(1f, 1f, 1f, minimisedTransparency); // Set transparency
            subtitleHeightOffset = subtitle.transform.localPosition.y; // Get height offset
        }
    }

    private void Update()
    {
        // Update animation timer
        bool isAnimating = false;
        if (animationTimer < 1f && GameManager.GMinstance.GetInput("keyHUD"))
        {
            isAnimating = true;
            animationTimer = Mathf.Min(1f, animationTimer + (Time.unscaledDeltaTime * animationSpeed));
        }
        else if (animationTimer > 0f && !GameManager.GMinstance.GetInput("keyHUD"))
        {
            isAnimating = true;
            animationTimer = Mathf.Max(0f, animationTimer - (Time.unscaledDeltaTime * animationSpeed));
        }

        // Used in lerp calculations so that animations are smoother instead of completely linear
        float animationGradient = Mathf.Sin(animationTimer * Mathf.PI * 0.5f);

        // Play animation
        if (isAnimating)
        {
            // Icons
            foreach(IconHolder iconHolder in iconHolders)
            {
                // Move up/down screen
                iconHolder.icon.transform.localPosition = new Vector3(iconHolder.icon.transform.localPosition.x,
                    Mathf.Lerp(0f, iconHolder.yPos, animationGradient),
                    iconHolder.icon.transform.localPosition.y);

                // Transparency
                iconHolder.icon.color = new Color(1f, 1f, 1f, Mathf.Lerp(iconHolder.baseTransparency, 1f, animationGradient));
            }

            // Subtitle
            if (subtitle != null && subtitle.activeSelf)
            {
                // Move subtitle
                subtitle.transform.localPosition = new Vector3(subtitle.transform.localPosition.x,
                    Mathf.Lerp(0f, iconHolders[counter - 1].yPos, animationGradient) + subtitleHeightOffset,
                    subtitle.transform.localPosition.z);

                // Subtitle transparency
                subtitle.GetComponent<Text>().color = new Color(1f, 1f, 1f, Mathf.Lerp(minimisedTransparency, 1f, animationGradient));
            }
        }

        // Hover text box
        if (hoverBox != null)
        {
            // Move text box to follow mouse if ui is expanded, otherwise just hide it
            if (animationTimer > 0f)
            {
                hoverBox.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            }
            else
            {
                hoverBox.SetActive(false);
            }
        }
    }

    // Inserts a powerup sprite into the first position of the iconholders array
    public void AddPowerup(Sprite icon, string title, string description)
    {
        // Increase counter (does not exceed array size)
        counter = Mathf.Min(counter + 1, iconHolders.Length); // Update counter

        // Update icons sprites and descriptions
        for (int i = iconHolders.Length - 1; i > 0; --i)
        {
            iconHolders[i].icon.sprite = iconHolders[i - 1].icon.sprite;
            iconHolders[i].title = iconHolders[i - 1].title;
            iconHolders[i].description = iconHolders[i - 1].description;
        }
        iconHolders[0].icon.sprite = icon;
        iconHolders[0].title = title;
        iconHolders[0].description = description;

        // Update icon visibilities
        iconHolders[counter - 1].icon.gameObject.SetActive(true);

        // Update subtitle visibility
        subtitle.SetActive(true);

        // Manually move subtitle if in expanded view as animation will no longer be playing at this point and wont update on it's own
        if (animationTimer == 1f)
        {
            subtitle.transform.localPosition = new Vector3(subtitle.transform.localPosition.x,
                    iconHolders[counter - 1].yPos + subtitleHeightOffset,
                    subtitle.transform.localPosition.z);
        }
    }

    public void SetBox(int index)
    {
        if (animationTimer > 0f)
        {
            iconHolders[index].icon.transform.localScale = Vector3.one * 1.2f;
            hoverBox.GetComponent<ItemDescriptionBox>().SetText(iconHolders[index].title, iconHolders[index].description);
            hoverBox.SetActive(true);
        }
    }
    public void RemoveBox(int index)
    {
        iconHolders[index].icon.transform.localScale = Vector3.one;
        hoverBox.SetActive(false);
    }
}

struct IconHolder
{
    public Image icon; // Image component of the icon
    public string title; // Title of the powerup seen when hovering
    public string description; // ^^ for the description
    public float baseTransparency; // Transparency of the icon when not in expanded view. Used for animation
    public float yPos; // Y position of the icon in expanded view. Used for animation
}