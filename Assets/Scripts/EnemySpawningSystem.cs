using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawningSystem : MonoBehaviour
{
    #region Variables
    [Space(6)]
    [Header("-=Enemy prefabs=-")]
    public List<EnemySpawningProfile> EnemyProfiles = new List<EnemySpawningProfile>();
    public EnemySpawningProfile fillerEnemy;
    [Header("-----------------------------------------------------")]
    [Space(6)]
    [Header("-=General Settings=-")]
    [Min(3)]
    public int maxEnemyTypesPerWave = 3;
    public float delayBetweenSpawns = 0.25f;
    public float minDistanceFromPlayerToSpawn = 5;
    public float maxDistanceFromPlayerToSpawn = 25;
    private float delayTimer = 0;
    private int currProfileIndex = 0;

    [Header("-----------------------------------------------------")]
    [Space(6)]
    [Header("-=Difficulty Scaling Settings=-")]
    public int maxDifficulty = 5;
    public int startingDifficulty = 1;
    private int difficulty = 0;
    public int difficultyIncreasePerRoom = 1;
    [Header("-----------------------------------------------------")]
    [Space(6)]
    [Header("-=Elite Enemy Settings=-")]
    public int minRoomsClearedToSpawnElites = 3;
    public List<BTModifier> eliteModifiers = new List<BTModifier>();
    [Range(1,5)]
    public int maxModifiersPerElite = 1;
    [Range(0,1)]
    public float EliteSpawnChance = 0.2f;
    [Range(1,3)]
    public float EliteHealthMultiplier = 2;
    [Range(12,20)]
    public float maxEliteHealth = 16;
    public GameObject eliteEyePrefab;
    [Header("-----------------------------------------------------")]
    [Space(6)]
    [Header("-=Test Settings=-")]
    public EnemyWaveSize testWaveSize;
    public EnemyWaveType testWaveType;
    public int testWaveCount = 2;
    public bool spawnOnAwake = false;
    public int roomsCleared = 0;
    public enum EnemyWaveSize
    {
        SMALL,
        MEDIUM,
        LARGE
    }
    public enum EnemyWaveType
    {
        EASY_ENEMIES,
        MEDIUM_ENEMIES,
        HARD_ENEMIES,
        ALL_ENEMIES,
        RANDOM
    }
    private enum SpawnerState
    {
        inactive,
        generatingWave,
        spawningEnemies,
        waveActive,
        wavesCompleted
    }
    public enum EnemyType
    {
        EASY,
        MEDIUM,
        HARD,
        UNIQUE
    }
    private SpawnerState currState;
    private int WaveCount, currWave;
    private int waveBudget, currBudget;
    private EnemyWaveSize waveSize;
    private EnemyWaveType waveType;
    private List<BTAgent> subscribedEnemies = new List<BTAgent>();
    private List<Vector3> spawns = new List<Vector3>();
    #endregion
    #region Singleton Pattern
    public static EnemySpawningSystem instance;
    private List<int> 
    easyProfiles = new List<int>(),
    mediumProfiles = new List<int>(),
    HardProfiles = new List<int>();
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
        currState = SpawnerState.inactive;
        roomsCleared = 0;
    }
    #endregion
    private void Start()
    {
        ResetProfilePools(spawnOnAwake);
        ResetProfileCounts();
        difficulty = startingDifficulty;
        if(spawnOnAwake)
        {
            difficulty = 5;
            roomsCleared = 999;
            ActivateSpawner(testWaveCount,testWaveSize,testWaveType);
        }
    }
    private void ResetProfilePools(bool _unlockAll = false)
    {
        easyProfiles.Clear();
        mediumProfiles.Clear();
        HardProfiles.Clear();
        for(int i = 0; i < EnemyProfiles.Count; i++)
        {
            if(!_unlockAll)
            {
                if(EnemyProfiles[i].minRoomsClearedToSpawn > roomsCleared){continue;}
                if(GameController.instance != null)
                {
                    if(EnemyProfiles[i].minLevelToSpawn > GameManager.GMinstance.level){continue;}
                }
            }
            switch(EnemyProfiles[i].enemyType)
            {
                case EnemyType.EASY:
                    easyProfiles.Add(i);
                    break;
                case EnemyType.MEDIUM:
                    mediumProfiles.Add(i);
                    break;
                case EnemyType.HARD:
                    HardProfiles.Add(i);
                    break;
            }
        }
    }
    public void ActivateSpawner(int _waveCount, EnemyWaveSize _waveSize, EnemyWaveType _waveType)
    {
        if(currState != SpawnerState.inactive && currState != SpawnerState.wavesCompleted){Debug.Log("Error: Wave already in progress"); return;}
        WaveCount = _waveCount;
        currWave = 1;
        waveSize = _waveSize;
        waveType = _waveType;
        switch(waveSize)
        {
            case EnemyWaveSize.SMALL:
                waveBudget = 2 + difficulty;
                break;
            case EnemyWaveSize.MEDIUM:
                waveBudget = 6 + difficulty;
                break;
            case EnemyWaveSize.LARGE:
                waveBudget = 10 + difficulty;
                break;
        }
        ByleCoin.setCoinsToFly(false);
        currBudget = waveBudget;
        currState = SpawnerState.generatingWave;
        spawns.Clear();
        foreach (EQSNode node in EQS.instance.GetNodes())
        {
            if (!node.GetTraversable()) { continue; }
            if(node.GetDistance() > maxDistanceFromPlayerToSpawn || node.GetDistance() < minDistanceFromPlayerToSpawn) { continue; }
            spawns.Add(node.GetWorldPos());
        }
        Debug.Log(spawns.Count);
    }
    void Update()
    {
        switch(currState)
        {
            case SpawnerState.inactive:
                return;
            case SpawnerState.generatingWave:
                //Calculate which enemies to spawn
                //Calculate how many of each to spawn
                //Get Spawns
                //Spawn enemies
                //Move to wave active
                SpawnWave();
                TryAddEliteEnemies();
                break;
            case SpawnerState.spawningEnemies:
                //Spawns enemies in batches
                delayTimer+= Time.deltaTime;
                if(delayTimer < delayBetweenSpawns){break;}
                delayTimer = 0;
                if(AllEnemiesSpawned())
                {
                    currState = SpawnerState.waveActive;
                    break;
                }
                SpawnEnemies();
                break;
            case SpawnerState.waveActive:
                //Wait until all enemies unsubscribed
                //Move back to spawningWave if more waves
                //Else go to waves completed
                CheckWaveEnded();
                break;
            case SpawnerState.wavesCompleted:
                return;
        }
    }
    private void SpawnWave()
    {
        ResetProfileCounts();
        ResetProfilePools();
        if(roomsCleared < 2)
        {
            waveType = EnemyWaveType.EASY_ENEMIES;
        }
        else if(roomsCleared < 3)
        {
            if(waveType == EnemyWaveType.HARD_ENEMIES || waveType == EnemyWaveType.ALL_ENEMIES || waveType == EnemyWaveType.RANDOM)
            {
                waveType = EnemyWaveType.EASY_ENEMIES;
            }
        }
        BossBar.instance.ClearBar();
        currBudget = waveBudget;
        //Get all the profile groups that are required
        //This gets available indexes
        List<List<int>> availablePools = new List<List<int>>();
        List<int> selectedProfiles = new List<int>();
        switch(waveType)
        {
            case EnemyWaveType.EASY_ENEMIES:
                availablePools.Add(easyProfiles);
                break;
            case EnemyWaveType.MEDIUM_ENEMIES:
                availablePools.Add(mediumProfiles);
                break;
            case EnemyWaveType.HARD_ENEMIES:
                availablePools.Add(HardProfiles);
                break;
            case EnemyWaveType.ALL_ENEMIES:
                availablePools.Add(easyProfiles);
                availablePools.Add(mediumProfiles);
                availablePools.Add(HardProfiles);
                break;
        }


        if(waveType == EnemyWaveType.ALL_ENEMIES)
        {
            foreach(List<int> pool in availablePools)
            {
                int rnd = Random.Range(0,pool.Count);
                selectedProfiles.Add(pool[rnd]);
                int index = pool[rnd];
                EnemyProfiles[index].IncremenetCount();
                currBudget -= EnemyProfiles[index].GetCost();
            }
        }
        else
        {
            //Chance to get one enemy type rather than a mix
            int rndChance = Random.Range(0,100);
            if(rndChance < 70)
            {
                //mix
                for(int i =0; i < maxEnemyTypesPerWave; i++)
                {
                    int rndProfile = Random.Range(0,availablePools[0].Count);
                    selectedProfiles.Add(availablePools[0][rndProfile]);
                    int index = selectedProfiles[i];
                    EnemyProfiles[index].IncremenetCount();
                    currBudget -= EnemyProfiles[index].GetCost();
                }
            }
            else
            {
                Debug.Log("Homogenous wave");
                //one
                int rndProfile = Random.Range(0,availablePools[0].Count);
                selectedProfiles.Add(availablePools[0][rndProfile]);
                int index = selectedProfiles[0];
                EnemyProfiles[index].IncremenetCount();
                currBudget -= EnemyProfiles[index].GetCost();
            }
        }

        //Pick one random from each group, and add +1 to it
        //Keep randomly iterating profiles until score is zero
        List<int> iteratableProfiles = new List<int>();
        iteratableProfiles.AddRange(selectedProfiles);
        int breaker = 0;
        while(iteratableProfiles.Count > 0 && currBudget > 0)
        {
            breaker++;
            if(breaker > 100){break;}
            int rnd = Random.Range(0,iteratableProfiles.Count);
            int index = iteratableProfiles[rnd];
            //Check Budget
            if(EnemyProfiles[index].GetIsFull())
            {
                if(currBudget <= 0)
                {
                    break;
                }
                Debug.Log("Adding filler enemy!");
                fillerEnemy.IncremenetCount();
                currBudget -= 1;
            }
            if(EnemyProfiles[index].GetCost() <= currBudget)
            {
                currBudget -= EnemyProfiles[index].GetCost();
                EnemyProfiles[index].IncremenetCount();
            }
            else
            {
                iteratableProfiles.Remove(index);
            }
        }

        //Debug.Log("WAVE SPAWNED");
        foreach(EnemySpawningProfile profile in EnemyProfiles)
        {
            if(profile.GetCount() > 0)
            {
                //Debug.Log(profile.prefab + " #" + profile.GetCount());
            }
        }
        //Get count of enemies to spawn
        currWave ++;
        currState = SpawnerState.spawningEnemies;
        currProfileIndex = 0;
    }
    private void CheckWaveEnded()
    {
        if(subscribedEnemies.Count != 0){return;}
        
        if(currWave > WaveCount)
        {
            currState = SpawnerState.wavesCompleted;
            //Debug.Log("Waves completed!");
            roomsCleared ++;
            Debug.Log(roomsCleared + " rooms cleared");
            difficulty += difficultyIncreasePerRoom;
            if(difficulty > maxDifficulty)
            {
                difficulty = maxDifficulty;
            }
            ByleCoin.setCoinsToFly(true);
        }
        else
        {
            currState = SpawnerState.generatingWave;
        }
    }

    public bool isRoomCleared()
    {
        if (currWave > WaveCount)
        {
            if (subscribedEnemies.Count != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    private void ResetProfileCounts()
    {
        foreach(EnemySpawningProfile profile in EnemyProfiles)
        {
            profile.ResetCount();
        }
    }
    public bool AllEnemiesSpawned()
    {
        foreach(EnemySpawningProfile profile in EnemyProfiles)
        {
            if(profile.GetCount() > 0)
            {
                return false;
            }
        }
        if(fillerEnemy.GetCount() > 0)
        {
            return false;
        }
        return true;
    }
    private void SpawnEnemies()
    {
        //Spawn filler
        if(fillerEnemy.GetCount() > 0)
        {
            int rndSpawn = Random.Range(0, spawns.Count);
            Vector3 randSpawn = spawns[rndSpawn];
            spawns.RemoveAt(rndSpawn);
            GameObject enemy = GameObject.Instantiate(fillerEnemy.prefab, randSpawn, Quaternion.identity);
            Vector3 pos = enemy.transform.position;
            pos.y = 1;
            enemy.transform.position = pos;
            enemy.GetComponent<BTAgent>().SubscribeAgent(this);
            fillerEnemy.DecrementCount();
            return;
        }
        //Check if all profiles are full
        foreach(EnemySpawningProfile profile in EnemyProfiles)
        {
            if(profile.GetCount() <= 0){continue;}
            int rndSpawn = Random.Range(0, spawns.Count);
            Vector3 randSpawn = spawns[rndSpawn];
            spawns.RemoveAt(rndSpawn);
            GameObject enemy = GameObject.Instantiate(profile.prefab, randSpawn, Quaternion.identity);
            Vector3 pos = enemy.transform.position;
            pos.y = 1;
            enemy.transform.position = pos;
            enemy.GetComponent<BTAgent>().SubscribeAgent(this);
            profile.DecrementCount();
            return;
        }
    }

    public void KillAllEnemies()
    {
        foreach (BTAgent subscriber in subscribedEnemies)
        {
            subscriber.Damage(999, true, false, true);
        }
    }
    private void TryAddEliteEnemies()
    {
        //Dice roll
        if(roomsCleared <= minRoomsClearedToSpawnElites){return;}
        float rnd = Random.Range(1, 100);
        if(rnd > EliteSpawnChance * 100) { return; }
        List<int> validProfiles = new List<int>();
        for (int i = 0; i < EnemyProfiles.Count - 1; i++)
        {
            if (EnemyProfiles[i].GetCount() <= 0) { continue; }
            validProfiles.Add(i);
        }
        int rndIndex = Random.Range(0, validProfiles.Count);
        int val = validProfiles[rndIndex];
        EnemyProfiles[val].SetElite(true);
        int rndSpawn = Random.Range(0, spawns.Count - 1);
        Vector3 randSpawn = spawns[rndSpawn];
        spawns.RemoveAt(rndSpawn);
        SpawnEliteEnemy(EnemyProfiles[val], randSpawn);
    }
    private void SpawnEliteEnemy(EnemySpawningProfile _eliteProfile, Vector3 spawn)
    {
        _eliteProfile.SetElite(false);
        GameObject enemy = GameObject.Instantiate(_eliteProfile.prefab, spawn, Quaternion.identity);
        Vector3 pos = enemy.transform.position;
        pos.y = 1;
        enemy.transform.position = pos;
        foreach (Transform eye in enemy.GetComponent<BTAgent>().eliteEyes)
        {
            GameObject eliteEye = Instantiate(eliteEyePrefab, eye.transform.position + eye.transform.forward * 0.1f, eye.transform.rotation);
            eliteEye.transform.parent = eye;
        }
        BTAgent elite = enemy.GetComponent<BTAgent>();
        elite.SubscribeAgent(this);
        elite.enemyType = BTAgent.EnemyType.Elite;
        elite.SetMaxHealth(maxEliteHealth);
        int count = Random.Range(1,maxModifiersPerElite + 1);
        if(count == 1)
        {
            BTModifier rndModifier = GetEliteModifier();
            elite.gameObject.AddComponent(rndModifier.GetType());
        }
        else
        {   
            List<BTModifier> mods = GetEliteModifiers(count);
            foreach(BTModifier mod in mods)
            {
                elite.gameObject.AddComponent(mod.GetType());
            }
        }
        elite.DisplayName = RandomEliteNames.GetRandomName();
        enemy.GetComponent<BTAgent>().isElite = true;
    }
    private BTModifier GetEliteModifier()
    {
        int rnd = Random.Range(0,eliteModifiers.Count);
        return eliteModifiers[rnd];
    }
    private List<BTModifier> GetEliteModifiers(int _count)
    {
        List<BTModifier> temp = new List<BTModifier>();
        List<BTModifier> list = new List<BTModifier>();
        temp.AddRange(eliteModifiers);
        for(int i =0 ; i < _count; i++)
        {
            int rnd = Random.Range(0, temp.Count);
            list.Add(temp[rnd]);
            temp.Remove(temp[rnd]);
        }
        return list;
    }
    public void DefaultSpawnerVariables()
    {
        currWave = 2;
        WaveCount = 1;
        foreach (EnemySpawningProfile profile in EnemyProfiles)
        {
            profile.IncremenetCount();
            profile.IncremenetCount();
            profile.IncremenetCount();
        }
    }
    public void ResetDifficulty()
    {
        difficulty = 0;
    }
    #region Subscriber Pattern
    public void SubscribeEnemy(BTAgent _enemy)
    {
        if(subscribedEnemies.Contains(_enemy)){return;}
        subscribedEnemies.Add(_enemy);
    }
    public void UnsubscribeEnemy(BTAgent _enemy)
    {
        if(!subscribedEnemies.Contains(_enemy)){return;}
        subscribedEnemies.Remove(_enemy);
    }
    public List<BTAgent> GetEnemies(){return subscribedEnemies;}
    #endregion
}
[System.Serializable]
public class EnemySpawningProfile
{
    public GameObject prefab;
    public EnemySpawningSystem.EnemyType enemyType;
    public int maxCount = 12;
    private int amount;
    public int minRoomsClearedToSpawn = 0;
    public int minLevelToSpawn = 0;
    private bool isElite = false;
    public void ResetCount(){amount =0; isElite = false; }
    public void IncremenetCount()
    {
        if(amount >= maxCount)
        {
            amount = maxCount;
            return;
        } 
        amount++;
    }
    public void DecrementCount(){amount--;}
    public int GetCount(){return amount;}
    public int GetCost()
    {
        switch(enemyType)
        {
            case EnemySpawningSystem.EnemyType.EASY:
                return 1;
            case EnemySpawningSystem.EnemyType.MEDIUM:
                return 2;
            case EnemySpawningSystem.EnemyType.HARD:
                return 4;
            default:
                return 1;
        }
    }
    public bool GetIsFull()
    {
        return amount >= maxCount;
    }
    public bool GetElite() { return isElite; }
    public void SetElite(bool _isElite)
    {
        isElite = _isElite;
    }
}