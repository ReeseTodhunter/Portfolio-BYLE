using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuUI, charSelectUI, leaderboardUI, creditsUI, settingsUI, rebindUI;
    public CharacterSelectCameraController camController;
    public List<TextMeshProUGUI> leaderboardPlaces = new List<TextMeshProUGUI>();
    public SceneLoader sceneLoader;
    private enum MenuState
    {
        MAIN_MENU,
        CHARACTER_SELECT,
        LEADERBOARD,
        CREDITS,
        SETTINGS,
        REBIND
    }
    private MenuState currState = MenuState.MAIN_MENU;

    void Start()
    {
        GoToMainMenu();
    }
    
    public void LoadScene(int _sceneID){sceneLoader.OnLoadScene(_sceneID);}
    private void UpdateUI()
    {
        HideAllUI();
        switch(currState)
        {
            case MenuState.MAIN_MENU:
                mainMenuUI.SetActive(true);
                break;
            case MenuState.CHARACTER_SELECT:
                charSelectUI.SetActive(true);
                break;
            case MenuState.LEADERBOARD:
                UpdateLeaderboard();
                leaderboardUI.SetActive(true);
                break;
            case MenuState.CREDITS:
                creditsUI.SetActive(true);
                break;
            case MenuState.SETTINGS:
                settingsUI.SetActive(true);
                break;
            case MenuState.REBIND:
                rebindUI.SetActive(true);
                break;
        }
    }
    public void GoToMainMenu()
    {
        if(currState == MenuState.LEADERBOARD || currState == MenuState.MAIN_MENU || currState == MenuState.CREDITS || currState == MenuState.SETTINGS || currState == MenuState.REBIND)
        {
            camController.SkipToMenu();
        }
        else
        {
            camController.OnMainMenu();
        }
        currState = MenuState.MAIN_MENU;
        UpdateUI();
    }
    public void GoToLeaderBoard()
    {
        currState = MenuState.LEADERBOARD;
        UpdateUI();
    }

    public void GoToCredits()
    {
        currState = MenuState.CREDITS;
        UpdateUI();
    }

    public void GoToCharacterSelect()
    {
        currState = MenuState.CHARACTER_SELECT;
        camController.OnCharacterSelect();
        GameManager.GMinstance.loadGame = false;
        SaveLoadSystem.instance.ResetSave();
        UpdateUI();
    }
    public void ContinueToCharacterSelect()
    {
        if (SaveLoadSystem.instance.LoadGame() != null)
        {
            sceneLoader.OnLoadScene(3);
            GameManager.GMinstance.currentState = GameManager.gameState.playing;
            GameManager.GMinstance.loadGame = true;
        }
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void Settings()
    {
        currState = MenuState.SETTINGS;
        UpdateUI();
    }
    public void Rebind()
    {
        currState = MenuState.REBIND;
        UpdateUI();
    }

    private void HideAllUI()
    {
        mainMenuUI.SetActive(false);
        charSelectUI.SetActive(false);
        leaderboardUI.SetActive(false);
        creditsUI.SetActive(false);
        settingsUI.SetActive(false);
        rebindUI.SetActive(false);
    }

    private void UpdateLeaderboard()
    {
        if (GameManager.GMinstance.scoreList.Count > 0)
        {
            for (int i = 0; i < GameManager.GMinstance.scoreList.Count; i++)
            {
                if (leaderboardPlaces.Count > i)
                {
                    int tempNum = i + 1;
                    leaderboardPlaces[i].text = (tempNum + " - " + GameManager.GMinstance.scoreList[i].name + ": " + GameManager.GMinstance.scoreList[i].score);
                }
            }
        }
    }
}
