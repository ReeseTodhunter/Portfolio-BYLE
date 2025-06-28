using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmiliarPowerUp : MonoBehaviour
{
    private GameObject player;

    public float speed;
    public float fireDelay;
    public GameObject bulletPrefab;

    public Transform projectileSpawn;
    bool recoverposition = false;
    float timer = 0;



    /// //////////////
    public Transform centerObject;  
    public float radius = 2f;    
    public float rotSpeed = 2f;
    private float initialRadius = 0;

    private float angle = 0f;
    /// ///////////

    private void Start()
    {
        player = PlayerController.instance.gameObject;

        timer = Random.Range(0, fireDelay);

        initialRadius = radius;
        radius = radius + Random.Range(-1.75f, 1.75f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        float horizontal = Input.GetAxis("MoveH");
        float vertical = Input.GetAxis("MoveV");


        if (timer > fireDelay && !recoverposition)
        {
            Shoot(1, 1);
            timer = 0;
        }
        
        speed = PlayerController.instance.GetSpeed() - 1;

        gameObject.transform.eulerAngles = PlayerController.instance.gameObject.transform.eulerAngles;




        centerObject = player.transform;

        // Calculate the position of the floating object in a circular orbit
        float x = centerObject.position.x + Mathf.Cos(angle) * radius;
        float z = centerObject.position.z + Mathf.Sin(angle) * radius;

        // Update the position of the floating object
        transform.position = new Vector3(x, transform.position.y, z);

        // Increment the angle based on the speed
        angle += rotSpeed * Time.deltaTime;

    }

    void Shoot(float x, float y)
    {
        gameObject.GetComponent<Animator>().Play("Shoot");
        GameObject projectile = GameObject.Instantiate(bulletPrefab, projectileSpawn.position, projectileSpawn.rotation); //Instantiate projectile

        projectile.transform.eulerAngles = gameObject.transform.eulerAngles;
        Projectile projectileScript = projectile.GetComponent<Projectile>(); //init projectile
        projectileScript.Init(speed + speed / 2, 0, 10, 5 * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject);

    }
}