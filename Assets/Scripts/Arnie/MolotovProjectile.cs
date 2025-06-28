using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolotovProjectile : Projectile
{

    [SerializeField] private Rigidbody rb;

    [SerializeField] private GameObject explosionEffect;

    public float delay = 5f;

    public float explosionForce = 10f;
    public float radius = 10f;

    List<GameObject> flames = new List<GameObject>();

    private void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody>();
        rb.AddTorque(new Vector3(90, 0, 0));
        Initialise(transform.forward * 30);
    }

    protected override void Update()
    {
        if (gameObject.transform.position.y < 1)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1, gameObject.transform.position.z);
        }

        timer += Time.deltaTime;
        if (timer >= delay)
        {
            DestroyProjectile();
        }

        rb.velocity -= 0.5f * rb.velocity * Time.deltaTime;
    }
    public void Initialise(Vector3 velocity)
    {
        rb.AddForce(velocity, ForceMode.Impulse);
    }

    public override void DestroyProjectile(GameObject projectileDestroyer = null)
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject flame = Instantiate(Resources.Load("Projectiles/FlameProjectile 1") as GameObject, new Vector3(gameObject.transform.position.x + Random.Range(2, -2), gameObject.transform.position.y, gameObject.transform.position.z + Random.Range(2, -2)), Quaternion.identity);
            flame.GetComponent<Projectile>().Init(0.0f, 0.0f, 5, 1.0f, parentObject, false);
            flame.GetComponent<Projectile>().SetBurn(true, 1.0f, 5);
            flames.Add(flame);
        }
        Destroy(gameObject);

        CameraController.instance.ShakeCameraOverTime(0.2f, 2);

        base.DestroyProjectile(projectileDestroyer);
    }
}
