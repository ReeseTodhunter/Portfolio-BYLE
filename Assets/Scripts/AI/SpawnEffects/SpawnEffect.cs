using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    //Base spawn effect used by the revenant spawn and base spawn
    // - Tom
    protected BTAgent agent;
    public virtual void Initialise(BTAgent parentAgent)
    {
        agent = parentAgent;
    }
}
