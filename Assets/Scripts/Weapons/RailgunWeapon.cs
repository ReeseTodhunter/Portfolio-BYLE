using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RailgunWeapon : BaseWeapon
{
    public List<AudioClip> chargeAudioClips = null;

    public ParticleSystem railgunParticleSystem;
    public ParticleSystem chargeParticleSystem;
    public ParticleSystem ShootParticleSystem;
    public GameObject punctureVFXPrefab;
    public float chargeSpeed = 1;
    private float chargeAmount = 0;
    public float cooldownSpeed = 0.5f;
    private bool isHoldingFireButton = false;
    public GameObject explosionPrefab;

    public string coolDownAnimName = "";
    public string shootAnimName = "";
    public string swapAnimName = "";
    public string idleAnimName = "";
    private Animator animator;
    private enum RailgunState
    {
        idle,
        charging,
        firing,
        coolingDown
    }
    private RailgunState currRailgunState = RailgunState.idle;

    private void Start()
    {
        animator = PlayerController.instance.GetComponent<Animator>();
    }
    public override void OnFireOneDown()
    {
        base.OnFireOneDown();
        isHoldingFireButton = true;
    }
    public override void OnFireOneUp()
    {
        base.OnFireOneUp();
        isHoldingFireButton = false;    
    }
    private void Update()
    {
        if (currState == WeaponState.DROPPED)
        {
            ChargeSlider.instance.SetSliderVisible(false);
            Idle();
            return;
        }
        if(currState == WeaponState.STOWED)
        {
            ChargeSlider.instance.SetSliderVisible(false);
            return;
        }

        switch (currRailgunState)
        {
            case RailgunState.idle:
                if (isHoldingFireButton) { currRailgunState = RailgunState.charging; PlayAudio(chargeAudioClips); break; }
                gameObject.GetComponent<AudioSource>().Stop();
                chargeAmount -= Time.deltaTime * chargeSpeed;
                if (chargeAmount <= 0) { chargeAmount = 0; }
                ChargeSlider.instance.SetSliderColor(Color.Lerp(Color.grey, Color.white, chargeAmount));
                break;
            case RailgunState.charging:
                if (!isHoldingFireButton) { currRailgunState = RailgunState.idle; }
                chargeAmount += Time.unscaledDeltaTime * chargeSpeed;
                if (chargeAmount >= 1) { currRailgunState = RailgunState.firing; }
                ChargeSlider.instance.SetSliderColor(Color.Lerp(Color.grey, Color.white, chargeAmount));
                break;
            case RailgunState.firing:
                PlayAudio(fireAudioClips);
                animator.Play(coolDownAnimName);
                currRailgunState = RailgunState.coolingDown;
                ChargeSlider.instance.SetSliderColor(Color.red);
                FireRailgun();
                break;
            case RailgunState.coolingDown:
                chargeAmount -= Time.deltaTime * cooldownSpeed;
                if (chargeAmount <= 0) { animator.Play(shootAnimName); currRailgunState = RailgunState.idle; ChargeSlider.instance.SetSliderColor(Color.white); }
                break;
        }
        ChargeSlider.instance.SetSliderValue(chargeAmount);
        if (chargeAmount <= 0) { ChargeSlider.instance.SetSliderVisible(false); }
        else { ChargeSlider.instance.SetSliderVisible(true); }
        UpdateChargeVFX(currRailgunState == RailgunState.coolingDown);

    }
    private void UpdateChargeVFX(bool _isCoolingDown = false)
    {
        ParticleSystem.EmissionModule em = chargeParticleSystem.emission;
        float p = Mathf.Clamp(chargeAmount,0,1);
        em.rateOverTime = _isCoolingDown ? 0 : p * 256;
    }
    private void FireRailgun()
    {
        ShootParticleSystem.Play();
        //Get distance
        float distance = 99;
        float emissionRatio = 128/5;
        Vector3 pos = transform.position + transform.forward * distance;
        if(Physics.Raycast(transform.position,transform.forward,out RaycastHit hit,99,LayerMask.GetMask("Untraversable"), QueryTriggerInteraction.Ignore))
        {
            distance = hit.distance/2;
            pos = hit.point;
            pos.y = 1;
        }
        Vector3 start;
        start = transform.position;
        start.y = .5f;
        RaycastHit[] hits = Physics.SphereCastAll(start,3,transform.forward,distance,LayerMask.GetMask("Enemy"),QueryTriggerInteraction.Ignore);
        foreach(RaycastHit col in hits)
        {
            if(col.collider.gameObject.TryGetComponent<Character>(out Character entity))
            {
                entity.Damage(6 * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), false,false);
                entity.Burn(3,.5f);
            }
            GameObject punctureVFX = Instantiate(punctureVFXPrefab,col.point,transform.rotation);
        }
        ParticleSystem.ShapeModule sm = railgunParticleSystem.shape;
        sm.radius = distance;
        sm.position = Vector3.forward * distance;
        sm.radiusSpeed = distance * -10;
        ParticleSystem.EmissionModule em = railgunParticleSystem.emission;
        em.rateOverTime = distance * emissionRatio;
        GameObject impact = Instantiate(explosionPrefab,pos,transform.rotation);
        railgunParticleSystem.Play();
    }
}
