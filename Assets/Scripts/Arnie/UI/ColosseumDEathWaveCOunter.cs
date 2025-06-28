using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColosseumDEathWaveCOunter : MonoBehaviour
{
    public Text waveCounter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        waveCounter.text = "YOU GOT TO WAVE " + (ColosseumController.instance.overallWaveCounter + ColosseumController.instance.currentWaveCounter);
    }
}
