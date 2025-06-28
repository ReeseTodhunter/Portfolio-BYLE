using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StrengthTotem : TotemScript
{
    public int budget, rounds;
    private float timer = 0;
    private enum scriptState 
    {
        idle,
        active,
        failed,
        success,
        inactive
    }
    private scriptState currState;
    public override void Initialise()
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        pos = transform.position;
        pos.y = 0;
        if (interactText != null)
        {
            interactText.SetActive(false);
        }
        baseText = interactText.GetComponent<TextMeshProUGUI>().text;
        currState = scriptState.idle;
    }
    public override void Activate()
    {
        if (interactText != null)
        {
            parentRoom.totemActive = true;
        }
        foreach (ParticleSystem emitter in eyeFire)
        {
            var col = emitter.colorOverLifetime;
            col.color = activeColorGradient;
            emitter.Play();
        }
        EnemySpawningSystem.instance.ActivateSpawner(rounds, EnemySpawningSystem.EnemyWaveSize.MEDIUM, EnemySpawningSystem.EnemyWaveType.ALL_ENEMIES);
        interactText.SetActive(false);
        currState = scriptState.active;
    }
    public override void DeActivate()
    {
        foreach (ParticleSystem emitter in eyeFire)
        {
            var col = emitter.colorOverLifetime;
            col.color = activeColorGradient;
            interactText.GetComponent<TextMeshProUGUI>().text = baseText;
            emitter.Stop();
        }
    }
    private void Update()
    {
        switch (currState)
        {
            case scriptState.idle:
                IdleUpdate();
                break;
            case scriptState.active:
                timer += Time.deltaTime;
                if (EnemySpawningSystem.instance.isRoomCleared() == true && EnemySpawningSystem.instance.AllEnemiesSpawned() == true && timer > 4)
                {
                    AllEnemiesDead();
                }
                break;
            case scriptState.failed:
                Debug.Log("YOU FAILED");
                CanvasScript.instance.roomClear.clearText.text = "FAILED";
                currState = scriptState.inactive;
                DeActivate();
                break;
            case scriptState.success:
                Debug.Log("YOU SUCCEEDED");

                AchievementSystem.UnlockAchievement(3);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Worthy");

                ///////////////
                int rand = Random.Range(0, potentialPowerUps.Count);
                GameObject itemInstance = Instantiate(potentialPowerUps[rand]);
                itemInstance.transform.position = itemPos.transform.position;
                itemInstance.AddComponent<Rigidbody>();
                itemInstance.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                itemInstance.AddComponent<BoxCollider>();
                itemInstance.GetComponent<BasePowerup>().RandomiseModAmount();
                //////////////

                CanvasScript.instance.roomClear.clearText.text = "SUCCEEDED";
                currState = scriptState.inactive;
                DeActivate();
                break;
            case scriptState.inactive:
                return;
        }
    }
    private void IdleUpdate()
    {
        if (Vector3.Distance(pos, PlayerController.instance.transform.position) > interactDistance)
        {
            interactText.SetActive(false);
            return;
        }
        interactText.SetActive(true);
        if (GameManager.GMinstance.GetInputDown("keyInteract"))
        {
            Activate();
        }
    }

    public override void AllEnemiesDead()
    {
        if(currState == scriptState.active)
        {
            currState = scriptState.success;
        }
        
    }

}
