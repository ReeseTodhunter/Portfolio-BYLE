using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveController : MonoBehaviour
{
    public float emissiveIntensity = 0;
    public float maxIntensity = 10,minIntensity = 5;
    private MaterialPropertyBlock block;
    public List<Renderer> emissives = new List<Renderer>();
    void Update()
    {
        emissiveIntensity = Mathf.Clamp(emissiveIntensity,minIntensity,maxIntensity);
        //Update blocks
        foreach(Renderer rnd in emissives)
        {
            block = new MaterialPropertyBlock();
            rnd.GetPropertyBlock(block);
            block.SetFloat("_EmissiveStrength", emissiveIntensity);
            rnd.SetPropertyBlock(block);
        }
    }
}
