using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealZone : MonoBehaviour
{
    public float bulletSlowAmount;
    private Projectile effectedProjectile;

    private float regenTimer = 0.0f;
    public float healPerSecond = .66f;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<BTAgent>(out BTAgent agent))
        {
            if (agent.GetHealth() < agent.GetMaxHealth())
            {
                agent.Heal(Time.deltaTime * healPerSecond);
                regenTimer = 0;
                if (agent.gameObject.GetComponentInChildren<EnemyHealthBar>())
                {
                    agent.gameObject.GetComponentInChildren<EnemyHealthBar>().alwaysVisible = true;
                }
            }
        }
        regenTimer += Time.deltaTime;
    }

    public float maxSize = 1.5f; // Maximum size of the sphere
    public float minSize = 0.5f; // Minimum size of the sphere
    public float resizeSpeed = 0.25f; // Speed of resizing

    private float currentSize = 1f; // Current size of the sphere
    private bool isGrowing = true; // Flag to indicate whether the sphere is growing

    void Start()
    {
        currentSize = transform.localScale.x; // Start with the minimum size
        minSize = currentSize * 0.5f;
        maxSize = currentSize * 1.5f;
        transform.localScale = new Vector3(currentSize, currentSize, currentSize);
    }

    void Update()
    {
        // Adjust the size based on growth or shrinkage
        if (isGrowing)
        {
            currentSize += Time.deltaTime * resizeSpeed;
            if (currentSize >= maxSize)
            {
                currentSize = maxSize;
                isGrowing = false;
            }
        }
        else
        {
            currentSize -= Time.deltaTime * resizeSpeed;
            if (currentSize <= minSize)
            {
                currentSize = minSize;
                isGrowing = true;
            }
        }

        // Update the scale of the sphere
        gameObject.transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(currentSize, currentSize, currentSize);
    }
}
