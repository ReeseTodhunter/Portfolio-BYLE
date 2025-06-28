using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevenantActions : Tree
{
    public TrailRenderer dashTrail;
    public Transform impaleTransform;
    public Transform roomCentre;
    public Transform primaryRing, secodaryRing, TertiaryRing, scytheRing;
    protected override BTNode SetupTree(BTAgent agent)
    {
        //Primitives
        BTNode spreadShot = new BTSpreadShot(agent,"Projectiles/RevenantProjectile",12,15,0,20,135,1.1f,"SyctheSlash", 3, false,false);
        BTNode dash = new BTRevenantDash(agent,30,15,4,15,1,dashTrail);
        BTNode impale = new BTRevenantImpale(agent,15,15,2,"ImpaleWindup","Impale",5f,1,12/60f,3f,50f,6f,3f,impaleTransform, dashTrail);
        BTNode Vortex = new BTRevenantVortex(agent, 15, 20, 10,null, "Projectiles/RevenantProjectile", null, roomCentre, primaryRing, secodaryRing, TertiaryRing, scytheRing);
        //Composites
        List<BTNode> temp = new List<BTNode>();
        temp.Add(spreadShot);
        temp.Add(dash);
        temp.Add(impale);
        temp.Add(Vortex);
        BTNode rndSelector = new BTRandSelector(agent,temp);
        return rndSelector;
    }
}
