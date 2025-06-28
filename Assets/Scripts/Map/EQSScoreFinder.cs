using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQSScoreFinder : MonoBehaviour
{
    /*
        Debugger / Visualiser / educator / toy / showing off script that helps
        visualise how the EQS works.
        -Tom
    */
    public float optimalDistance = 2f;
    public bool showGizmos = true;
    public bool needsLineOfSight = true;
    private List<Vector3> validPositions = new List<Vector3>();
    public float minScore = 2;
    public float distanceForgiveness = 1;
    public bool dummyAI = false;
    public Vector3 dummyPos = Vector3.one;
    private Vector3 bestPos = Vector3.one;
    public void Update()
    {
        validPositions.Clear();
        EQSNode[,] nodes = EQS.instance.GetNodes();
        float score, proximityToDistance;
        float closest = float.MaxValue, distance = 0;
        foreach(EQSNode node in nodes)
        {
            score = 0;
            score += node.GetLineOfSight() == needsLineOfSight ? 1 : 0;
            proximityToDistance = Mathf.Abs(node.GetDistance() - optimalDistance);
            score += proximityToDistance < distanceForgiveness ? 1 : 0;
            
            if(score < minScore){continue;}
            validPositions.Add(node.GetWorldPos());

            if(!dummyAI){continue;}
            distance = Vector3.Distance(dummyPos, node.GetWorldPos());
            
            if(distance > closest){continue;}
            closest = distance;
            bestPos = node.GetWorldPos();
        
        }
    }
    public void OnDrawGizmos()
    {
        if(validPositions == null || !showGizmos){return;}
        Gizmos.color = Color.green;
        foreach(Vector3 pos in validPositions)
        {
            if(dummyAI)
            {
                if(pos == bestPos)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }
            }
            else
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawWireCube(pos, Vector3.one * 0.5f);
        }
        if(dummyAI)
        {
            Vector3 size = new Vector3(1,2,1);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(dummyPos, size);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(dummyPos, bestPos);
        }
    }
}
