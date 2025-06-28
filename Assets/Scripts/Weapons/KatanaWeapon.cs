using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class KatanaWeapon : ProjectileWeapon
{
    private float slashTime = 0;
    protected override void OnShoot()
    {
        Vector3 pos = PlayerController.instance.transform.position + PlayerController.instance.transform.forward * 3;
        GameObject projectile = GameObject.Instantiate(projectilePrefab, pos, spawnLocation.rotation);
        if(spawnLocation.gameObject.TryGetComponent<VisualEffect>(out VisualEffect fx))
        {
            fx.Play();
        }
        //Play weapon fire audio
        PlayAudio(fireAudioClips);
        float spread = Random.Range(-spreadInDegrees/2,spreadInDegrees/2);
        Vector3 angle = spawnLocation.transform.eulerAngles;
        angle.y += spread;
        projectile.transform.eulerAngles = angle;
        projectile.GetComponent<Projectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject)
        .SetCrit(GetIsCrit(),critMultiplier)
        .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f);
        projectile.transform.rotation = PlayerController.instance.transform.rotation;
        return;
    }
    public override void OnFireOneHeld()
    {
        if(Time.unscaledTime - slashTime < rateOfFire * (PlayerController.instance.GetModifier(ModifierType.RateOfFire))) {return;}
        slashTime = Time.unscaledTime;
        Animator animator = PlayerController.instance.GetComponent<Animator>();
        if (shootAnimName != "")
        {
            animator.speed = 1 / rateOfFire * (PlayerController.instance.GetModifier(ModifierType.RateOfFire)) * 2;
            animator.Play(shootAnimName);
        }
        OnShoot();
        MagazineUIController.instance.RefreshMagazineUI(magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)), currMagSize);
    }
    public override void OnFireOneDown()
    {
        return;
    }
    public override void OnReloadDown()
    {
        return;
    }
    public override void OnReloadHeld()
    {
        return;
    }
}
