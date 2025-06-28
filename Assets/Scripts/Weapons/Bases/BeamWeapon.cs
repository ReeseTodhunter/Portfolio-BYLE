using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamWeapon : BaseWeapon
{
    private float heat = 0;
    private float maxHeat = 1;
    public float heatDissipationRate = 0.33f;
    public float heatBuildupRate = 1f;
    public float forcedDissipateDuration = 2;
    private bool isFiring = false;
    private bool forcedDissipating = false;
    public GameObject laserPrefab;
    private GameObject laserObject;
    public Transform laserStartPos;
    private Transform impactVFX;
    public string coolDownAnimName = "";
    public string shootAnimName = "";
    public string swapAnimName = "";
    public string idleAnimName = "";
    private Animator animator;
    public float tickRate = 5;
    private float tickTimer = 0;
    private bool canDamge = false;
    public float damagePerSecond = 2;
    [Range(0,1)]
    public float critChance = 0.1f;
    public float critMultiplier = 2;
    private enum State
    {
        inactive,
        shooting,
        overheated
    }
    State currentState = State.inactive;
    void Start()
    {
        animator = PlayerController.instance.GetComponent<Animator>();
        currentState = State.inactive;
        laserObject = GameObject.Instantiate(laserPrefab,laserStartPos.position,laserStartPos.rotation);
        laserObject.SetActive(true);
        laserObject.GetComponent<Animator>().Play("Asleep");
        laserObject.transform.parent = laserStartPos;
        impactVFX = laserObject.transform.GetChild(2);
        impactVFX.parent = null;
    }
    void Update()
    {
        if (currState == WeaponState.DROPPED)
        {
            Idle();
        }

        laserObject.GetComponent<LineRenderer>().SetPosition(0,laserStartPos.position);
        switch(currentState)
        {
            case State.inactive:
                //PlayAudio(reloadAudioClips);
                if (heat <= 0)
                {
                    heat = 0;
                    HeatIndicator.instance.SetVisible(false);
                    return;
                }
                heat -= Time.unscaledDeltaTime * heatDissipationRate;
                HeatIndicator.instance.SetVisible(true);
                HeatIndicator.instance.SetHeat(heat,maxHeat);
                break;
            case State.shooting:
                HeatIndicator.instance.SetVisible(true);
                if (gameObject.GetComponent<AudioSource>().isPlaying == false)
                {
                    PlayAudio(fireAudioClips);
                }
                
                if (heat >= maxHeat)
                {
                    heat = maxHeat;
                    HeatIndicator.instance.SetCoolingDown(forcedDissipateDuration);
                    laserObject.GetComponent<Animator>().Play("WindDown");
                    animator.Play(coolDownAnimName);
                    currentState = State.overheated;
                    return;
                }
                heat += Time.unscaledDeltaTime * heatBuildupRate;
                GetLaserEndPos();
                HeatIndicator.instance.SetHeat(heat,maxHeat);
                break;
            case State.overheated:
                heat -= Time.unscaledDeltaTime * 1 / forcedDissipateDuration;
                if(heat <= 0)
                {
                    currentState = State.inactive;
                    HeatIndicator.instance.SetVisible(false);
                    animator.Play("BeamCooldownDeTransition");
                }
                break;
        }
        tickTimer += Time.unscaledDeltaTime;
        if(tickTimer < 1 / tickRate){canDamge = false; return;}
        canDamge = true;
        tickTimer = 0;
    }
    public override void OnFireOneHeld()
    {
        if(currentState != State.overheated)
        {
            laserObject.GetComponent<Animator>().Play("Active");
            currentState = State.shooting;
        }
    }
    public override void OnFireOneDown()
    {
        if(currentState != State.overheated)
        {
            animator.Play(shootAnimName);
        }
    }
    public override void OnFireOneUp()
    {
        if(currentState != State.overheated)
        {
            laserObject.GetComponent<Animator>().Play("WindDown");
            animator.Play(idleAnimName);
            currentState = State.inactive;
        }
    }
    private void GetLaserEndPos()
    {
        RaycastHit hit;
        Vector3 endPos;
        if(Physics.Raycast(laserStartPos.position, laserStartPos.forward, out hit, 100, LayerMask.GetMask("Untraversable", "Player", "Enemy", "Hurtbox", "SeeThrough"), QueryTriggerInteraction.Ignore))
        {
            endPos = hit.point;
            if(hit.collider.gameObject.tag != "Player" && canDamge)
            {
                if(hit.collider.gameObject.TryGetComponent<Character>(out Character _character))
                {
                    bool isCrit = GetIsCrit();
                    float _dmg;
                    if(isCrit)
                    {
                        _dmg = damagePerSecond * critMultiplier;
                    }
                    else
                    {
                        _dmg = damagePerSecond;
                    }
                    _dmg *= 1 + PlayerController.instance.GetModifier(ModifierType.Damage);
                    _character.Damage(_dmg / tickRate, true, false, false,EffectType.None,isCrit);
                    tickTimer = 0;
                    canDamge = false;
                }
            }
        }
        else
        {
            endPos = laserStartPos.position + laserStartPos.forward * 100;
        }
        laserObject.GetComponent<LineRenderer>().SetPosition(1,endPos);
        impactVFX.position = endPos;
        impactVFX.forward = impactVFX.forward * -1;
    }
    public override void OnPickup()
    {
        base.OnPickup();
        MagazineUIController.instance.RefreshMagazineUI(0,0);
    }
    public override void OnEquip()
    {
        base.OnEquip();
        MagazineUIController.instance.RefreshMagazineUI(0,0);
        transform.localPosition = Vector3.zero;
    }
    public override string GetSwapAnimName()
    {
        return swapAnimName;
    }
    public override string GetIdleAnimName()
    {
        return idleAnimName;
    }
    protected bool GetIsCrit()
    {
        float rnd = Random.Range(1,100);
        if(rnd < critChance * 100)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
