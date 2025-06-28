using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    private Camera miniMapCamera;
    public float moveSpeed = 0.01f;
    public float zoomSpeed = 100;
    public float minZoom = 200;
    private bool isMouseDown = false;
    private Vector3 oldMousePos = Vector3.zero, currMousePos = Vector3.zero;
    void Awake()
    {
        miniMapCamera = this.GetComponent<Camera>();
    }
    public void Update()
    {
        currMousePos = Input.mousePosition;
        //Controls
        float scroll = Input.mouseScrollDelta.y * -1;
        miniMapCamera.orthographicSize += scroll * zoomSpeed * Time.deltaTime;
        //Clamp zoom
        miniMapCamera.orthographicSize = Mathf.Clamp(miniMapCamera.orthographicSize, minZoom, 1000);
        if(Input.GetMouseButton(0))
        {   
            Vector3 difference = currMousePos - oldMousePos;
            difference.Normalize();
            Vector3 velocity = new Vector3(difference.x, 0, difference.y);
            velocity *= -1f;
            miniMapCamera.transform.position += velocity * moveSpeed * Time.deltaTime;
        }
        oldMousePos = currMousePos;

        gameObject.transform.position = new Vector3(PlayerController.instance.transform.position.x, gameObject.transform.position.y, PlayerController.instance.transform.position.z);
    }
}
