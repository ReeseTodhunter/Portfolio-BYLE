using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLineObject : MonoBehaviour
{
    public bool hitPlayer;
    public bool hitTarget;
    public float damage =0.01f;

    public GameObject agentLineObject;
    public GameObject targetLineObject;

    public LineRenderer lineRenderer;

    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent.gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parent.transform.position;
        lineRenderer.SetPosition(0, agentLineObject.transform.position);
        lineRenderer.SetPosition(1, targetLineObject.transform.position);

        ConjoinedLaser(agentLineObject, targetLineObject, false, damage, agentLineObject.transform.position, targetLineObject.transform.position);
        if(agentLineObject.GetComponent<DamageLineObject>().hitPlayer && targetLineObject.GetComponent<DamageLineObject>().hitPlayer)
        {
            Debug.LogWarning("Damaged Player");
            PlayerController.instance.Damage(damage);
            agentLineObject.GetComponent<DamageLineObject>().hitPlayer = false;
            targetLineObject.GetComponent<DamageLineObject>().hitPlayer = false;
        }
        //Raycast from self to paired agent
        //Draw a line
        //When the player crosses the line it does something

    }

    public void ConjoinedLaser(GameObject _self, GameObject _target, bool isReciever, float damage, Vector3 _agentPos, Vector3 _targetPos)
    {
        Debug.DrawLine(_agentPos, _targetPos, Color.red, 1.0f);
        //Get to ignore triggers
            if(Physics.Raycast(_agentPos,_targetPos,out RaycastHit hit,100,7,QueryTriggerInteraction.Ignore))
            {
                Debug.Log("Raycast Hit: " + hit.collider.name);
                if(hit.collider.transform.root.GetComponent<GameController>() != null 
                    && hit.collider.transform.root.GetComponent<BTAgent>() == null)
                {
                    //Debug.Log("Hit Terrain");
                    hitPlayer = false;
                    return;
                }
                else if(hit.collider.transform.root.GetComponent<BTAgent>()!= null)
                {
                    hitPlayer = false;
                    return;
                }
                else if(hit.collider.transform.root.gameObject.GetComponent<PlayerController>() != null)
                {
                    Debug.Log("Hit Player");
                    hitPlayer = true;
                }
            }
        
    }
}
