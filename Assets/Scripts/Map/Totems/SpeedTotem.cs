using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SpeedTotem : TotemScript
{
    private float timer = 0;
    public float duration = 30;
    private enum scriptState
    {
        idle,
        countDown,
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
        interactText.SetActive(false);
        baseText = interactText.GetComponent<TextMeshProUGUI>().text;
        timer = duration;
        currState = scriptState.idle;
    }
    public override void Activate()
    {
        parentRoom.totemActive = true;
        timer = duration;
        foreach (ParticleSystem emitter in eyeFire)
        {
            var col = emitter.colorOverLifetime;
            col.color = activeColorGradient;
            emitter.Play();
        }
        EnemySpawningSystem.instance.ActivateSpawner(1, EnemySpawningSystem.EnemyWaveSize.SMALL, EnemySpawningSystem.EnemyWaveType.ALL_ENEMIES);
        currState = scriptState.countDown;
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
    void Update()
    {
        switch (currState)
        {
            case scriptState.idle:
                IdleUpdate();
                break;
            case scriptState.countDown:
                countDownUpdate();
                break;
            case scriptState.failed:
                Debug.Log("YOU FAILED!");
                CanvasScript.instance.roomClear.clearText.text = "FAILED";
                interactText.SetActive(false);
                DeActivate();
                currState = scriptState.inactive;
                break;
            case scriptState.success:
                Debug.Log("YOU SUCCEEDED!  SPEED");

                AchievementSystem.UnlockAchievement(3);
                AchievementSystem.Init();
                SaveLoadSystem.instance.SaveAchievements();
                GameManager.GMinstance.UnlockSteamAchievement("Worthy");

                ////////////////
                int rand = Random.Range(0, potentialPowerUps.Count);
                GameObject itemInstance = Instantiate(potentialPowerUps[rand]);
                itemInstance.transform.position = itemPos.transform.position;
                itemInstance.AddComponent<Rigidbody>();
                itemInstance.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                itemInstance.AddComponent<BoxCollider>();
                itemInstance.GetComponent<BasePowerup>().RandomiseModAmount();
                ////////////////
                CanvasScript.instance.roomClear.clearText.text = "SUCCEEDED";
                interactText.SetActive(false);
                DeActivate();
                currState = scriptState.inactive;
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
    private void countDownUpdate()
    {
        timer -= Time.deltaTime;
        interactText.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(timer).ToString();
        if (timer <= 0)
        {
            EnemySpawningSystem.instance.KillAllEnemies();
            DeActivate();
            interactText.SetActive(false);
            currState = scriptState.failed;
        }
        if(EnemySpawningSystem.instance.isRoomCleared() == true && timer > 0 && timer < 26)
        {
            AllEnemiesDead();
        }
    }
    public override void AllEnemiesDead()
    {
        if (currState == scriptState.countDown)
        {
            currState = scriptState.success;
        }
    }
}
