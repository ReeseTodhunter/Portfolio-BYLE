using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public BTAgent[] agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = gameObject.GetComponentsInChildren<BTAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAgentsArray();
    }

    public void UpdateAgentsArray()
    {
        agents = gameObject.GetComponentsInChildren<BTAgent>();
    }
}
