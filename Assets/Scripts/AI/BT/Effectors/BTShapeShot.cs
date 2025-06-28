using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTShapeShot : BTNode
{
    /*
     * This node allows red cultist enemies to fire projectile in shapes, such as squares, circles and triangles
     * -Tom
     */
    public enum attackShape {circle, square, triangle,random}; //shape to shoot
    private attackShape shape; //current shape
    private GameObject projectilePrefab; // prefab of projectile
    private float speed, damage, windupDuration, projectileCount; //stats of projectiles
    private float timer, interval; //interval between projectiles
    private int currentCount; //current count of projectiles
    private List<GameObject> projectiles = new List<GameObject>(); //list of projectiles
    private List<Vector3> projectilePositions = new List<Vector3>(); //list of projectile spawn positions
    private string animName;

    //Constructor
    public BTShapeShot(BTAgent _agent, Projectiles _projectile, Projectiles _byleProjectile, ProjectileMats _mat, float _speed, float _damage,attackShape _shape, float _windupDuration, float _projectileCount, string _animName = "")
    {
        agent = _agent;
        projectilePrefab = ProjectileLibrary.instance.GetProjectile(_projectile);
        speed = _speed;
        damage = _damage;
        shape = _shape;
        windupDuration = _windupDuration;
        projectileCount = _projectileCount;
        interval = windupDuration / projectileCount;
        animName = _animName;
    }

    public override NodeState Evaluate(BTAgent agent) //Evaluate node output
    {
        if(!EQS.instance.GetNearestNode(agent.transform.position).GetLineOfSight()) //return false if no line of sight
        {
            return NodeState.FAILURE;
        }
        agent.SetCurrentAction(this);
        agent.SetMovementEnabled(false);
        timer = 0;
        currentCount = 0;
        projectiles = new List<GameObject>();
        agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
        projectilePositions.Clear();
        switch(shape) //Get shape positions
        {
            case attackShape.circle:
                projectilePositions = GetProjectilePositions(((int)projectileCount),3,((int)projectileCount));
                break;
            case attackShape.square:
                projectilePositions = GetProjectilePositions(4,3,((int)projectileCount));
                break;
            case attackShape.triangle:
                projectilePositions = GetProjectilePositions(3,3,((int)projectileCount));
                break;
            case attackShape.random:
                int rnd = Random.Range(3,6);
                projectilePositions = rnd > 4 ? GetProjectilePositions(((int)projectileCount),4,((int)projectileCount)) : GetProjectilePositions(rnd,3,((int)projectileCount));
                break;
        }
        if(animName != "") //play anim
        {
            agent.GetComponent<Animator>().Play(animName);
        }
        return NodeState.SUCCESS;
    }
    public override void UpdateNode(BTAgent agent) //update node
    {
        GameObject projectile = null;

        timer += Time.deltaTime;
        if(timer <= interval) return;
        timer = 0;
        if(currentCount >= projectilePositions.Count) //Fire shape
        {
            FireShape();
            return;
        }
        //Get current position
        Vector3 currPos = projectilePositions[currentCount];
        projectile = GameObject.Instantiate(projectilePrefab, currPos, Quaternion.identity); //Instantiate projectiles along shape
        projectiles.Add(projectile);
        projectile.GetComponent<IProjectile>().Init(0, 0, 20, damage, agent.transform.tag);
        currentCount++;
    }  
    private void FireShape() //Fire all projectile at player
    {
        agent.ClearCurrentAction();
        foreach(GameObject currentProj in projectiles)
        {
            if(currentProj == null){continue;}
            currentProj.transform.rotation = agent.transform.rotation;
            currentProj.GetComponent<IProjectile>().speed = speed;
        }
        projectiles.Clear();
        agent.SetMovementEnabled(true);
    }
    private List<Vector3> GetProjectilePositions(int sides, int radius = 3, int positionCount = 12)
    {
        List<Vector3> positions = new List<Vector3>();
        //Get vertices
        Vector3 offset = Vector3.zero;
        float angleInteravl = 360 / sides;
        Vector3 pos = Vector3.zero;
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < sides; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(angleInteravl * i, Vector3.up) * Vector3.one;
            direction.y = 0;
            pos = agent.transform.position + direction * radius;
            vertices.Add(pos);
            //Debug.Log(pos);
        }
        //
        Vector3 pos2;
        float percentage = 0;
        float vertexPercentage = 0;
        int prevIndex, currIndex;
        float interVertextPercentage = 0;
        for (float i = 0; i < positionCount; i++)
        {
            //Percentage of the current point
            percentage = i / positionCount;
            //Get the percentage along the vertices list
            vertexPercentage = percentage * vertices.Count;
            //Get the previous vertex
            prevIndex = Mathf.FloorToInt(vertexPercentage);
            //Get the current vertex
            currIndex = Mathf.CeilToInt(vertexPercentage);
            if (currIndex >= vertices.Count) { currIndex = 0; }
            interVertextPercentage = vertexPercentage - prevIndex;
            pos2 = Vector3.Lerp(vertices[prevIndex], vertices[currIndex], interVertextPercentage);
            positions.Add(pos2);
        }
        return positions;
    }
    public override void DeInitialiseNode(BTAgent agent) //Destroy all projectiles when agent dies
    {
        if(projectiles == null) { return; }
        for (int i = 0; i < projectiles.Count; i++)
        {
            GameObject.Destroy(projectiles[i]);
        }
    }
}
