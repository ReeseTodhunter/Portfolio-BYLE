using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortaCannon : BaseActivePowerup
{
    [SerializeField] GameObject rocketProjectile = null;
    [SerializeField] GameObject rocketExplosion = null;
    [SerializeField] float detectionRange = 25.0f;
    BTAgent closestEnemy = null; // Enemy that the launcher is locked onto
    [SerializeField] GameObject crosshairPrefab = null;
    GameObject crosshair = null;

    public override float UseAbility()
    {
        // Find nearest enemy to lock on to
        List<BTAgent> enemies = EnemySpawningSystem.instance.GetEnemies();
        closestEnemy = null;
        float closestDistance = 999.0f;
        foreach (BTAgent enemy in enemies)
        {
            if (!enemy.isAlive()) continue; // Ignore dead enemies

            float distance = Vector3.Distance(PlayerController.instance.transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= detectionRange) // No enemies beyond 50 units will be locked onto
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        // Dont use ability if no enemies found
        if (enemies.Count == 0 || closestDistance == 999.0f) { return 0.0f; }

        // Kill
        // Rocket that flies off your back
        Instantiate(rocketProjectile, transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f)).GetComponent<MortarProjectile>()
            .InitMortar(rocketExplosion, 5.0f, null)
            .Init(30.0f, 20.0f, 3.0f, 0.0f, PlayerController.instance.gameObject);
        // The actual kill rocket
        Instantiate(rocketProjectile, closestEnemy.transform.position + 60.0f * Vector3.up, Quaternion.Euler(90.0f, 0.0f, 0.0f)).GetComponent<MortarProjectile>()
            .InitMortar(rocketExplosion, 5.0f, closestEnemy.transform)
            .Init(30.0f, 20.0f, 3.0f, 20.0f * (1+PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject);

        // Hide crosshair
        crosshair.transform.position = -1000.0f * Vector3.up;

        return base.UseAbility();
    }

    private void Start()
    {
        // Spawn crosshair prefab under map
        crosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);
        crosshair.transform.position = -1000.0f * Vector3.up;
    }

    private void Update()
    {
        base.Update();

        if (equipped && PlayerController.instance.GetAbilityCooldownTime() <= 0.0f)
        {
            // Search for closest enemy to place crosshair on
            List<BTAgent> enemies = EnemySpawningSystem.instance.GetEnemies();
            closestEnemy = null;
            float closestDistance = 999.0f;
            foreach (BTAgent enemy in enemies)
            {
                if (!enemy.isAlive()) continue; // Ignore dead enemies

                float distance = Vector3.Distance(PlayerController.instance.transform.position, enemy.transform.position);
                if (distance < closestDistance && distance <= detectionRange) // No enemies beyond 50 units will be locked onto
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }

            // Hide crosshair if no enemies are found
            if (enemies.Count == 0 || closestDistance == 999.0f)
            {
                crosshair.transform.position = -1000.0f * Vector3.up;
                return;
            }

            // Place crosshair at tracked enemy's "feet"
            crosshair.transform.position = new Vector3(closestEnemy.transform.position.x, 0.01f, closestEnemy.transform.position.z);

        }

        // Ability isn't active or is on cooldown
        else
        {
            crosshair.transform.position = -1000.0f * Vector3.up;
        }
    }
}
