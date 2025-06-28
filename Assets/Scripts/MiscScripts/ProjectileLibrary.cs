using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLibrary : MonoBehaviour
{
    /*
        Class that contains all projectiles used in the game. 
        This allows any script to get a reference for any projectile
        it needs. 
    */
    
    public static ProjectileLibrary instance;
    [Header("Prefabs")]
    public GameObject basicProjectilePrefab;
    public GameObject sniperProjectilePrefab;
    public GameObject cultistProjectilePrefab;
    public GameObject nurseProjectilePrefab;
    public GameObject duplicatorProjectilePrefab;
    public GameObject gandlerProjectilePrefab;
    public GameObject SlimePuddlePrefab;
    public GameObject BYLEProjectilePrefab;
    public GameObject floatingText;
    public GameObject genericPlayerBullet;
    public GameObject molotovPrefab;
    public GameObject itemDescriptionBox;
    public GameObject byleExplosion;
    [Space(6),Header("Mats")]
    public Material laserMat;
    public Material slimeBallMat;
    public Material basicMat;
    public Material sniperMat;
    public Material shotgunMat;
    [Space(6), Header("Effects")]
    public GameObject muzzleFlash;
    public GameObject bulletImpact;
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        instance = this;
    }
    public GameObject GetProjectile(Projectiles _projectile)
    {
        switch(_projectile)
        {
            case Projectiles.BASIC_PROJECTILE:
                return basicProjectilePrefab;
            case Projectiles.SNIPER_PROJECTILE:
                return sniperProjectilePrefab;
            case Projectiles.CULTIST_PROJECTILE:
                return cultistProjectilePrefab;
            case Projectiles.NURSE_PROJECTILE:
                return nurseProjectilePrefab;
            case Projectiles.DUPLICATOR_PROJECTILE:
                return duplicatorProjectilePrefab;
            case Projectiles.GANDLER_PROJECTILE:
                return gandlerProjectilePrefab;
            case Projectiles.SLIME_PUDDLE:
                return SlimePuddlePrefab;
            case Projectiles.BYLE_PROJECTILE:
                return BYLEProjectilePrefab;
            case Projectiles.FLOATING_TEXT:
                return floatingText;
            case Projectiles.GENERIC_PLAYER_BULLET:
                return genericPlayerBullet;
            case Projectiles.MOLOTOV_PREFAB:
                return molotovPrefab;
            case Projectiles.ITEM_DESC_BOX:
                return itemDescriptionBox;
            case Projectiles.BYLE_EXPLOSION:
                return byleExplosion;
            default:
                Debug.Log("Projectile not found, pretty cringe");
                return null;
        }
    }
    public Material GetMaterial(ProjectileMats _mat)
    {
        switch(_mat)
        {
            case ProjectileMats.LASERPOINTER_MAT:
                return laserMat;
            case ProjectileMats.SLIMEBALL_MAT:
                return slimeBallMat;
            case ProjectileMats.BASIC_MAT:
                return basicMat;
            case ProjectileMats.SNIPER_MAT:
                return sniperMat;
            case ProjectileMats.SHOTGUN_MAT:
                return shotgunMat;
            default:
                Debug.Log("mat not found, kind cringe");
                return null;
        }
    }
    public GameObject GetEffect(ProjectileEffects _effect)
    {
        switch(_effect)
        {
            case ProjectileEffects.MUZZLE_FLASH:
                return muzzleFlash;
            case ProjectileEffects.BULLET_IMPACT:
                return bulletImpact;
            default:
                return null;
        }
    }
}
public enum Projectiles
{
    BASIC_PROJECTILE,
    SNIPER_PROJECTILE,
    CULTIST_PROJECTILE,
    NURSE_PROJECTILE,
    DUPLICATOR_PROJECTILE,
    GANDLER_PROJECTILE,
    SLIME_PUDDLE,
    BYLE_PROJECTILE,
    FLOATING_TEXT,
    GENERIC_PLAYER_BULLET,
    MOLOTOV_PREFAB,
    ITEM_DESC_BOX,
    BYLE_EXPLOSION
}
public enum ProjectileMats
{
    LASERPOINTER_MAT,
    SLIMEBALL_MAT,
    BASIC_MAT,
    SNIPER_MAT,
    SHOTGUN_MAT
}
public enum ProjectileEffects
{
    MUZZLE_FLASH,
    BULLET_IMPACT
}
