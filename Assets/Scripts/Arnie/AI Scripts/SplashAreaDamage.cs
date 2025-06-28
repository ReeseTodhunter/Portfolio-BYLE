using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashAreaDamage : MonoBehaviour
{
    public float delay = 2;
    public float damage;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= delay)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.gameObject.TryGetComponent<BTAgent>(out BTAgent agent))
            {
                agent.Damage(damage * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)));
            }
        }
    }
}
