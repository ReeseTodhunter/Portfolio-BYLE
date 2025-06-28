using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float delay = 2;
    public float radius = 3;
    public bool damagesPlayer,damagesEnemies;
    public float playerDamage,enemyDamage;
    public GameObject explosionPrefab;
    private float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= delay)
        {
            Explode();
            if(explosionPrefab != null)
            {
                Instantiate(explosionPrefab,transform.position,Quaternion.identity);
            }
            Destroy(this);
        }
    }
    private void Explode()
    {
        Collider[] cols;
        cols = Physics.OverlapSphere(transform.position,radius);
        foreach(Collider col in cols)
        {
            if(col.gameObject.layer == 7 && damagesPlayer)
            {
                PlayerController.instance.Damage(playerDamage);
            }
            else if(col.gameObject.layer == 10 && damagesEnemies)
            {
                if(col.gameObject.TryGetComponent<BTAgent>(out BTAgent agent))
                {
                    agent.Damage(enemyDamage);  
                }
            }
        }
    }
}
