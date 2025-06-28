using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public GameObject UI;
    public GameObject settingsUI;
    public GameObject rebindUI;

    [SerializeField]
    public GameObject[] menuIcons;
    public int seletedIcon;

    private GameObject menuChoice;
    private void Awake()
    {
        if (instance!= null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
        ToggleUI(false);
    }


    private void Update()
    {
        if (GameManager.GMinstance.gamePaused)
        {
            /*
            seedText.text = "Seed: " + GameManager.GMinstance.levelSeed.ToString();
            //Pause Menu Navigation
            //seletedIcon = MenuIcons[];
            //A & D rotate the base object
            //whilst also going up and down through the list
            //The child text object is also turned on and off depending whether
            //or not it's the selected item
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (seletedIcon > 0)
                {
                    menuChoice.SetActive(false);
                    seletedIcon -= 1;
                }
                else
                {
                    seletedIcon = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (seletedIcon < menuIcons.Length - 1)
                {
                    menuChoice.SetActive(false);
                    seletedIcon += 1;
                }
                else
                {
                    seletedIcon = menuIcons.Length - 1;
                }
            }
            //if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            //{
            //    onResume();
            //}
            */


            /*menuChoice.SetActive(false);
            //Set the selected Menu item
            menuChoice = menuIcons[seletedIcon];

            switch (seletedIcon)
            {
                case 0:
                    revolverBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    menuChoice.SetActive(true);
                    break;
                case 1:
                    revolverBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, 45.0f);
                    menuChoice.SetActive(true);
                    break;
                case 2:
                    revolverBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                    menuChoice.SetActive(true);
                    break;
                case 3:
                    revolverBase.transform.eulerAngles = new Vector3(0.0f, 0.0f, 135.0f);
                    menuChoice.SetActive(true);
                    break;
            }
            */
        }
    }

    
    //Pause Menu actions
    public void onResume()
    {
        
        UI.SetActive(false);
        settingsUI.SetActive(false);
        rebindUI.SetActive(false);
        Debug.Log("Resume");
        GameManager.GMinstance.Pause(false);
        GameManager.GMinstance.gamePaused = false;
        
    }
    public void onAudio()
    {
        Debug.Log("Audio Option");
    }
    public void onSettings()
    {
        Debug.Log("Settings Option");
        settingsUI.SetActive(settingsUI.activeSelf ? false : true);
        rebindUI.SetActive(false);
    }
    public void onRebind()
    {
        settingsUI.SetActive(false);
        rebindUI.SetActive(true);
    }
    public void onExit()
    {
        
        GameManager.GMinstance.ReturnToMainMenu();
    }
    
    public void ToggleUI(bool _isEnabled){ 
        UI.SetActive(_isEnabled);
        settingsUI.SetActive(_isEnabled);
        rebindUI.SetActive(_isEnabled);
    }
    public bool GetUIEnabled() { return UI.activeSelf; }
    public void ReturnToMainMenu()
    {
        GameManager.GMinstance.ReturnToMainMenu();
    }
}

public class MenuIcons
{

}
