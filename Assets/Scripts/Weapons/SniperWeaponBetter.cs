using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperWeaponBetter : ProjectileWeapon
{
    public float perfectZoneTime; // Centre point of perfect reload time
    public float perfectZoneSize; // Size of perfect reload

    float reloadStartTime; // Time that the last reload was started at
    bool attemptedFastReload; // True if the player has tried to fast reload in current reload cycle

    protected override void OnShoot()
    {
        animator.Play(idleAnimName);
        base.OnShoot();
    }

    // When reload pressed
    public override void OnReloadDown()
    {
        // Don't reload if mag is full
        if (currMagSize == magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize))) { return; }

        // If currently reloading and player hasn't attempted to fast reload, call fast reload
        if (!attemptedFastReload && currReloadState == reloadState.RELOADING) FastReload();

        // Start reload if possible
        else if (currReloadState == reloadState.READY)
        {
            // Set reload state
            currReloadState = reloadState.RELOADING;

            // Reload timer
            reloadStartTime = Time.unscaledTime;
            reloadTimer = 0;

            // Reload UI animation
            ReloadAnimation.instance.BeginAnimation(reloadDuration / (1 + PlayerController.instance.GetModifier(ModifierType.ReloadTime)),
                perfectZoneTime / (1 + PlayerController.instance.GetModifier(ModifierType.ReloadTime)),
                perfectZoneSize, true);

            // Reload animation and audio trigger after the reload has finished
        }
    }

    protected override void OnReloadFinished()
    {
        // Play reload audio
        PlayAudio(reloadAudioClips);

        // Play reload animation
        if (reloadAnimName != "")
        {
            animator.speed = 1 / reloadDuration * (1 + PlayerController.instance.GetModifier(ModifierType.ReloadTime));
            animator.Play(reloadAnimName);
        }

        // Reset fast reload attempt
        attemptedFastReload = false;
    }

    // Called when player attempts a fast reload
    void FastReload()
    {
        // Player has attempted fast reload
        attemptedFastReload = true;

        // Get current time and check if player has successfully fast reloaded
        float currTime = Time.unscaledTime;
        if (currTime >= reloadStartTime + perfectZoneTime / (1 + PlayerController.instance.GetModifier(ModifierType.ReloadTime)) - (perfectZoneSize / 2)
            && currTime <= reloadStartTime + perfectZoneTime / (1 + PlayerController.instance.GetModifier(ModifierType.ReloadTime)) + (perfectZoneSize / 2))
        {
            // Reset mag
            currMagSize = magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize));
            MagazineUIController.instance.RefreshMagazineUI(magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)), magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)));
            
            // Reset timer
            reloadTimer = 0;

            // Set reload state
            currReloadState = reloadState.READY;

            // Call on reload finished
            OnReloadFinished();

            // Remove reload UI
            ReloadAnimation.instance.StopAnimation();
        }
        else
        {
            // Epic fail
            ReloadAnimation.instance.FailFastReload();
        }
    }
}
