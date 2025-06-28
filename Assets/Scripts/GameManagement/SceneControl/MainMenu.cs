using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject Main;
    [SerializeField]
    private GameObject HowToPlay;
    [SerializeField]
    private GameObject Encyclopedia;
    [SerializeField]
    private GameObject Leaderboard;
    [SerializeField]
    private GameObject Credits;
    [SerializeField]
    private List<TextMeshProUGUI> leaderboard;
    public void OnPlay()
    {
        SceneManager.LoadScene("BUILD");
        GameManager.GMinstance.gameTimer = 0;
        GameManager.GMinstance.currentState = GameManager.gameState.playing;
        GameManager.GMinstance.loadGame = false;
    }
    public void OnCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelectionScreen");
    }
    public void OnContinue()
    {
        //GameManager.GMinstance.LoadSaveData();
        SceneManager.LoadScene("BUILD");
        GameManager.GMinstance.currentState = GameManager.gameState.playing;
        GameManager.GMinstance.loadGame = true;
    }
    public void OnHowToPlay()
    {
        //HowToPlay.SetActive(true);
        //Main.SetActive(false);
        SceneManager.LoadScene("TutorialLevel");
    }
    public void OnLeaderBoard()
    {
        Leaderboard.SetActive(true);
        Main.SetActive(false);

        if (GameManager.GMinstance.scoreList.Count > 0)
        {
            for (int i = 0; i < GameManager.GMinstance.scoreList.Count; i++)
            {
                if (leaderboard.Count > i)
                {
                    int tempNum = i + 1;
                    leaderboard[i].text = (tempNum + " - " + GameManager.GMinstance.scoreList[i].name + ": " + GameManager.GMinstance.scoreList[i].score);
                }
            }
        }
    }
    public void OnCredits()
    {
        Credits.SetActive(true);
        Main.SetActive(false);
    }
    public void BackToMenu()
    {
        Main.SetActive(true);
        HowToPlay.SetActive(false);
        Leaderboard.SetActive(false);
        Credits.SetActive(false);
    }
    public void OnQuit()
    {
        Application.Quit();
    }

    public void onEncyclopedia()
    {
        SceneManager.LoadScene("Encyclopedia");
    }
}
