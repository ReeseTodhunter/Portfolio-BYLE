using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    /*
     * This script controls any flash effects for bt-agents
     * -Tom
     */
    public Material damageMaterial; //damage material
    private List<Material> originalMaterials = new List<Material>(); //cached original materials
    public float duration = .1f; //flash duration
    private float timer = 0; //flash timer
    private bool isFlashing = false; 
    private bool lerpMaterials = false; //whether or not to lerp materials
    private List<Transform> children = new List<Transform>(); //All child renderers
    public Material spawnMaterial; //spawn flash
    public Material burnMaterial; //burn flash
    public Material stunMaterial; //burn flash
    private Material CurrMaterial; //curr flash
    public List<Transform> noFlashObjects = new List<Transform>(); //any gameobjects included in this list wont flash
    public enum FlashType //type of flash
    {
        DAMAGE,
        BURN,
        STUN,
        SPAWNING
    }
    void Awake()
    {
        spawnMaterial = Resources.Load("Materials/damageFlashes/SpawnFlash") as Material;
        burnMaterial =  Resources.Load("Materials/damageFlashes/BurnFlash") as Material;
        stunMaterial =  Resources.Load("Materials/damageFlashes/StunFlash") as Material;
    }
    void Start()
    {
        //Initialise mats
        //Get all transforms
        children.Clear();
        GetAllChildren(transform); //get all renderers that will flash
        for(int i = 0; i < children.Count; i++) //get all their default materials
        {
            originalMaterials.Add(children[i].GetComponent<MeshRenderer>().material);
        }
    }
    public void StartFlash(float _duration = .1f, bool _lerpMaterials = false, FlashType type = FlashType.DAMAGE) //start a flasj
    {
        //set flash material
        switch(type)
        {
            case FlashType.DAMAGE:
                CurrMaterial = damageMaterial;
                break;
            case FlashType.STUN:
                CurrMaterial = stunMaterial;
                break;
            case FlashType.BURN:
                CurrMaterial = burnMaterial;
                break;
            case FlashType.SPAWNING:
                CurrMaterial = spawnMaterial;
                break;
            default:
                CurrMaterial = damageMaterial;
                break;
        }
        duration = _duration;
        lerpMaterials = _lerpMaterials;
        timer = 0;
        isFlashing = true;
        for(int i = 0; i < children.Count; i++) //set the flash mat on all children
        {
            if(children[i] == null){continue;}
            children[i].GetComponent<MeshRenderer>().material = CurrMaterial;
        }
    }
    public void Update()
    {
        if(!isFlashing){return;}
        if(lerpMaterials) //lerp materials
        {
            for(int i =0; i < children.Count; i++)
            {
                children[i].GetComponent<MeshRenderer>().material.Lerp(CurrMaterial, originalMaterials[i], timer / duration);
            }
        }
        timer += Time.deltaTime;
        if(timer >= duration){EndFlash();} //endflash
    }
    private void EndFlash()
    {
        for(int i = 0; i < children.Count; i++) //restore the original material to all children
        {
            if(children[i] == null){continue;}
            children[i].GetComponent<MeshRenderer>().material = originalMaterials[i];
        }
        isFlashing = false;
    }
    private void GetAllChildren(Transform parent) //recursively get all children from a parent
    {
        foreach(Transform child in parent) 
        {
            if(noFlashObjects.Contains(child)){continue;}
            if(child.gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer rend))
            {
                children.Add(child);
            }
            Transform _hasChildren = child.GetComponentInChildren<Transform>();
            if(_hasChildren != null)
                GetAllChildren(child);
        }
    }
}
