using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQSPathfinder : MonoBehaviour
{
    /*
        This script utilises the EQS to create a path for the agent to use
        to navigate the game map. Uses a Djikstra algorithm to find the 
        shortest route. 
        -Tom
    */
    private Vector2Int startPos, endPos;
    private Vector3 startWorldPos = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;
    private Vector2Int gridSize;
    private PathfindingNode startNode, endNode;
    private PathfindingNode[,] nodes;
    private List<Vector2Int> EQSPath = new List<Vector2Int>();
    Vector2Int gridCorner;
    private GameObject agent;
    public bool showPath = false;

    private void Update()
    {
        if (this.gameObject.transform.position.y < -3.0f)
            Destroy(this.gameObject);
    }
    public void Start()
    {
        Initialise(this.gameObject);
    }
    public void Initialise(GameObject _agent)
    {
        agent = _agent;
    }
    public List<Vector3> GetPathToPosition(Vector3 endPosition, int pathfindingRadius)
    {
        targetPosition = endPosition;
        Vector2Int endEQSNode = EQS.instance.WorldToEQSCoords(targetPosition);
        Vector2Int bounds = EQS.instance.dimensions;
        if(endEQSNode.x < 0 || endEQSNode.y < 0 || endEQSNode.x > bounds.x || endEQSNode.y > bounds.y)
        {
            return null;
        }

        if(!EQS.instance.GetNodes()[endEQSNode.x,endEQSNode.y].GetTraversable())
        {
            return null;
        }
        startPos = EQS.instance.WorldToEQSCoords(agent.transform.position);
        endPos = endEQSNode;

        if(!AttemptGenerateNodeGrid(pathfindingRadius,endPos))
        {
            return null;
        }
        EQSPath = DijkstraCompute();
        if(EQSPath == null)
        {
            return null;
        }
        Vector2Int start = EQS.instance.WorldToEQSCoords(transform.position);
        List<Vector3> path = new List<Vector3>();
        foreach(Vector2Int localNode in EQSPath)
        {
            Vector2Int currNode = start + localNode;
            Vector3 worldPos = EQS.instance.EQSCoordsToWorld(currNode);
            path.Add(worldPos);
        }
        return path;
    }
    private bool AttemptGenerateNodeGrid(int radius, Vector2Int end)
    {
        int length = 1 + 2 * radius;
        nodes = new PathfindingNode[length, length];
    
        gridSize = new Vector2Int(length, length);
        gridCorner = startPos - new Vector2Int(radius,radius);
        Vector2Int endCoord = end - gridCorner;
        if(endCoord.x < 0 || endCoord.y < 0 || endCoord.x > length || endCoord.y > length){return false;}
        Vector2Int bounds = EQS.instance.dimensions;
        
        //Generate grid, starting from corner
        for(int i = 0; i < length; i++){
            for(int j = 0; j < length; j++)
            {
                Vector2Int pos = new Vector2Int(i,j);
                bool traversable;
                Vector2Int worldPos = pos + gridCorner;
                if(worldPos.x < 0 || worldPos.x >= bounds.x || worldPos.y < 0 || worldPos.y >= bounds.y)
                {
                    traversable = false;
                }
                else
                {
                    traversable = EQS.instance.GetNodes()[worldPos.x,worldPos.y].GetTraversable();
                }
                PathfindingNode newNode = new PathfindingNode(pos, traversable);
                nodes[i,j] = newNode;
            }
        }
        startNode = nodes[radius,radius];
        endNode = nodes[endCoord.x,endCoord.y]; 
        return true;
    }
    private List<Vector2Int> DijkstraCompute()
    {
        EQSPath = new List<Vector2Int>();
        PathfindingNode currentNode = startNode;
        currentNode.distance = 0;
        List<PathfindingNode> nodesToCompute = new List<PathfindingNode>();
        nodesToCompute.AddRange(GetAdjacentNodes(currentNode.pos));
        List<PathfindingNode> nodesToPrune = new List<PathfindingNode>();
        foreach(PathfindingNode node in nodesToCompute)
        {
            if(node.traversable){continue;}
            nodesToPrune.Add(node);
        }
        foreach(PathfindingNode node in nodesToPrune)
        {
            nodesToCompute.Remove(node);
        }
        foreach(PathfindingNode node in nodesToCompute)
        {
            node.rootNode = currentNode.pos;
        }
        int x = 0;
        while(nodesToCompute.Count > 0)
        {
            float distance;
            List<PathfindingNode> nextNodesToCompute = new List<PathfindingNode>();
            foreach(PathfindingNode adjacentNode in nodesToCompute)
            {
                x++;
                currentNode = nodes[adjacentNode.rootNode.x, adjacentNode.rootNode.y];
                distance = Vector2Int.Distance(adjacentNode.pos, currentNode.pos);
                if(distance + currentNode.distance < adjacentNode.distance)
                {
                    adjacentNode.distance = distance + currentNode.distance;
                    adjacentNode.visited = true;
                    adjacentNode.chainNode = currentNode.pos;
                    List<PathfindingNode> adjacentNodes = GetAdjacentNodes(adjacentNode.pos);
                    foreach(PathfindingNode node in adjacentNodes)
                    {
                        if(!node.visited && !nextNodesToCompute.Contains(node) && node.traversable)
                        {
                            node.rootNode = adjacentNode.pos;
                            nextNodesToCompute.Add(node);
                        }
                    }
                }
            }
            nodesToCompute.Clear();
            nodesToCompute.AddRange(nextNodesToCompute);
        }
        if(!endNode.visited)
        {
            return null;
        }
        List<Vector2Int> reversePath = new List<Vector2Int>();
        Vector2Int currStep = endNode.pos;
        x = 0;
        while(currStep != startNode.pos && x < 99)
        {
            x++;
            reversePath.Add(currStep);
            currStep = nodes[currStep.x, currStep.y].chainNode;
        }
        reversePath.Add(startNode.pos);
        EQSPath = new List<Vector2Int>();

        for(int i = reversePath.Count - 1; i >= 0; i--)
        {
            EQSPath.Add(reversePath[i] - startNode.pos);
        }
        startWorldPos = new Vector3(Mathf.RoundToInt(transform.position.x),transform.position.y, Mathf.RoundToInt(transform.position.z));
        return EQSPath;
    }
    private List<PathfindingNode> GetAdjacentNodes(Vector2Int nodePos)
    {
        List<PathfindingNode> adjacentNodes = new List<PathfindingNode>();
        for(int i = -1; i <= 1; i++){
            for(int j = -1; j <= 1; j++)
            {
                Vector2Int displacement = new Vector2Int(i,j);
                int magnitude = Mathf.Abs(displacement.x) + Mathf.Abs(displacement.y);
                if(magnitude == 0 || magnitude == 2) //Cull diagonals
                {
                    continue;
                }
                Vector2Int coords = nodePos + displacement;
                if(coords.x >= 0 && coords.x < gridSize.x &&
                   coords.y >= 0 && coords.y < gridSize.y)
                {
                    adjacentNodes.Add(nodes[coords.x,coords.y]);
                }
            }
        }
        return adjacentNodes;
    }
}
