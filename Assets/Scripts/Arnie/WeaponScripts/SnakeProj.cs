using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeProj : Projectile
{
    private float turnTime = 0.5f; // Turn time. Magic numbers are cringe but it's best this stays the same
    private float turnVelocity; // I don't know what this does I'll be honest I didn't make the original boomerang code

    public bool flip;
    private int flipInt = 1;

    private Vector3 pos;
    private Vector3 axis;
    private Vector3 targetPos;

    public Transform rightPos;
    public Transform leftPos;
    public Transform straightPos;

    int shot = 0;

    public GameObject head;

    public List<Color> colours = new List<Color>();
    public List<GameObject> snakeParts = new List<GameObject>();

    private Color tempColour;

    public GameObject snakePrefab;

    private void Start()
    {
        spawnPos = transform.position;
        targetPos = straightPos.position;

        tempColour = colours[Random.Range(0, colours.Count - 1)];
        foreach (GameObject part in snakeParts)
        {
            if (part.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material.color = tempColour;
            }
        }
            
    }

    protected override void ProjectileUpdate()
    {

        head.transform.LookAt(straightPos.transform);
        head.transform.eulerAngles = new Vector3(0, head.transform.eulerAngles.y + 180, 0);


        if (Vector3.Distance(spawnPos, transform.position) > 2f && shot < 2)
        {
            spawnPos = transform.position;
            flipInt = -flipInt;

            if (flipInt > 0)
            {
                targetPos = leftPos.position;
            }
            else
            {
                targetPos = rightPos.position;
            }

            
            shot += 1;
        }

        if (Vector3.Distance(spawnPos, transform.position) > 3.5f && shot >= 2)
        {
            spawnPos = transform.position;
            flipInt = -flipInt;

            if(flipInt > 0)
            {
                targetPos = leftPos.position;
            }
            else
            {
                targetPos = rightPos.position;
            }
            
            
        }

        if (parentObject != null)
        {

            Vector2 temp = new Vector2(targetPos.x - transform.position.x, targetPos.z - transform.position.z);

            float targetAngle = Mathf.Atan2(temp.x, temp.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


        }
        
    }

    public void spawnSnake()
    {
        GameObject snake = Instantiate(snakePrefab);
        snake.transform.position = transform.position;

        snake.GetComponent<SnakeController>().setSnake(tempColour);

        base.DestroyProjectile();
    }

}