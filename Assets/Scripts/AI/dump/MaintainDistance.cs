using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainDistance : Action
{
    // GOP STUFF, NOT USED // Tom
    float minDistance, maxDistance;
    public void Initialise(float _minDistance, float _maxDistance)
    {
        minDistance = _minDistance;
        maxDistance = _maxDistance;
    }
}
