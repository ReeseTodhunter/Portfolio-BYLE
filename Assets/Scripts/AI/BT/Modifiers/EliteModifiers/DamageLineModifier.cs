using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLineModifier : BTModifier
{
    private LineRenderer line;
    private float damagePerSecond = 25;
    private GameObject tetherPrefab, tetherObject;
    private GameObject tetherPartner;

    //Gizmo stuff
    private Vector3 gizmoStartPos;
    private Vector3 gizmoEndPos;
    //
    public override void Initialise(Character _agent)
    {
        title = "Byle-tether";
        type = modifierType.onUpdate;
        tetherPrefab = Resources.Load("Elite/EliteTether") as GameObject;
        tetherObject = GameObject.Instantiate(tetherPrefab,transform.position,Quaternion.identity);
        tetherObject.transform.parent = _agent.transform;
        tetherObject.transform.localPosition = Vector3.up;
        line = tetherObject.GetComponent<LineRenderer>();
    }

    public override void ActivateModifier(Character _agent)
    {
        if(tetherPartner == null || tetherPartner.GetComponent<BTAgent>().GetHealth() <= 0) //If no partner, get new one
        {
            tetherPartner = null;
            tetherPartner = GetTetherPartner();
            if(tetherPartner == null){tetherObject.SetActive(false); return;} //return if no valid partner
            tetherObject.SetActive(true);
        }
        //Get tether points
        Vector3 startTetherPoint = transform.position;
        Vector3 endTetherPoint = tetherPartner.transform.position;
        startTetherPoint.y = 1;
        endTetherPoint.y = 1;
        line.SetPosition(0,startTetherPoint);
        line.SetPosition(1,endTetherPoint);
        if(!CanHitPlayer(startTetherPoint,endTetherPoint)){return;}
        PlayerController.instance.Damage(damagePerSecond * Time.deltaTime);
    }
    private bool CanHitPlayer(Vector3 _startPos, Vector3 _endPos)
    {
        Vector3 direction = _endPos - _startPos;
        direction.y = 0;
        direction.Normalize();
        float distance = Vector3.Distance(transform.position,tetherPartner.transform.position);
        RaycastHit hit;
        gizmoStartPos = _startPos;
        gizmoEndPos = _endPos;
        LayerMask mask = LayerMask.GetMask("Player");
        if(Physics.Raycast(_startPos,direction,out hit,distance,mask,QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;    
    }
    private GameObject GetTetherPartner()
    {
        List<BTAgent> nearbyAgents = EnemySpawningSystem.instance.GetEnemies();
        float maxDistance = 15;
        float minDistance = 4;
        foreach(BTAgent nearbyAgent in nearbyAgents)
        {
            float distance = Vector3.Distance(this.transform.position,nearbyAgent.transform.position);
            if(distance > maxDistance || distance < minDistance){continue;}
            return nearbyAgent.gameObject;
        }
        return null;
    }
}
