using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class Grenade : Projectile
{
    private Vector3 initialScale;
    private void OnTriggerEnter(Collider other)
    {


        if(other.GetComponent<PlayerController>() != null)
        {
            PlayerController.instance.Damage(5.0F);
        }
    }




    [SerializeField] private Rigidbody rb;

    [SerializeField] private GameObject explosionEffect;
    private bool _isGhost;

    public float delay = 5f;

    public float explosionForce = 10f;
    public float radius = 10f;

    void Start()
    {
        initialScale = gameObject.transform.localScale;
        rb = GetComponent<Rigidbody>();
        Initialise(transform.forward * 30, false);
        Invoke("Explosion", delay);
    }

    private void Update()
    {
        if (gameObject.transform.position.y < 1)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x ,1, gameObject.transform.position.z);
        }

        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, 3 * initialScale, 0.01f);
    }
    public void Initialise(Vector3 velocity, bool isGhost)
    {
        _isGhost = isGhost;
        rb.AddForce(velocity, ForceMode.Impulse);
    }


    public void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider near in colliders)
        {
            Rigidbody rbNear = near.GetComponent<Rigidbody>();

            if (rbNear != null)
            {
                rbNear.AddExplosionForce(explosionForce, transform.position, radius, 1f, ForceMode.Impulse);

            }
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        foreach (Collider col in colliders)
        {
            if (col.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.Damage(20.0F);
            }
            else if (col.gameObject.TryGetComponent<BTAgent>(out BTAgent agent))
            {
                //agent.Damage(10.0F);
            }
        }
        CameraController.instance.ShakeCameraOverTime(0.5f, 2);
    }
}

