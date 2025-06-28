using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TetherWeapon : ProjectileWeapon
{
    public GameObject tetherProj1 = null;
    public GameObject tetherProj2 = null;

    private GameObject currentProjectile;

    public int burstSize;
    public float burstInterval;
    private bool isShooting = false;
    private int currBurstShot = 0;
    private float burstTimer = 0;



    public float tickRate = 5;
    private float tickTimer = 0;
    private bool canDamge = true;

    public GameObject laserPrefab;
    private GameObject laserObject;

    private void Start()
    {
        projPool = new GameObject[poolSize];
        animator = PlayerController.instance.gameObject.GetComponent<Animator>();
        currMagSize = magazineSize + (int)(magazineSize * PlayerController.instance.GetModifier(ModifierType.ClipSize));
        baseDmg = dmg;

        laserObject = GameObject.Instantiate(laserPrefab, spawnLocation.position, spawnLocation.rotation);
        laserObject.SetActive(true);
        laserObject.transform.parent = spawnLocation;
    }

    protected override void OnShoot()
    {
        if (isShooting) { return; }
        isShooting = true;
        currBurstShot = 0;
        burstTimer = 0;


        if (tetherProj1 == null)
        {
            // Object pooling
            if (poolObjects)
            {
                // If index in pool array is empty, instantiate new projectile and add it to the pool array
                if (projPool[poolIndex] == null)
                {
                    tetherProj1 = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
                    projPool[poolIndex] = tetherProj1;
                }
                // If index in pool array isnt empty, reinitialise the projectile
                else
                {
                    tetherProj1 = projPool[poolIndex];
                    tetherProj1.GetComponent<Projectile>().Enable(spawnLocation);
                }
                poolIndex = (poolIndex + 1) % projPool.Length;
            }
            // Just instantiate if object pooling isnt being used (only applies to katana really, and that overrides this function anyway)
            else
            {
                tetherProj1 = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
            }


            //Play weapon fire audio
            PlayAudio(fireAudioClips);

            float spread = Random.Range(-spreadInDegrees / 2, spreadInDegrees / 2);
            Vector3 angle = spawnLocation.transform.eulerAngles;
            angle.y += spread;
            tetherProj1.transform.eulerAngles = angle;
            tetherProj1.GetComponent<Projectile>().Init(speed * (1), accel, lifetime,
                dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
            .SetCrit(GetIsCrit(), critMultiplier)
            .SetPierce(-1, 0.0f);
            currMagSize--;

            if (tetherProj1.GetComponent<TetherProjectile>() != null)
            {
                tetherProj1.GetComponent<TetherProjectile>().SetInitialPos(spawnLocation.gameObject);
                tetherProj1.GetComponent<TetherProjectile>().count = 1;
            }


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
        }
        else
        {
            // Object pooling
            if (poolObjects)
            {
                // If index in pool array is empty, instantiate new projectile and add it to the pool array
                if (projPool[poolIndex] == null)
                {
                    tetherProj2 = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
                    projPool[poolIndex] = tetherProj2;
                }
                // If index in pool array isnt empty, reinitialise the projectile
                else
                {
                    tetherProj2 = projPool[poolIndex];
                    tetherProj2.GetComponent<Projectile>().Enable(spawnLocation);
                }
                poolIndex = (poolIndex + 1) % projPool.Length;
            }
            // Just instantiate if object pooling isnt being used (only applies to katana really, and that overrides this function anyway)
            else
            {
                tetherProj2 = Instantiate(projectilePrefab, spawnLocation.position, spawnLocation.rotation);
            }

            //Play weapon fire audio
            PlayAudio(fireAudioClips);

            float spread = Random.Range(-spreadInDegrees / 2, spreadInDegrees / 2);
            Vector3 angle = spawnLocation.transform.eulerAngles;
            angle.y += spread;
            tetherProj2.transform.eulerAngles = angle;
            tetherProj2.GetComponent<Projectile>().Init(speed * (1), accel, lifetime,
                dmg * (1 + PlayerController.instance.GetModifier(ModifierType.Damage)), PlayerController.instance.gameObject, poolObjects)
            .SetCrit(GetIsCrit(), critMultiplier)
            .SetPierce(-1, 0.0f);
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

            if(tetherProj1 != null)
            {
                if(tetherProj2.GetComponent<TetherProjectile>() != null)
                {
                    tetherProj2.GetComponent<TetherProjectile>().count = 2;
                    tetherProj1.GetComponent<TetherProjectile>().SetTetherPartner(tetherProj2);

                    // THIS IS RELEVANT ITS ONLY COMMENTED FOR TESATING
                    tetherProj1 = null;
                }
               
            }

            isShooting = false;
            currReloadState = reloadState.FIRING;
        }
        

        return;
    }

    void Update()
    {
        switch (currState)
        {
            case WeaponState.DROPPED:
                Idle();
                break;
            case WeaponState.STOWED:
                if (tetherProj2 == null)
                {
                    Destroy(tetherProj1);
                }
                isShooting = false;
                currBurstShot = 0;
                burstTimer = 0;
                break;
            case WeaponState.EQUIPPED:
                UpdateProjectileWeapon();
                if (!isShooting) { return; }
                if (currBurstShot >= burstSize) { isShooting = false; return; }
                burstTimer += Time.deltaTime;
                if (burstTimer < burstInterval) { return; }
                burstTimer = 0;
                currBurstShot++;
                isShooting = false;
                OnShoot();
                break;
        }

        GetLaserEndPos();
        tickTimer += Time.unscaledDeltaTime;
        if (tickTimer < 1 / tickRate) { canDamge = false; return; }
        canDamge = true;
        tickTimer = 0;
    }

    public override void OnReloadDown()
    {
        base.OnReloadDown();
        isShooting = false;
    }


    private void GetLaserEndPos()
    {
        if(tetherProj1 == null || tetherProj2 == null)
        {
            return;
        }

        RaycastHit hit;
        Vector3 endPos;
        if (Physics.Raycast(tetherProj1.transform.position, tetherProj2.transform.position - tetherProj1.transform.position, out hit, 100, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Ignore))
        {
            endPos = hit.point;

            if (hit.collider.gameObject.tag != "Player" && canDamge)
            {
                if (hit.collider.gameObject.TryGetComponent<Character>(out Character _character))
                {
                    float _dmg = 2;


                    _dmg *= 1 + PlayerController.instance.GetModifier(ModifierType.Damage);
                    _character.Damage(_dmg / tickRate, true, false, false, EffectType.None);
                    tickTimer = 0;
                    canDamge = false;
                }
            }
        }
        else
        {
            endPos = tetherProj2.transform.position;
        }
        laserObject.GetComponent<LineRenderer>().SetPosition(1, endPos);
        laserObject.GetComponent<Animator>().Play("Active");
    }
}
