using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffect : MonoBehaviour
{
    private float timer = 0, duration;
    public virtual void Initialise(Transform _effectPos, float _effectDuration = 1)
    {
        duration = _effectDuration;
    }
    public virtual void Start()
    {
        
    }
    public void Update()
    {
        timer += Time.deltaTime;
        if(timer > duration)
        {
            Destroy(this.gameObject);
        }
    }
}
