using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherProjectile : Projectile
{
    public GameObject TetherProjPartner;
    private LineRenderer line;
    private GameObject tetherPrefab, tetherObject;
    private GameObject initialPos;

    // For if it is the first or second proj
    public int count = 1;


    // Start is called before the first frame update
    void Start()
    {
        tetherPrefab = Resources.Load("Elite/Tether") as GameObject;
        tetherObject = GameObject.Instantiate(tetherPrefab, transform.position, Quaternion.identity);
        tetherObject.transform.parent = gameObject.transform;
        tetherObject.transform.localPosition = Vector3.up;
        line = tetherObject.GetComponent<LineRenderer>();
    }

    public void SetInitialPos(GameObject pos)
    {
        initialPos = pos;
        SetTetherPartner(initialPos);
    }

    protected virtual void Update() // Update
    {
        if (!enabled) return;
        // Player projectiles must use unscaled delta time due to time scale altering abilities

        // Move projectile forwards
        if (isPlayer) transform.Translate(Vector3.forward * speed * Time.unscaledDeltaTime);
        else transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Updating speed for acceleration
        if (isPlayer) speed += acceleration * Time.unscaledDeltaTime;
        else speed += acceleration * Time.deltaTime;

        ProjectileUpdate(); // Child updates

        
        if(count == 1)
        {
            
            SetTether();
        }
        

        if (timer >= lifetime) DestroyProjectile(); // Check if projectile has exceeded its lifetime

        // Update timer
        if (isPlayer) timer += Time.unscaledDeltaTime;
        else timer += Time.deltaTime;

    }

    public void SetTether()
    {
        if(TetherProjPartner == null)
        {
            Destroy(tetherObject);
            return;
        }
        


        //Get tether points
        Vector3 startTetherPoint = transform.position;
        Vector3 endTetherPoint = TetherProjPartner.transform.position;
        startTetherPoint.y = 1;
        endTetherPoint.y = 1;
        line.SetPosition(0, startTetherPoint);
        line.SetPosition(1, endTetherPoint);

    }

    public void SetTetherPartner(GameObject tetherpartner)
    {
        TetherProjPartner = tetherpartner;
    }

    public override void DestroyProjectile(GameObject projectileDestroyer = null) // Called when projectile should kill itself NOW
    {
        Destroy(gameObject);
    }

}
