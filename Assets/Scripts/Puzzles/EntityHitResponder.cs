using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHitResponder : MonoBehaviour, IHitResponder
{
    [SerializeField]
    private bool Attack;
    [SerializeField]
    private int Damage = 10;
    [SerializeField]
    private HitBox hitBox;
    int IHitResponder.Damage { get => Damage; }

    bool IHitResponder.CheckHit(HitData data)
    {
        return true;
    }

    void IHitResponder.Response(HitData data)
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        hitBox.HitResponder = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Attack)
        {
            hitBox.CheckHit();
        }
    }
}
