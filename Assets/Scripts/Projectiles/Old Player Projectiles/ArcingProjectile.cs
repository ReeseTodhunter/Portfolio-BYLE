using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcingProjectile : MonoBehaviour
{
    /*
        This script controls arcing projectiles. Probably not going to be a final
        thing but just here for me to test what the AI will look like when using
        them. The projectile lerps to its destination (the player), from its source
        (the agent spawning it). The projectile also arcs, which is a sin wave
        equal to sin(%distance covered * pi). This ensures that there is always one arc.
        -Tom
    */
    public float speed; //Speed of projectile
    private float maxHeight; //Max height the projectile can reach, scales with distance
    private Vector3 startPos,endPos; //Start and end pos of projectile path
    private float duration; //How long the projectile will travel for, useful for lerp stuff
    private float currTime = 0; //Timer for lerp
    
    void Start()
    {
        startPos = transform.position;
        endPos = PlayerController.instance.transform.position;
        speed = 5;
        float distance = Vector3.Distance(startPos,endPos);
        maxHeight = distance * 0.75f;
        duration = distance / speed;
    }
    public void Update()
    {
        currTime += Time.deltaTime;
        if(currTime >= duration){Destroy(this.gameObject);}
        float lerpVal = currTime / duration;
        float height = Mathf.Sin(lerpVal * Mathf.PI);
        Vector3 newPos = Vector3.Lerp(startPos,endPos,lerpVal);
        newPos.y = height * maxHeight;
        transform.position = newPos;
    }
    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
