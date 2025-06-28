using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BYLE : MonoBehaviour
{

    public float ByleCounter = 10.0f;

    private Vector3 startingSize;
    private float byleSize = 1.0f;

    private float timeElapsed = 0.0f;
    private float lerpTime = 5.0f;
    private bool Lerp;
    //When a character runs into BYLE get poisoned
    //AI will have their own stuff
    //Special Cases for pickups
    void Start()
    {

    }

    void Update()
    {
        /*if(startingSize == new Vector3(0.0f,0.0f,0.0f))
        {
            startingSize = this.gameObject.transform.localScale;
        }
        if(ByleCounter <= 0.0f)
        {
            Destroy(this.gameObject);
        }*/
        
        /*if (startingSize * byleSize != this.gameObject.transform.localScale)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed < lerpTime)
            {
                this.gameObject.transform.localScale = Vector3.Lerp(this.gameObject.transform.localScale, new Vector3(startingSize.x * byleSize, startingSize.y, startingSize.z * byleSize), timeElapsed / lerpTime);
            }
        }
        else
        {
            timeElapsed = 0.0f;
        }
        if(Lerp) 
        { 
            timeElapsed = 0.0f;
            Lerp = false;
        }*/
    }

        private void OnTriggerStay(Collider entity)
    {
        if (entity.GetComponent<Character>())
        {
            if (entity.GetComponent<PlayerController>()&& !PlayerController.instance.GetComponent<Character>().IsPoisoned())
            {
                entity.GetComponent<Character>().Poison(5.0f, 1.0f, -0.5f);
                //ByleCounter -= 1;
                
               // byleSize -= 0.1f;
                //Lerp = true;
            }
            //Debug.Log("BYLE: Generic");
            //Make the enemies use up byle when they do their moves

        }
    }
}
