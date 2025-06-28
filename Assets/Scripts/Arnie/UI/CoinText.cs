using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinText : MonoBehaviour
{
    public Text coinText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = "COINS: " + PlayerController.instance.GetCoinValue();
    }
}
