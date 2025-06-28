using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomColour : MonoBehaviour
{
    public List<GameObject> BandanaParts = new List<GameObject>();

    void Start()
    {
        Color background = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        foreach(GameObject part in BandanaParts)
        {
            if(part.TryGetComponent<Image>(out Image image))
            {
                image.color = background;
            }
            else if(part.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material.color = background;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
