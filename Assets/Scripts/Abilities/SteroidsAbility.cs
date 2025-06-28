using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteroidsAbility : BaseActivePowerup
{
    [SerializeField] GameObject punchProjectile;

    [SerializeField] float length = 10.0f;
    [SerializeField] float punchLength = 0.333f;
    [SerializeField] float damage = 5.0f;
    [SerializeField] float resistance = -0.6f;

    float timer;
    float punchTimer;

    string currentPunchAnim = "BuffPunchR"; // Animation that plays when the player punches
    float punchProjFlip = -1.0f; // Variable used to flip punch projectile offset and model

    GameObject normalPlayerModel;
    GameObject buffPlayerModel;

    public override float UseAbility()
    {
        // Disable weapons & player model
        PlayerController.instance.EnableWeapons(false);
        normalPlayerModel = PlayerController.instance.transform.GetChild(0).gameObject;
        normalPlayerModel.SetActive(false);

        // Enable buff player and parent powerup to it
        buffPlayerModel = PlayerController.instance.transform.GetChild(1).gameObject;
        buffPlayerModel.SetActive(true);
        transform.parent = buffPlayerModel.transform;

        // 90% damage resistance
        PlayerController.instance.AddModifier(ModifierType.Vulnerability, resistance);

        // Start timer
        timer = length;

        return base.UseAbility();
    }

    private void Update()
    {
        // When ability is active
        if (timer > 0.0f)
        {
            // Decrease timer
            timer -= Time.unscaledDeltaTime;
            punchTimer -= Time.unscaledDeltaTime;

            // Play sound effect and camera shake when walking
            // Took this out because I felt the actual punching part of the ability would be better suited for the sound effects rather than walking around with non-existent legs
            /* if (PlayerController.instance.GetComponent<Rigidbody>().velocity != Vector3.zero && !buffPlayerModel.GetComponent<AudioSource>().isPlaying)
            {
                buffPlayerModel.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f) * (1 + PlayerController.instance.GetModifier(ModifierType.Speed));
                buffPlayerModel.GetComponent<AudioSource>().Play();

                CameraController.instance.ShakeCameraOverTime(0.2f, 1.5f);
            } */

            // PUNCHING
            if ((GameManager.GMinstance.GetInput("keyShoot1") || GameManager.GMinstance.GetInput("keyShoot2")) && punchTimer < 0.0f && !PlayerController.instance.IsRolling())
            {
                // Animation - Swaps between left and right arm punches
                if (currentPunchAnim == "BuffPunchL") currentPunchAnim = "BuffPunchR";
                else currentPunchAnim = "BuffPunchL";
                PlayerController.instance.GetComponent<Animator>().speed = 1 / punchLength;
                PlayerController.instance.GetComponent<Animator>().Play(currentPunchAnim);
                punchProjFlip *= -1.0f;

                // Punch projectile
                GameObject punch = Instantiate(punchProjectile, PlayerController.instance.transform.position + (PlayerController.instance.transform.rotation * ((Vector3.forward * 2.5f) + (Vector3.left * punchProjFlip) + Vector3.up)), PlayerController.instance.transform.rotation);
                punch.GetComponent<Projectile>().Init(20.0f, -20.0f, 1.0f, damage, PlayerController.instance.gameObject, false).SetPierce(-1, 1.0f);
                punch.transform.GetChild(0).transform.localScale = new Vector3(punchProjFlip, 1.0f, 1.0f); // Flipping model

                // Camera shake
                CameraController.instance.ShakeCameraOverTime(0.2f, 1.5f);

                // Play sound effect
                // Gets higher pitched in the last 2 seconds to warn player the ability is ending
                buffPlayerModel.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f) * Mathf.Clamp(2.0f - (timer/2.0f), 1.0f, 2.0f);
                buffPlayerModel.GetComponent<AudioSource>().Play();

                // Reset timer
                punchTimer = punchLength;
            }

            // When ability has ended
            if (timer <= 0.0f)
            {
                // Re-enable weapons & player model
                PlayerController.instance.EnableWeapons(true);
                normalPlayerModel.SetActive(true);

                // Disable buff player and parent powerup back to normal model
                buffPlayerModel.SetActive(false);
                transform.parent = normalPlayerModel.transform;

                // 90% damage resistance end
                PlayerController.instance.RemoveModifier(ModifierType.Vulnerability, resistance);

                // Player idle anim
                PlayerController.instance.GetComponent<Animator>().Play(PlayerController.instance.GetMainWeapon().GetIdleAnimName());
            }
        }

        base.Update();
    }
}
