using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQSNode
{
    /*
        Node class for the EQS, contains all relevant info
        required by an EQS.
        -Tom
    */
    private Vector3 worldPos;
    private Vector2Int coords; 
    private float distanceToPlayer = 0;
    private bool traversable = false;
    private bool lineOfSight = false;
    private bool bileLineOfSight = false;
    private bool bile = false;
    public EQSNode(Vector2Int _coords, Vector3 _worldPos)
    {
        worldPos = _worldPos;
        coords = _coords;
    }
    #region Getters 
    public Vector3 GetWorldPos(){return worldPos;}
    public float GetDistance(){return distanceToPlayer;}
    public bool GetTraversable(){return traversable;}
    public bool GetLineOfSight(){return lineOfSight;}
    public bool GetBileLineOfSight(){return bileLineOfSight;}
    public bool GetHasBile(){return bile;}

    #endregion
    #region Setters
    public void SetDistance(float _distance){distanceToPlayer = _distance;}
    public void SetTraversable(bool _traversable){traversable = _traversable;}
    public void SetLineOfSight(bool hasLineOfSight){lineOfSight = hasLineOfSight;}
    public void SetBileLineOfSight(bool hasSight){bileLineOfSight = hasSight;}
    public void SetBile(bool _hasBile){bile = _hasBile;}
    #endregion
}
