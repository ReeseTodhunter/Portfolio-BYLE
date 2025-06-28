using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeTest : MonoBehaviour
{
    public int count = 1;
    public float distance = 1;
    public bool showGizmos = false;
    public Vector3 sourceVector = Vector3.one;
    void OnDrawGizmos()
    {
        if(!showGizmos) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        float angleOffset = 360 / count;
        for(int i =0; i < count; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(angleOffset * i,Vector3.up) * sourceVector;
            direction.y = 0;
            //Gizmos.DrawLine(transform.position, transform.position + direction * distance);
            Gizmos.DrawWireCube(transform.position + direction * distance, Vector3.one * 0.25f);
        }
    }
}
