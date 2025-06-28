using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chick : Character
{
    public int chickNum;
    public float orbitRadius = 2.5f;
    public Vector3 TargetPos;
    [Range(0.0f, 1.0f)]
    public float orbitSpeed = 0.2f;

    private bool orbiting;
    public float chickTimer;

    private void Start()
    {
        orbiting = false;
        gameObject.GetComponent<Collider>().enabled = false;
    }
    // Update is called once per frame
    void Update()
    {


        //Update the chick's orbit timer
        chickTimer += Time.deltaTime * speed * 0.1f;
        if (chickTimer >= 360)
        {
            chickTimer -= 360;
        }

        //Get's the position orbiting the player where the chick should go
        TargetPos = OrbitPoint(PlayerController.instance.transform.position, orbitRadius);

        //If the chick is not within orbiting range move the chick towards it's orbit
        if ((Vector3.Distance(transform.position, TargetPos) > 0.1f) && !orbiting)
        {
            transform.LookAt(TargetPos);
            transform.position += transform.forward * Time.deltaTime * speed;
        }
        //Once the chick is within orbiting range set their orbiting bool to true
        else if(!orbiting)
        {
            orbiting = true;
            gameObject.GetComponent<Collider>().enabled = true;
        }
        //Run the chick's fake orbit, updating the chick's position and rotation in relation to the player
        else
        {
            transform.position = TargetPos;
            transform.LookAt(PlayerController.instance.transform.position);
            transform.eulerAngles += new Vector3(0, -90, 0);
        }
    }

    protected override void Die()
    {
        SpecialsManager.instance.chicks.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

    //Fake an orbit around the player
    Vector3 OrbitPoint(Vector3 centrePoint, float radius)
    {
        float x = -Mathf.Cos(chickTimer) * radius;
        float z = Mathf.Sin(chickTimer) * radius;
        Vector3 pos = new Vector3(x, 0, z);
        return pos + centrePoint;
    }
}
