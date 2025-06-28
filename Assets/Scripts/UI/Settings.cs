using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    //Create a struct of saved settings

    public TMP_Dropdown resolutionDropdown;
    public List<TMP_Dropdown.OptionData> resolutionDropdownList = new List<TMP_Dropdown.OptionData>();
    public Toggle Fullscreen;

    public int[,] resolution = new int[,] { { 640, 360 }, { 800, 600 }, { 1024, 768 }, { 1280, 720 }, { 1280, 800 }, { 1280, 1024 }, { 1360, 768 }, { 1366, 768 }, { 1440, 900 }, { 1536, 864 },
                                            { 1600,900 },{1600,1200 },{1680,1050 },{1920,1080 },{1920,1200 },{2048,1152 },{2048,1536 },{2560,1080 },{2560,1440 },{2560,1600 },{3440,1440 },{3840,2160 } };
    public List<string> resolutionNames = new List<string>();
    int chosenResolutionX;
    int chosenResolutionY;
    int resolutionArrayChoice;
    bool isFullscreen = false;
    bool isVsync = false;

    public Slider masterAudioSlider;
    public Slider FXAudioSlider;
    public Slider musicAudioSlider;
    [Range(0, 1)]
    private float masterVolume;
    public float MasterVolume { get => masterVolume; }
    [Range(0, 1)]
    private float fxVolume;
    public float FXVolume { get => fxVolume; }
    [Range(0, 1)]
    private float musicVolume;
    public float MusicVolume { get => musicVolume; }

    public Toggle easyDifficulty;
    public Toggle mediumDifficulty;
    public Toggle hardDifficulty;
    public int difficultyChoice;

    public Toggle vSyncToggle;



    //Things that go in Settings menus
    /*
     * Whole thing scrolls
     * Resolution
     * Fullscreen
     * Audio Levels 
     * Delete Saves
     * Change Keybinds (has a separate menu)
     * V-Sync
     * Difficulty Setting ?
     */

    // List of resolutions
    // Chosen resolution depends on what you click

   

    

    private void Awake()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        masterAudioSlider.value = PlayerPrefs.GetFloat("masterVolume");
        fxVolume = PlayerPrefs.GetFloat("fxVolume");
        FXAudioSlider.value = fxVolume;
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        musicAudioSlider.value = musicVolume;

        difficultyChoice  = PlayerPrefs.GetInt("difficulty");
        chosenResolutionX = PlayerPrefs.GetInt("chosenResolutionX");
        chosenResolutionY = PlayerPrefs.GetInt("chosenResolutionY");
        resolutionArrayChoice = PlayerPrefs.GetInt("resolutionArrayChoice");

        if (PlayerPrefs.GetInt("isFullscreen") == 1)
        {
            Fullscreen.isOn = true;
            isFullscreen = true;
        }
        else
        {
            Fullscreen.isOn = false;
            isFullscreen = false;
        }
       
        if (chosenResolutionX <= 640 || chosenResolutionX <= 360)
        {
            chosenResolutionX = Screen.width;
            chosenResolutionY = Screen.height;
            isFullscreen = true;
            Fullscreen.isOn = true;
            resolutionDropdown.value = 13;
            PlayerPrefs.SetInt("resolutionArrayChoice", 13);
        }
        else
        {
           resolutionDropdown.value = PlayerPrefs.GetInt("resolutionArrayChoice");
        }

        for (int i = 0; i < resolution.Length/2; i++)
        {
            resolutionNames.Add((resolution[i, 0]+" x "+ resolution[i,1]).ToString());
            
        }

        resolutionDropdown.AddOptions(resolutionNames);

        resolutionDropdown.onValueChanged.AddListener(delegate { UpdateResolution(resolutionDropdown.value); });
        
        Fullscreen.onValueChanged.AddListener(delegate
        {
            if (Fullscreen.isOn) isFullscreen = true;
            else isFullscreen = false;
            PlayerPrefs.SetInt("isFullscreen", isFullscreen ? 1 : 0);
            GameManager.GMinstance.UpdateSettings();
        });

        masterAudioSlider.onValueChanged.AddListener(delegate { UpdateAudio(); });
        FXAudioSlider.onValueChanged.AddListener(delegate { UpdateAudio(); });
        musicAudioSlider.onValueChanged.AddListener(delegate { UpdateAudio(); });

        easyDifficulty.onValueChanged.AddListener(delegate { UpdateDifficulty(); });
        mediumDifficulty.onValueChanged.AddListener(delegate { UpdateDifficulty(); });
        hardDifficulty.onValueChanged.AddListener(delegate { UpdateDifficulty(); });

        // V-Sync
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("isVsync");
        vSyncToggle.onValueChanged.AddListener(delegate { UpdateVSync(); });
    }

    private void Update()
    {
        resolutionDropdown.value = resolutionArrayChoice;
    }

    public void DebugSettings()
    {
       // Debug.Log();
        Debug.Log(resolutionNames);
        Debug.Log(chosenResolutionX.ToString() +" " + chosenResolutionY.ToString());
        Debug.Log(resolution[chosenResolutionX,chosenResolutionY]);
        Debug.Log(difficultyChoice);
    }
    public void UpdateResolution(int chosenResolution)
    {
        Debug.Log(chosenResolution);
        Debug.Log(resolution[chosenResolution, 0].ToString() + " x " + resolution[chosenResolution, 1].ToString());
        
        chosenResolutionX = resolution[chosenResolution, 0];
        chosenResolutionY = resolution[chosenResolution, 1];
        PlayerPrefs.SetInt("chosenResolutionX", chosenResolutionX);
        PlayerPrefs.SetInt("chosenResolutionY", chosenResolutionY);
        PlayerPrefs.SetInt("isFullscreen", isFullscreen ? 1 : 0);
        resolutionArrayChoice = chosenResolution;
        PlayerPrefs.SetInt("resolutionArrayChoice",chosenResolution);
        GameManager.GMinstance.UpdateSettings();
    }

    public void UpdateAudio()
    {
        masterVolume = masterAudioSlider.value;
        fxVolume = FXAudioSlider.value;
        musicVolume = musicAudioSlider.value;

        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("fxVolume", fxVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        //Debug.Log("Updated Audio " + masterVolume + " " + fxVolume + " " + musicVolume);
        GameManager.GMinstance.UpdateSettings();
    }

    public void UpdateDifficulty()
    {
        if (easyDifficulty.isOn)        difficultyChoice = 0;
        else if (mediumDifficulty.isOn) difficultyChoice = 1;
        else if (hardDifficulty.isOn)   difficultyChoice = 3;
        else {                          difficultyChoice = 1; }
        PlayerPrefs.SetInt("difficulty", difficultyChoice);
        //Debug.Log("Updated Difficulty");
        GameManager.GMinstance.UpdateSettings();
    }

    public void UpdateVSync()
    {
        isVsync = vSyncToggle.isOn;
        PlayerPrefs.SetInt("isVsync", isVsync ? 1 : 0);
        QualitySettings.vSyncCount = isVsync ? 1 : 0;
    }
    
    public void BackButton()
    {
        this.gameObject.SetActive(false);
        GameManager.GMinstance.UpdateSettings();
    }
    
    public void ClearData()
    {
        GameManager.GMinstance.ClearAllData();
    }

    //Drop down menu for resolutions
    //Sliders for audio levels
    //Split Audio into master, FX, voice and music
    //Difficulty is a toggle between 3 horizontally
    //Keybinds work like Apex Legends
}
