using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class characterSelection : MonoBehaviour
{
    public GameObject ui;
    public PlayerController.SelectedCharacter character = PlayerController.SelectedCharacter.RAMBO;
    public SceneLoader loader;
    public bool selectable = true;
    private void Start()
    {
        ui.SetActive(false);
    }
    private void OnMouseOver()
    {
        ui.SetActive(true);
    }
    private void OnMouseDown()
    {
        if(!selectable){return;}
        GameManager.GMinstance.gameTimer = 0;
        GameManager.GMinstance.currentState = GameManager.gameState.playing;
        loader.OnLoadScene(3);
        GameManager.GMinstance.selectedCharacter = character;
    }
    private void OnMouseExit()
    {
        ui.SetActive(false);
    }
}
