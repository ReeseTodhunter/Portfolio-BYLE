using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffectProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerController.instance.GetComponent<Character>().IsPoisoned())
        {
            gameObject.GetComponent<Projectile>().SetPoison(true, 1, -0.5f, 5);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null && !PlayerController.instance.GetComponent<Character>().IsPoisoned())
        {
            other.GetComponent<Character>().Poison(5.0f, 1.0f, -0.5f);
        }
    }
}
