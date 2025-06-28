using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class ProjectileWeapon : BaseWeapon
{
    public Transform spawnLocation;
    public GameObject projectilePrefab;
    public GameObject muzzleFlash;
    public float rateOfFire = 0.2f;
    public float speed;
    public float accel;
    public float lifetime;
    public float dmg;
    public float spreadInDegrees = 0;
    protected float baseDmg;
    public int magazineSize = 8;
    public float reloadDuration = 1;
    protected float reloadTimer = 0;
    protected int currMagSize = 8;
    [Range(0, 1)]
    public float critChance = 0.1f;
    public float critMultiplier = 1.25f;
    protected float time = 0;
    public string shootAnimName = "";
    public string reloadAnimName = "";
    public string swapToAnimName = "";
    public string idleAnimName = "";
    public bool poolObjects = false; // True if weapon uses object pooling
    protected int poolIndex = 0; // Current index in array
    public int poolSize = 5; // Size of pool
    public GameObject[] projPool; // Array containing all pooled projectiles
    protected Animator animator;
    protected reloadState currReloadState = reloadState.READY;
    protected enum reloadState
    {
        READY,
        RELOADING,
        FIRING
    }
    private void Start()
    {
        projPool = new GameObject[poolSize];
        animator = PlayerController.instance.gameObject.GetComponent<Animator>();
        currMagSize = magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize));
        baseDmg = dmg;
    }

    public override void OnFireOneHeld()
    {
        if (currReloadState != reloadState.READY) { return; }
        if (currMagSize <= 0)
        {
            OnReloadDown();
            return;
        }

        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != idleAnimName)
        {
            // Gun isnt in idle animation so it will probably fire in the wrong place. dont shoot.
            return;
        }

        time = Time.unscaledTime;
        if (shootAnimName != "")
        {
            animator.speed = 1 / (rateOfFire * (PlayerController.instance.GetModifier(ModifierType.RateOfFire)));
            animator.Play(shootAnimName);
        }
        OnShoot();
        MagazineUIController.instance.RefreshMagazineUI(magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)), currMagSize);
    }
    protected virtual void OnShoot()
    {
        GameObject projectile;

        // Object pooling
        if (poolObjects)
        {
            // If index in pool array is empty, instantiate new projectile and add it to the pool array
            if (projPool[poolIndex] == null)
            {
                projectile = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
                projPool[poolIndex] = projectile;
            }
            // If index in pool array isnt empty, reinitialise the projectile
            else
            {
                projectile = projPool[poolIndex];
                projectile.GetComponent<Projectile>().Enable(spawnLocation);
            }
            poolIndex = (poolIndex + 1) % projPool.Length;
        }

        // Just instantiate if object pooling isnt being used (only applies to katana really, and that overrides this function anyway)
        else
        {
            projectile = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
        }

        //Play weapon fire audio
        PlayAudio(fireAudioClips);

        float spread = Random.Range(-spreadInDegrees / 2, spreadInDegrees / 2);
        Vector3 angle = spawnLocation.transform.eulerAngles;
        angle.y += spread;
        projectile.transform.eulerAngles = angle;
        projectile.GetComponent<Projectile>().Init(speed * (1 + PlayerController.instance.GetModifier(ModifierType.ProjectileSpeed)), accel, lifetime,
            dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
        .SetCrit(GetIsCrit(), critMultiplier)
        .SetPierce((int)PlayerController.instance.GetModifier(ModifierType.Pierce), 1.0f);
        currMagSize--;

        //Play VFX
        if (muzzleFlash == null) { return; }
        VisualEffect vfx;
        if (muzzleFlash.TryGetComponent<VisualEffect>(out vfx))
        {
            vfx.Play();
        }
        ParticleSystem pSystem;
        if (muzzleFlash.TryGetComponent<ParticleSystem>(out pSystem))
        {
            pSystem.Play();
        }

        currReloadState = reloadState.FIRING;

        return;
    }
    public override void OnReloadDown()
    {
        // Dont reload if mag is full or already reloading
        if (currMagSize == magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize))) { return; }
        if (currReloadState == reloadState.RELOADING) { return; }

        //Play weapon reload audio
        PlayAudio(reloadAudioClips);

        ReloadAnimation.instance.BeginAnimation(reloadDuration / (1 + PlayerController.instance.GetModifier(ModifierType.ReloadTime)), 0, 0, false);
        reloadTimer = 0;
        currReloadState = reloadState.RELOADING;
        if (reloadAnimName != "")
        {
            animator.speed = 1 / reloadDuration * (1 + PlayerController.instance.GetModifier(ModifierType.ReloadTime));
            animator.Play(reloadAnimName);
        }
    }
    public override void OnPickup()
    {
        base.OnPickup();
        animator.Play(swapToAnimName);
        MagazineUIController.instance.RefreshMagazineUI(magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)), currMagSize);
    }
    public override void OnEquip()
    {
        base.OnEquip();
        animator.Play(swapToAnimName);
        MagazineUIController.instance.RefreshMagazineUI(magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)), currMagSize);
    }
    public override void OnDrop()
    {
        if (poolObjects)
        {
            // Clear out object pool list
            foreach (GameObject projectile in projPool)
            {
                Destroy(projectile);
            }
            projPool = new GameObject[poolSize];
        }
        currReloadState = reloadState.READY;
        base.OnDrop();
    }
    public override void OnStow()
    {
        // Cancel reload
        currReloadState = reloadState.READY;
        reloadTimer = 0;
        ReloadAnimation.instance.StopAnimation();

        base.OnStow();
    }
    private void Update()
    {
        switch (currState)
        {
            case WeaponState.DROPPED:
                Idle();
                break;
            case WeaponState.EQUIPPED:
                UpdateProjectileWeapon();
                break;
        }
    }
    protected void UpdateProjectileWeapon()
    {
        switch (currReloadState)
        {
            case reloadState.RELOADING:
                reloadTimer += Time.unscaledDeltaTime * (1 + PlayerController.instance.GetModifier(ModifierType.ReloadTime));
                if (reloadTimer < reloadDuration) { break; }
                currMagSize = magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize));
                MagazineUIController.instance.RefreshMagazineUI(magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)), magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize)));
                reloadTimer = 0;
                currReloadState = reloadState.READY;
                OnReloadFinished();
                break;
            case reloadState.READY:
                if (animator.GetCurrentAnimatorClipInfo(0).Length == 0) return;
                if (currMagSize <= 0 && (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == idleAnimName || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Idle")))
                {
                    OnReloadDown();
                }
                break;
            case reloadState.FIRING:
                if (Time.unscaledTime - time > rateOfFire * PlayerController.instance.GetModifier(ModifierType.RateOfFire))
                {
                    currReloadState = reloadState.READY;
                }
                break;
            default:
                return;
        }
    }
    protected virtual void OnReloadFinished()
    {
    }
    public override string GetSwapAnimName()
    {
        return swapToAnimName;
    }
    public override string GetIdleAnimName()
    {
        return idleAnimName;
    }
    protected bool GetIsCrit()
    {
        float rnd = Random.Range(1, 100);
        rnd /= 100;
        if (rnd < critChance + PlayerController.instance.GetModifier(ModifierType.CritChance))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetMagazineSize(int mag)
    {
        magazineSize = mag;
    }

}
