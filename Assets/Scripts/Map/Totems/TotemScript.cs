using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;
public class TotemScript : MonoBehaviour, ISpawnerSubscriber
{
    public RoomController parentRoom;

    public float interactDistance = 3;
    public GameObject interactText;
    public List<ParticleSystem> eyeFire = new List<ParticleSystem>();
    public Gradient activeColorGradient;
    protected Vector3 pos;
    protected string baseText;

    public Transform itemPos;
    public List<GameObject> potentialPowerUps;

    public void Start()
    {
        Initialise();
    }
    public virtual void Initialise()
    {
    }
    public virtual void Activate()
    {
        
    }
    public virtual void DeActivate()
    {
        
    }
    public virtual void AllEnemiesDead() { }

}
