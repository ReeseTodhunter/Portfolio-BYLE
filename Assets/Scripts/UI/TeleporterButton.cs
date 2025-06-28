using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleporterButton : MonoBehaviour
{
    public Transform teleportPos;
    public GameObject ui;
    public Button Icon;
    Navigation navigation = new Navigation();
    

    public bool isTeleportRoom;
    public void TeleportPlayer()
    {
	Debug.Log("Try Teleport");
        if (EnemySpawningSystem.instance.isRoomCleared() == false)
            return;
        else
        {
            Vector3 pos = teleportPos.position;
            pos.y = 1;
            PlayerController.instance.transform.position = pos;
        }
    }
    private void Start()
    {

        Icon = ui.GetComponent<Button>();
        navigation.mode = Navigation.Mode.None;
        Icon.navigation = navigation;

        if (!isTeleportRoom)
        {
            float randNum =  Random.Range(0, 4);
            if (randNum == 0) isTeleportRoom = true;
            else isTeleportRoom = false;
        }
    }
    private void Update()
    {
        if (EnemySpawningSystem.instance.isRoomCleared() == false)
            return;
        if (!isTeleportRoom)
        {
            ui.SetActive(false);
        }
        else
        {
            if (EnemySpawningSystem.instance.isRoomCleared())
            {
                ui.SetActive(true);
            }
        }  
        
    }
}
