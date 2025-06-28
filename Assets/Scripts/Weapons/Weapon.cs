using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
public class Weapon : MonoBehaviour
{
    // Gun
    [SerializeField] private GameObject projectile;    // Projectile fired by gun
    [SerializeField] private GameObject laser;         // Laser to fire from gun
    [SerializeField] private Transform barrelOffset;   // Offset for spawning the projectile

    // Weapon stats
    [SerializeField] private float rateOfFire = 0.1f;  // Fire rate of weapon
    [SerializeField] private float projectileSpread;   // Angle of projectiles spread (only matters if number of projectiles is >1)
    [SerializeField] private float projectileSpeed;    // How fast projectiles move
    [SerializeField] private float projectileAccel;    // Acceleration of projectiles
    [SerializeField] private float projectileLifetime; // How long projectiles will exist after being fired
    [SerializeField] private float projectileDamage;   // How much damage each projectile does
    [SerializeField] private int numProjectiles = 1;   // Amount of projectiles fired by weapon
    [SerializeField] private float projectileBloom = 5.0f; // How far the projectiles can deviate from their original path. (if 0 the weapon has perfect aim)
    [SerializeField] private bool shakeCamera = true;
    [SerializeField] private bool bottomlessClip = false;

    [SerializeField] private int baseMagazineSize = 6;
    int magazineSize = 6;
    [SerializeField] private float baseReloadDuration = 2; // Default reload timer before modifiers
    float reloadDuration = 2; // Reload time after modifiers
    [SerializeField] private float basePerfectReloadTime = 1;
    float perfectReloadTime = 1;
    [SerializeField] private float basePerfectReloadForgiveness = 0.2f;
    float perfectReloadForgiveness = 0.2f;
    [SerializeField] Sprite UISprite; // Weapon sprite that appears on the HUD when weapon is equipped

    // Modifiers
    float bloomModifier = 1.0f;
    float damageModifier = 1.0f;
    float rofModifier = 1.0f;
    float speedModifier = 1.0f;
    float sizeModifier = 1.0f;
    float reloadSpeedModifier = 1.0f;
    float reloadForgivenessModifier = 1.0f;
    float magSizeModifier = 1.0f;
    private int currMagazineSize = 6;
    private int previousMagazineSize;
    private float reloadTimer = 0;
    [SerializeField] private bool isEquipped = false;

    //Timer for between shots
    private float timer = 0.0f;
    
    //Raytrace weapons
    [SerializeField] private bool raytrace = false;
    private GameObject spawnedLaser;

    private float bobSpeed = 1.0f;
    private bool isReloading = false;
    public bool reloadFailed = false;
    private bool canFastReload = false;
    public bool usesVFX = false;
    private VisualEffect vfx;


    void Awake()
    {
        magazineSize = (int)(baseMagazineSize * magSizeModifier);
        currMagazineSize = magazineSize;
        if (laser != null)
        {
            spawnedLaser = Instantiate(laser, barrelOffset);
            spawnedLaser.SetActive(false);
        }
    }
    void Start()
    {
        if(usesVFX){vfx = barrelOffset.gameObject.GetComponent<VisualEffect>();}
    }
    private void Update()
    {
        if (!isEquipped)
        {
            Idle();
        }
        if (timer > 0)
        {
            timer -= Time.unscaledDeltaTime * rofModifier;
        }
        if(isReloading)
        {
            reloadTimer += Time.unscaledDeltaTime;
            if(reloadTimer >= reloadDuration)
            {
                reloadTimer = 0;
                isReloading = false;
                currMagazineSize = magazineSize;
                //MagazineUIController.instance.RefreshMagazineUI();
                canFastReload = true;
                ReloadAnimation.instance.StopAnimation();
            }
        }

        if (PlayerController.instance.GetMainWeapon() == this)
        {
            if (spawnedLaser != null && raytrace)
            {
                if (currMagazineSize > 0)
                {
                    spawnedLaser.SetActive(GameManager.GMinstance.GetInput("keyShoot1"));
                }
                else
                {
                    spawnedLaser.SetActive(false);
                }
            }
            else if (raytrace && laser != null)
            {
                spawnedLaser = Instantiate(laser, barrelOffset);
                spawnedLaser.SetActive(GameManager.GMinstance.GetInput("keyShoot1"));
            }
        }
        
    }

