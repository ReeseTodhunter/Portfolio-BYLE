using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CerebralExplosion : MonoBehaviour
{
    public Color StartColor;
    public Color EndColor;

    private float timer = 0;

    public float explosionForce = 10f;
    public float radius = 10f;
    public GameObject explosionEffect;

    public GameObject[] SkinObjects;

    public List<Material> SkinMaterials;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Animator>().Play("BrainExplode");

        foreach (GameObject skin in SkinObjects)
        {
            SkinMaterials.Add(skin.GetComponent<Renderer>().material);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 3.95f)
        {
            Explosion();
        }
        if(timer < 4)
        {
            float t = timer / 4;

            Color currentColor = Color.Lerp(StartColor, EndColor, t);
            
            foreach(Material skin in SkinMaterials)
            {
                skin.color = currentColor;
            }
        }
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
                player.Damage(30.0F);
            }
            else if (col.gameObject.TryGetComponent<BTAgent>(out BTAgent agent))
            {
                //agent.Damage(10.0F);
            }
        }
        CameraController.instance.ShakeCameraOverTime(0.5f, 2);
    }
}
