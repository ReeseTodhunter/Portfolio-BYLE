using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHidingScript : MonoBehaviour
{
    public bool updateChildren;
    public enum transparencyState
    {
        DECREASING,
        INCREASING,
        STABLE
    }
    private transparencyState currState = transparencyState.STABLE;
    private MaterialPropertyBlock block;
    private Renderer rnd;
    private float maxAlpha = 1, minAlpha = -.15f, currAlpha =0;
    private static Vector3 playerCentre = Vector3.one * 0.5f;
    public static void SetPlayerScreenPosition(Transform modelPos)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(modelPos.position);
        playerCentre.x = pos.x / Screen.width;
        playerCentre.y = pos.y / Screen.height;
        playerCentre.z = 0;
    }
    void Awake()
    {
        currAlpha = maxAlpha;
        block = new MaterialPropertyBlock();
        if(!updateChildren)
        {
            rnd = this.GetComponent<Renderer>();
            rnd.GetPropertyBlock(block);
            block.SetFloat("_Alpha", currAlpha);
            rnd.SetPropertyBlock(block);
            return;
        }
        SetAlphaInChildren();
    }
    void Update()
    {
        switch(currState)
        {
            case transparencyState.DECREASING:
                DecreasingUpdate();
                break;
            case transparencyState.INCREASING:
                IncreasingUpdate();
                break;
            case transparencyState.STABLE:
                return;
        }
        if(updateChildren)
        {
            SetAlphaInChildren();
            return;
        }
        rnd.GetPropertyBlock(block);
        block.SetFloat("_Alpha",currAlpha);
        block.SetVector("_Centre",playerCentre);
        rnd.SetPropertyBlock(block);
    }
    private void DecreasingUpdate()
    {
        currAlpha -= Time.deltaTime;
        if(currAlpha <= minAlpha)
        {
            currAlpha = minAlpha;
            currState = transparencyState.STABLE;
        }
    }
    private void IncreasingUpdate()
    {
        currAlpha += Time.deltaTime;
        if(currAlpha >= maxAlpha)
        {
            currAlpha = maxAlpha;
            currState = transparencyState.STABLE;
        }
    }
    private void SetAlphaInChildren()
    {
        foreach(Transform child in transform)
        {
            if(child.TryGetComponent<Renderer>(out Renderer renderer))
            {
                rnd = child.gameObject.GetComponent<Renderer>();
                rnd.GetPropertyBlock(block);
                block.SetFloat("_Alpha",currAlpha);
                rnd.SetPropertyBlock(block);
            }
        }
    }
    public void SetTransparencyState(transparencyState _state){currState = _state;}
}
