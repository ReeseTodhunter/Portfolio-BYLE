using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{

    private float biteTimer = 0;
    public bool attacked = false;

    private GameObject target;

    public List<GameObject> snakeParts = new List<GameObject>();

    public int biteCount = 0;

    private bool dying = false;
    private float deathTimer = 0;

    public float lifeTime = 15;
    private float lifeTimer = 0;

    public GameObject deathParticle;

    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;
        if(lifeTimer > lifeTime && !dying)
        {
            Die();
        }

        if (!dying)
        {
            if (attacked)
            {
                biteTimer += Time.deltaTime;

                if (biteTimer > 0.5f)
                {
                    if (biteCount > 2)
                    {
                        Die();
                        return;
                    }

                    gameObject.GetComponent<Animator>().Play("SnakeIdle");
                    attacked = false;
                    biteTimer = 0;
                }
            }

            if (target == null)
            {
                gameObject.GetComponent<Animator>().Play("SnakeIdle");
                target = PlayerController.instance.gameObject;
            }
            gameObject.transform.LookAt(target.transform);
            gameObject.transform.eulerAngles = new Vector3(0, gameObject.transform.eulerAngles.y + 180, 0);

            
        }
        else
        {
            deathTimer += Time.deltaTime;

            if(deathTimer > 1.0f)
            {
                Destroy(gameObject);
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            if (other.gameObject.TryGetComponent<BTAgent>(out BTAgent agent) && !attacked && !dying)
            {
                gameObject.GetComponent<Animator>().Play("SnakeAttack");
                agent.Damage(1 * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)));
                agent.Poison(1.0f, 1, 0);

                biteCount += 1;
                attacked = true;
                target = other.gameObject;
            }
        }
    }

    public void setSnake(Color colour)
    {
        foreach (GameObject part in snakeParts)
        {
            if (part.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material.color = colour;
            }
        }
    }

    public void Die()
    {
        GameObject particle = Instantiate(deathParticle);
        particle.transform.position = transform.position;
        dying = true;
        gameObject.GetComponent<Animator>().Play("SnakeDie");
    }

}
