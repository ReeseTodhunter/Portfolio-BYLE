using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParticleEffect : WeaponEffect
{
    public ParticleSystem particleEffect;
    void Awake()
    {
        particleEffect.gameObject.SetActive(false);
    }
    public override void Initialise(Transform _effectPos, float _effectDuration = 1)
    {
        transform.forward = _effectPos.transform.forward;
        base.Initialise(_effectPos,_effectDuration);
    }
    public override void Start()
    {
        particleEffect.gameObject.SetActive(true);
        particleEffect.Play();
    }
}
