using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CareerStats : MonoBehaviour
{
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI coinsCountText;
    public TextMeshProUGUI roomsClearedText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        killCountText.text = "ENEMIES KILLED: " + GameManager.GMinstance.enemiesKilled;
        coinsCountText.text = "COINS COLLECTED: " + GameManager.GMinstance.coinsCollected;
        roomsClearedText.text = "ROOMS ENTERED: " + GameManager.GMinstance.roomsEntered;
    }
}
