using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunFXController : MonoBehaviour
{
    public ParticleSystem pSystem;
    public void PlayAnimation()
    {
        pSystem.Play();
    }
}
