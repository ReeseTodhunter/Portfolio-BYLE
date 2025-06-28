using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaProjectile : Projectile
{
    public ParticleSystem electric;
    public ParticleSystem electric2;

    public void Start()
    {
        electric.Play();
        electric2.Play();
    }

}

