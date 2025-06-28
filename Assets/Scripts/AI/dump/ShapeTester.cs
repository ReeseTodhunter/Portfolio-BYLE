using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeTester : MonoBehaviour
{
    public List<Vector3> vertices = new List<Vector3>();
    public int pointCount = 10;
    void OnDrawGizmos()
    {
        // foreach(Vector3 vertex in vertices)
        // {
        //     Gizmos.DrawWireCube(transform.position + vertex, Vector3.one * 0.25f);
        // }
        float percentage = 0;
        float vertexPercentage = 0;
        int prevIndex, currIndex;
        float interVertextPercentage = 0;
        for(float i = 0; i < pointCount; i++)
        {
            //Percentage of the current point
            percentage = i / pointCount;
            //Get the percentage along the vertices list
            vertexPercentage = percentage * vertices.Count;
            //Get the previous vertex
            prevIndex = Mathf.FloorToInt(vertexPercentage);
            //Get the current vertex
            currIndex= Mathf.CeilToInt(vertexPercentage);
            if(currIndex >= vertices.Count){currIndex = 0;}
            interVertextPercentage = vertexPercentage - prevIndex;
            Vector3 pos = Vector3.Lerp(vertices[prevIndex],vertices[currIndex], interVertextPercentage);
            Gizmos.DrawWireCube(transform.position + pos,Vector3.one * 0.25f);
        }
    }
}
