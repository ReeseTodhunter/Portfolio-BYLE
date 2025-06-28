using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperWeapon : ProjectileWeapon
{
    public float perfectZoneTime;
    public float perfectZoneSize;
    public float failedDamageMultiplier;
    private float reloadStartTime = 0;
    private bool attemptMade = false;
    float lockTimer = 0;
    bool shootLocked = false;
    float shootLockDuration = .5f;
    protected override void OnShoot()
    {
        if(shootLocked)
        {
            PlayerController.instance.GetComponent<Animator>().Play("SniperIdle");
            return;
        }
        base.OnShoot();
        if(currMagSize == 0)
        {
            //Trigger Reload
            attemptMade = false;
            shootLocked = false;
            critChance = 0;
            Reload();
        }
    }
    public override void OnReloadDown()
    {
        if (shootLocked) { return; }
        if (currMagSize != 0){return;}
        Reload();
        return;
    }
    public override void OnReloadHeld()
    {
        if (shootLocked) { return; }
        return;
    }
    public override void OnFireOneHeld()
    {
        if (shootLocked) { return; }
        base.OnFireOneHeld();
        return;
    }
    public override void OnFireOneDown()
    {
        base.OnFireOneHeld();
    }
    private void Reload()
    {
        if(attemptMade){return;}
        if(currReloadState == reloadState.RELOADING)
        {
            AttemptQuickReload();
            return;
        }
        dmg = baseDmg;
        attemptMade = false;
        reloadStartTime = Time.time;
        currReloadState = reloadState.RELOADING;
        ReloadAnimation.instance.BeginAnimation(reloadDuration,perfectZoneTime,perfectZoneSize,true);
    }
    private void AttemptQuickReload()
    {
        attemptMade = true;
        float currTime = Time.time - reloadStartTime;
        if(Mathf.Abs(currTime - perfectZoneTime) < perfectZoneSize /2)
        {
            //critChance = 100;
            currMagSize = magazineSize;
            currReloadState = reloadState.READY;
            MagazineUIController.instance.RefreshMagazineUI(magazineSize,currMagSize);
            reloadTimer = 0;
            PlayerController.instance.GetComponent<Animator>().speed = 1;
            ReloadAnimation.instance.StopAnimation();
            currReloadState = reloadState.READY;
            OnReloadFinished();
        }
        else
        {
            critChance = 0;
            PlayerController.instance.GetComponent<Animator>().speed = 1;
            ReloadAnimation.instance.FailFastReload();
        }
    }
    void Update()
    {
        if (currState == WeaponState.DROPPED)
        {
            Idle();
        }
        if (!shootLocked)
        {
            UpdateProjectileWeapon();
            return;
        }
        lockTimer += Time.unscaledDeltaTime;
        if(lockTimer < shootLockDuration){return;}
        shootLocked = false;
    }
    protected override void OnReloadFinished()
    {
        if(!attemptMade){dmg = baseDmg;}
        //Play weapon fire audio
        PlayAudio(reloadAudioClips);
        if (shootLocked) { return; }
        PlayerController.instance.GetComponent<Animator>().Play(reloadAnimName);
        lockTimer = 0;
        shootLocked = true;
    }
}