    public void AttemptReloadWeapon()
    {
        //Begin reloading
        if(!isReloading)
        {
            reloadDuration = baseReloadDuration * reloadSpeedModifier;
            perfectReloadForgiveness = basePerfectReloadForgiveness * reloadForgivenessModifier;
            perfectReloadTime = (basePerfectReloadTime / baseReloadDuration) * reloadDuration;
            ReloadAnimation.instance.BeginAnimation(reloadDuration, perfectReloadTime, perfectReloadForgiveness);
            reloadTimer = 0;
            isReloading = true;
            canFastReload = true;
            return;
        }
        if(canFastReload)
        {
            if(Mathf.Abs(reloadTimer - perfectReloadTime) <= perfectReloadForgiveness/2)
            {
                reloadTimer = reloadDuration;
                ReloadAnimation.instance.StopAnimation();
               // MagazineUIController.instance.RefreshMagazineUI();
            }
            else
            {
                ReloadAnimation.instance.FailFastReload();
                canFastReload = false;
            }
        }

    }
    private void Idle()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + (50 * Time.unscaledDeltaTime), 0);

        if (transform.position.y >= 1.2f)
        {
            bobSpeed = -0.5f;
        }
        else if (transform.position.y <= 0.8f)
        {
            bobSpeed = 0.5f;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y + (bobSpeed * Time.unscaledDeltaTime), transform.position.z);
    }

    public void Fire(string a_tag)
    {
        if(isReloading)
        {
            return;
        }
        if(currMagazineSize <= 0)
        {
            if(!isReloading)
            {
                AttemptReloadWeapon();
            }
            return;
        }
        //Fire Projectiles
        if (!raytrace && projectile != null && timer <= 0)
        {
            if(usesVFX){vfx.Play();}
            if (numProjectiles > 1)
            {
                float angleSplit = projectileSpread / numProjectiles;
                currMagazineSize--;
                for (int i = 0; i < numProjectiles; i++)
                {
                    GameObject firedProjectile = Instantiate(projectile, barrelOffset.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y - (projectileSpread / 2) + (angleSplit / 2) + (angleSplit * i) + (projectileBloom*bloomModifier*Random.Range(-1.0f, 1.0f)), 0));
                    firedProjectile.GetComponent<IProjectile>().Init(projectileSpeed * speedModifier, projectileAccel, projectileLifetime, projectileDamage * damageModifier, a_tag);
                    firedProjectile.transform.localScale *= sizeModifier;
                    if(shakeCamera){CameraController.instance.ShakeCamera();}
                    //MagazineUIController.instance.RefreshMagazineUI();
                }
            }
            else
            {
                GameObject firedProjectile = Instantiate(projectile, barrelOffset.position, Quaternion.Euler(0, transform.rotation.eulerAngles.y + (projectileBloom*bloomModifier*Random.Range(-1.0f, 1.0f)), 0));
                firedProjectile.GetComponent<IProjectile>().Init(projectileSpeed * speedModifier, projectileAccel, projectileLifetime, projectileDamage * damageModifier, a_tag);
                firedProjectile.transform.localScale *= sizeModifier;
                if (shakeCamera){CameraController.instance.ShakeCamera();}
                currMagazineSize--;
                //MagazineUIController.instance.RefreshMagazineUI();
            }
            timer = rateOfFire;
        }
        else if (raytrace && timer <= 0)
        {

            // Cast the ray from the mouse position out into the scene if hitting the ground layer turning the model to face the collision point
            if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, Mathf.Infinity,LayerMask.GetMask("Untraversable", "Player", "Enemy", "Hurtbox", "SeeThrough"), QueryTriggerInteraction.Ignore))
            {
                Vector3 endPoint = new Vector3(0, 0, Vector3.Distance(barrelOffset.position, hitInfo.point));

                if (hitInfo.collider.TryGetComponent(out Character c))
                {
                    c.Damage(projectileDamage * damageModifier);
                }

                spawnedLaser.GetComponent<LineRenderer>().SetPosition(1, endPoint);
            }
            currMagazineSize--;
            //MagazineUIController.instance.RefreshMagazineUI();
            timer = rateOfFire;
        }
    }

    public void CancelReload()
    {
        isReloading = false;
        canFastReload = true;
        ReloadAnimation.instance.StopAnimation();
    }

    // Public get/setters
    public void SetEquipped(bool equip)
    {
        isEquipped = equip;
        GetComponent<BoxCollider>().enabled = !equip;
    }
    public bool IsEquipped()
    {
        return isEquipped;
    }
    public Sprite GetSprite()
    {
        return UISprite;
    }
    public int GetMagazineSize()
    {
        return magazineSize;
    }

    public void SetMagazineSize(int mag)
    {
        baseMagazineSize = mag;
        magazineSize = mag;
        
    }
    public int GetCurrentMagazineSize()
    {
        return currMagazineSize;
    }
    public bool IsReloading()
    {
        return isReloading;
    }
    public void SetModifiers()
    {
        if (PlayerController.instance != null)
        {
            /*
            bloomModifier =     Mathf.Max(0.0f, PlayerController.instance.GetMultiplicativeModifier(ModifierType.Accuracy));
            damageModifier =    Mathf.Max(0.0f, 1.0f + PlayerController.instance.GetAdditiveModifier(ModifierType.Damage));
            rofModifier =       Mathf.Max(0.0f, PlayerController.instance.GetMultiplicativeModifier(ModifierType.RateOfFire));
            speedModifier =     Mathf.Max(0.0f, PlayerController.instance.GetMultiplicativeModifier(ModifierType.ProjectileSpeed));
            sizeModifier =      Mathf.Max(0.0f, 1.0f + PlayerController.instance.GetAdditiveModifier(ModifierType.ProjectileSize));
            reloadSpeedModifier = Mathf.Max(0.1f, PlayerController.instance.GetMultiplicativeModifier(ModifierType.ReloadTime));
            reloadForgivenessModifier = Mathf.Max(0.0f, 1.0f + PlayerController.instance.GetAdditiveModifier(ModifierType.ReloadForgiveness));
            magSizeModifier =   Mathf.Max(0.0f, 1.0f + PlayerController.instance.GetAdditiveModifier(ModifierType.ClipSize));*/

            int oldMagazineSize = magazineSize;
            magazineSize = (int)(baseMagazineSize * magSizeModifier);
            if (oldMagazineSize < magazineSize || currMagazineSize > magazineSize)
            {
                currMagazineSize = magazineSize; // Reload player when they get more clip size or current clip size suddenly goes over max
            }
        }
    }
    public GameObject GetProjectile()
    {
        return projectile;
    }
}