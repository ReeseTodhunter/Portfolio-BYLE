using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirismMaterial : MonoBehaviour
{
    [SerializeField] List<Material> materials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material = materials[Random.Range(0,materials.Count)];
    }
}
