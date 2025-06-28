using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heads : MonoBehaviour
{
    [SerializeField]
    private float timeUntilShow = 100.0f;
    [SerializeField]
    private float timeToMove = 5.0f;

    [SerializeField]
    private Transform startPos;
    [SerializeField]
    private Transform endPos;

    private float timer = 0.0f;
    private float elapsedTime = 0.0f;

    void Start()
    {
        timer = 0.0f;
        elapsedTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < timeUntilShow) timer += Time.deltaTime;

        if (timer >= timeUntilShow)
        {
            if (elapsedTime < timeToMove)
            {
                elapsedTime += Time.deltaTime;
                float percentComplete = elapsedTime / timeToMove;
                transform.position = Vector3.Lerp(startPos.position, endPos.position, percentComplete);
            }
        }
    }
}
