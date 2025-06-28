using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera offset values")]
    public Vector3 camOffset;
    private Vector3 mousePos;
    [Range(0,1)]
    [Space(6), Header("Camera Drift values")]
    public float camDrift = 0.5f;
    public float maxCamDrift = 1f;
    private bool active = false;
    private Vector3 camShakeVector = Vector3.zero;
    private Vector3 currCamShakeVector = Vector3.zero;
    [Space(6),Header("Camera shake values")]
    public float camShakeDuration = 1;
    public float maxCamShake = 0.5f;
    public bool biasTowardsMousePosition = false;
    private float decayTimer = 0;
    private Vector3 offset;
    public static CameraController instance;
    private bool cameraShakingOverTime = false;
    private float timer = 0;
    private float duration = 0;
    private float interval = 0.05f, intervalTimer = 0;
    private float overTimeStrength = 1;
    private bool cameraLocked = false;
    private List<GameObject> culledObstacles = new List<GameObject>();
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
                Destroy(this.gameObject);
        }
        instance = this;
    }

    public void Start()
    {
        culledObstacles.Clear();
        culledObstacles = new List<GameObject>();
        cameraLocked = false;
        if (PlayerController.instance != null)
        {
            active = true;
        }
        transform.position = PlayerController.instance.transform.position + camOffset;
        transform.eulerAngles = new Vector3(45, 0, 0);
    }
    public void Update()
    {
        if(!active){return;}
        if(cameraLocked){return;}
        // Create a ray at the mouses position on screen
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Cast the ray from the mouse position out into the scene if hitting the ground layer turning the model to face the collision point
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, 1 << 6))
        {
            mousePos = new Vector3(hitInfo.point.x, 0, hitInfo.point.z);
        }
        offset = mousePos - PlayerController.instance.transform.position;
        if(offset.magnitude > maxCamDrift)
        {
            offset.Normalize();
            offset *= maxCamDrift;
        }

        //Active cameraShake
        if(cameraShakingOverTime)
        {
            timer+= Time.unscaledDeltaTime;
            intervalTimer += Time.unscaledDeltaTime;
            if(intervalTimer > interval)
            {
                intervalTimer = 0;
                ShakeCamera(overTimeStrength);
            }
            if(timer > duration)
            {
                cameraShakingOverTime = false;
            }
        }

        //Cam offset
        UpdateCameraShake();
        RaycastObstaclesToCull();
        transform.position = Vector3.Lerp(PlayerController.instance.transform.position, PlayerController.instance.transform.position + offset, camDrift) + camOffset + currCamShakeVector;
    }
    private void UpdateCameraShake()
    {
        //Iterate timer
        decayTimer += Time.unscaledDeltaTime;
        //skip lerp if 1
        if(decayTimer > camShakeDuration)
        {
            return;
        }
        //Lerp to zero
        currCamShakeVector = Vector3.Lerp(camShakeVector,Vector3.zero,decayTimer / camShakeDuration);
    }
    private void RaycastObstaclesToCull()
    {
        // //WIP

        //Disable all active materials
        if (culledObstacles == null)
        {
            return;
        }

        foreach (GameObject obj in culledObstacles)
        {
            if (obj.TryGetComponent<ObstacleHidingScript>(out ObstacleHidingScript script))
            {
                if (culledObstacles.Contains(obj))
                {
                    script.SetTransparencyState(ObstacleHidingScript.transparencyState.INCREASING);
                }
            }
        }
        culledObstacles.Clear();
        //Get all objects in between player and camera
        RaycastHit[] hits;
        Vector3 direction = PlayerController.instance.transform.position - transform.position;
        float distance = direction.magnitude;
        direction.Normalize();
        hits = Physics.SphereCastAll(transform.position, 1.5f, direction, distance, 1 << 8, QueryTriggerInteraction.Collide);
        List<GameObject> obstacles = new List<GameObject>();
        foreach (RaycastHit hit in hits)
        {
            if (Vector3.Distance(transform.position, hit.transform.position) > distance) { continue; }
            if (hit.collider.gameObject.TryGetComponent<ObstacleHidingScript>(out ObstacleHidingScript script) && hit.collider.gameObject.tag == "obstacle")
            {
                obstacles.Add(hit.collider.gameObject);
                script.SetTransparencyState(ObstacleHidingScript.transparencyState.DECREASING);
            }
        }
        //Refresh culled obstacles 
        culledObstacles.AddRange(obstacles);

    }
    public void ShakeCamera(float _strength = 1)
    {

        //Randomise camShakeVector
        float x,z;
        if(biasTowardsMousePosition)
        {
            if(camOffset.x <= 0)
            {
                x = Random.Range(camOffset.x,10);
            }
            else
            {
                x = Random.Range(-10, camOffset.x);
            }

            if(camOffset.y <= 0)
            {
                z = Random.Range(camOffset.y,10);
            }
            else
            {
                z = Random.Range(-10, camOffset.y);
            }
        }
        else
        {
            x = Random.Range(-10,10);
            z = Random.Range(-10,10);
        }
        camShakeVector = new Vector3(x,0,z);
        camShakeVector.Normalize();
        camShakeVector *= maxCamShake * _strength;
        decayTimer = 0;
    }
    public void ShakeCameraOverTime(float _duration, float _strength)
    {
        overTimeStrength = _strength;
        duration = _duration;
        timer = 0;
        intervalTimer = 0;
        cameraShakingOverTime = true;
    }
    public void SetCameraLocked(bool _isLocked)
    {
        cameraLocked = _isLocked;
    }
    public Vector3 GetOffset(){return offset;}
}
