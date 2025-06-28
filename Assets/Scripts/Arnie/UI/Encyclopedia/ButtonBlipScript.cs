using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBlipScript : MonoBehaviour
{
    public static ButtonBlipScript instance;

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

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBlip()
    {
        gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.1f);
        gameObject.GetComponent<AudioSource>().volume = GameManager.GMinstance.FXVolume;
        gameObject.GetComponent<AudioSource>().Play();
    }
}
