using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CricketController : MonoBehaviour
{

    public GameObject textprefab;
    float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 20)
        {
            gameObject.GetComponent<AudioSource>().Play();
            GameObject text = Instantiate(textprefab,PlayerController.instance.transform);
            text.transform.position = gameObject.transform.position;
            text.GetComponent<CricketText>().SetText("cricket");
            timer = 0;
        }
    }
}
