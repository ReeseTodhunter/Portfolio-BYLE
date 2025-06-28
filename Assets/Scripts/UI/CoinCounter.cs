using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoinCounter : MonoBehaviour
{
    public static CoinCounter instance;
    public Text text;
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
        text.text = "COINS: 0";
    }

    public void UpdateUI(int score)
    {
        text.text = "COINS: " + score;
    }
}
