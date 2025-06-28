using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    [SerializeField] private Projection _projection;

    private void Update()
    {
        //HandleControls();
        _projection.SimulateTrajectory(_ballPrefab, _ballSpawn.position, _ballSpawn.forward * _force);
        if (GameManager.GMinstance.GetInputDown("keyShoot1"))
        {
            var spawned = Instantiate(_ballPrefab, _ballSpawn.position, _ballSpawn.rotation);

            spawned.Initialise(_ballSpawn.forward * _force, false);

        }
    }

    [SerializeField] private Grenade _ballPrefab;
    [SerializeField] private float _force = 20;
    [SerializeField] private Transform _ballSpawn;

}
