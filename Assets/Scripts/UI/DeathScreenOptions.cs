using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenOptions : MonoBehaviour
{
    //Text box references for passing the Name of the player into the Leaderboard
    public Text textBox;
    public string nameInput;

    private void Update()
    {
        if(textBox != null)
        {
            nameInput = textBox.text;
        }
        
    }
    //Returns to main menu when corresponding button is pressed
    public void ReturnToMainMenu()
    {
        GameManager.GMinstance.ReturnToMainMenu();
        GameManager.GMinstance.UpdateScore(nameInput);
    }
    //Restarts the game from level 1 using the same character, when corresponding button is pressed
    public void Restart()
    {
        GameManager.GMinstance.Restart();
        GameManager.GMinstance.UpdateScore(nameInput);
    }
}
