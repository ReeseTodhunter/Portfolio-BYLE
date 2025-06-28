using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode
{
    /*
        This script is the main data structure for the EQSPathfiner.cs. 
        It essentially copies the EQS nodes and transforms them into a 
        more useful data structure, having propeties like node chaining for 
        pathfinding.
        -Tom
    */
    public Vector2Int pos;
    public bool traversable;
    public float distance;
    public Vector2Int rootNode;
    public Vector2Int chainNode;
    public bool visited;
    public PathfindingNode(Vector2Int _pos, bool _traversable)
    {
        pos = _pos;
        traversable = _traversable;
        distance = float.MaxValue;
        chainNode = pos;
        visited = false;
    }
}
