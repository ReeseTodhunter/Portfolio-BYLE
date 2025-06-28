using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQS : MonoBehaviour
{
    //Environmental query system, this helps the ai find the player, pathfind etc
    // -Tom

    
    //Singelton
    public static EQS instance;
    public Vector3 corner = Vector3.one;
    public Vector2Int dimensions = Vector2Int.one;
    public float nodeRadii = 1;
    public float gizmoMultiplier = 0.05f;
    [Range(0.2f,1f)]
    public float refreshRate;
    private float refreshRateTimer = 0;
    private EQSNode[,] nodes;
    bool isActive = false;
    public enum DebugLayer{traverable, distanceToPlayer, LineOfSight, bileLineOfSight, bile, combined, none};
    public DebugLayer currentDebugLayer = DebugLayer.none;
    public bool spawnOnPlay = false;
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
        if (spawnOnPlay)
        {
            Initialise(corner, dimensions);
        }
    }
    public void Initialise(Vector3 _corner, Vector2Int _roomDimensions)
    {
        corner = _corner;
        _roomDimensions.x = Mathf.FloorToInt(_roomDimensions.x / nodeRadii);
        _roomDimensions.y = Mathf.FloorToInt(_roomDimensions.y / nodeRadii);
        dimensions = _roomDimensions;
        nodes = null;
        nodes = new EQSNode[dimensions.x,dimensions.y];
        int x = 0;
        for(int i = 0; i < dimensions.x; i++)
        for(int j = 0; j < dimensions.y; j++)
        {
            x++;
            Vector3 worldPos = corner + new Vector3(i, 0 , j ) * nodeRadii;
            Vector2Int coords = new Vector2Int(i,j);
            EQSNode newNode = new EQSNode(coords,worldPos);
            float distance = 0;
            newNode.SetDistance(distance);
            nodes[i,j] = newNode;
        }
        isActive = true;
        CullEQSNodes();
        if(PlayerController.instance != null)
        {
            UpdateLinesOfSight();
            UpdateDistanceToPlayer();
        }
    }
    private void CullEQSNodes()
    {
        //Culls any nodes that are on top of obstacles
        foreach(EQSNode node in nodes)
        {
            Collider[] collisions = Physics.OverlapSphere(node.GetWorldPos(),nodeRadii);
            foreach(Collider col in collisions)
            {
                if(col.gameObject.layer == 8)
                {
                    node.SetTraversable(false);
                    break;
                }
                else if(col.gameObject.layer == 6)
                {
                    node.SetTraversable(true);
                }
            }
        }
    }
    public void Update()
    {
        if(PlayerController.instance == null || !isActive){return;}
        refreshRateTimer += Time.deltaTime;
        if (refreshRateTimer >= refreshRate && nodes != null)
        {
            refreshRateTimer = 0;
            UpdateDistanceToPlayer();
            UpdateLinesOfSight();
        }
    }
    private void UpdateLinesOfSight()
    {
        RaycastHit hit;
        float distance;
        Vector3 origin, direction;
        foreach(EQSNode node in nodes)
        {
            if(!node.GetTraversable())
            {
                node.SetLineOfSight(false);
                continue;
            }
            origin = node.GetWorldPos();
            direction = PlayerController.instance.transform.position - origin;
            direction.Normalize();
            distance = Vector3.Distance(origin, PlayerController.instance.transform.position);
            if(Physics.SphereCast(origin, .5f, direction, out hit, distance, 1<<8))
            {
                if(hit.collider.gameObject.layer == 8)
                {
                    node.SetLineOfSight(false);
                    continue;
                }
            }
            node.SetLineOfSight(true);
        }
    }
    private void UpdateDistanceToPlayer()
    {
        Vector3 playerPosition = PlayerController.instance.transform.position;
        float distance;
        foreach(EQSNode node in nodes)
        {
            if(!node.GetTraversable())
            {
                node.SetDistance(999);
                continue;
            }
            distance = Vector3.Distance(node.GetWorldPos(), playerPosition);
            node.SetDistance(distance);
        }
    }
    public EQSNode[,] GetNodes(){return nodes;}
    public Vector2Int getDimensions(){return dimensions;}
    public EQSNode GetNodeFromGrid(Vector2Int coord){return nodes[coord.x,coord.y];}
    public EQSNode GetNearestNode(Vector3 worldPosition)
    {
        Vector2Int coords = WorldToEQSCoords(worldPosition);
        if(coords.x < 0 || coords.y < 0 || coords.x > dimensions.x || coords.y > dimensions.y){return null;}
        EQSNode node = nodes[coords.x,coords.y];
        return node;
    }
    public Vector2Int WorldToEQSCoords(Vector3 worldPos)
    {
        worldPos.y = corner.y;
        Vector3 offset = worldPos - corner;
        offset /= nodeRadii;
        Vector2Int coords = new Vector2Int
        (
            Mathf.RoundToInt(offset.x),
            Mathf.RoundToInt(offset.z)
        );
        return coords;
    }
    public Vector3 EQSCoordsToWorld(Vector2Int EQSCoords)
    {
        return nodes[EQSCoords.x,EQSCoords.y].GetWorldPos();
    }
    void OnDrawGizmos()
    {
        if(nodes == null){return;}
        foreach(EQSNode node in nodes)
        {
            bool skipNode = false;
            switch(currentDebugLayer)
            {
                case DebugLayer.none:
                    continue;
                case DebugLayer.traverable:
                    if(!node.GetTraversable()){skipNode = true;}
                    else{Gizmos.color = Color.white;}
                    break;
                case DebugLayer.distanceToPlayer:
                    float val = node.GetDistance() * gizmoMultiplier;
                    val = val < 0 ? 0 : val;
                    val = val > 1 ? 1 : val;
                    Gizmos.color = Color.Lerp(Color.green, Color.red, val);
                    break;
                case DebugLayer.LineOfSight:
                    if(node.GetLineOfSight()){Gizmos.color = Color.green;}
                    else{skipNode = true;}
                    break;
                case DebugLayer.combined:
                    if(!node.GetTraversable() || !node.GetLineOfSight())
                    {
                        Gizmos.color = Color.red;
                        break;
                    }
                    else
                    {
                        val = node.GetDistance() * gizmoMultiplier;
                        val = val < 0 ? 0 : val;
                        val = val > 1 ? 1 : val;
                        Gizmos.color = Color.Lerp(Color.green, Color.red, val);
                        break;
                    }
                case DebugLayer.bile:
                    if(node.GetHasBile())
                    {
                        Gizmos.color = Color.green;
                    }
                    else{skipNode = true;}
                    break;
                case DebugLayer.bileLineOfSight:
                    if(node.GetBileLineOfSight())
                    {
                        Gizmos.color = Color.green;
                    }
                    else{skipNode = true;}
                    break;
            }
            if(skipNode){continue;}
            Gizmos.DrawWireCube(node.GetWorldPos(), Vector3.one * nodeRadii * 0.5f);
        }  
    }
}
