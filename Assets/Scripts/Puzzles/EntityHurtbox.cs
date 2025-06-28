using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHurtbox : MonoBehaviour, IHurtResponder
{
    [SerializeField] private Rigidbody m_Entity;

    private List<HurtBox> hurtBoxes = new List<HurtBox>();
    bool IHurtResponder.CheckHit(HitData data)
    {
        return true;
    }

    void IHurtResponder.Response(HitData data)
    {
        Debug.Log("Entity Hurt");
    }

    // Start is called before the first frame update
    void Start()
    {
        hurtBoxes = new List<HurtBox>(GetComponentsInChildren<HurtBox>());
        foreach(HurtBox _hurtBox in hurtBoxes)
        {
            _hurtBox.HurtResponder = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
