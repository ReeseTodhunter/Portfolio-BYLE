using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EnemySpawner : MonoBehaviour
{
    #region fields
    public List<EnemyProfile> Enemies = new List<EnemyProfile>();
    public List<BTModifier> eliteModifiers = new List<BTModifier>();
    [Range(0,1)]
    public float eliteSpawnRate = 0.05f;
    public int eliteModifierCount = 1;
    public static EnemySpawner instance;
    public int concurrentEnemies = 3;
    public EnemyProfile defaultEnemy = null;
    public bool spawnOnAwake = false;
    private int enemyCount = 0;
    public int rounds = 0;
    private int currRound = 0;
    private int currBudget = 0;
    private List<BTAgent> subscribers = new List<BTAgent>();
    private bool roomCleared = true;
    public GameObject LootGoblin, ShopKeeper, SlimeBoss;
    public float interval = 0.5f, intervalTimer = 0;
    public bool spawning = false;
    private List<EnemySpawnProfile> enemiesToSpawn = new List<EnemySpawnProfile>();
    private List<Vector3> spawns = new List<Vector3>();
    public List<ISpawnerSubscriber> listeners = new List<ISpawnerSubscriber>();
    private bool enemiesDropCoins = true;
    public bool spawnsElite = true;
    public GameObject EliteEyePrefab;
    public enum UnqiueEnemies
    {
        LootGoblin,
        ShopKeeper
    }
    #endregion
    #region Singleton
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
    void Start()
    {
        currRound = 0;
        spawning = false;
        if(spawnOnAwake){SpawnEnemies(6, rounds,spawnsElite);}
    }
    #endregion
    public void SpawnEnemies(int _budget, int _rounds = 1, bool _enemiesDropCoins = true, bool _spawnsElite = true)
    {
        ByleCoin.setCoinsToFly(false);
        roomCleared = false;
        currBudget = _budget;
        rounds = _rounds;
        currRound = 0;
        enemiesDropCoins = _enemiesDropCoins;
        SpawnRound(_budget,_spawnsElite);
    }
    public void SpawnSingularEnemy(UnqiueEnemies enemy, Transform spawnPos)
    {
    }
    private void SpawnRound(int budget, bool _spawnsElite = false)
    {
        currRound++;
        enemiesToSpawn = GetEnemyGroup(budget);
        float rnd = Random.Range(1,100) / 100;
        if(_spawnsElite)
        {
            EnemySpawnProfile eliteProfile = GetEliteEnemy();
            enemiesToSpawn.Add(eliteProfile);
        }
        EQSNode[,] nodes = EQS.instance.GetNodes();
        spawns.Clear();
        foreach(EQSNode node in nodes)
        {
            if(node.GetTraversable())
            {
                spawns.Add(node.GetWorldPos());
            }
        }
        spawning = true;
    }
    private List<EnemySpawnProfile> GetEnemyGroup(int _maxBudget)
    {
        //Select three random enemies
        List<EnemyProfile> availableProfiles = new List<EnemyProfile>();
        availableProfiles.AddRange(Enemies);
        List<EnemyProfile> selectedProfiles = new List<EnemyProfile>();
        for (int i = 0; i < concurrentEnemies; i++)
        {
            int index = Random.Range(0, availableProfiles.Count);
            selectedProfiles.Add(availableProfiles[index]);
            availableProfiles.Remove(availableProfiles[index]);
        }
        //Find the cheapest enemy and set it as the filler profile
        //Also create spawnprofiles
        List<EnemySpawnProfile> spawnProfiles = new List<EnemySpawnProfile>();
        EnemySpawnProfile fillerProfile = null;
        int lowestCost = int.MaxValue;
        foreach (EnemyProfile profile in selectedProfiles)
        {
            EnemySpawnProfile newProfile = new EnemySpawnProfile(profile);
            spawnProfiles.Add(newProfile);
            newProfile.IncrementCount();
            if (profile.cost < lowestCost)
            {
                lowestCost = profile.cost;
                fillerProfile = newProfile;
            }
        }
        /*
         * Iterate over each profile randomly buying them until either :
         * - The budget runs out
         * - The filler is the only enemy that can be afforded.
         * 
         * If the budget runs out simply spawn enemies
         * If the filler is left, spend the rest of the budget on the filler
         */

        int budget = _maxBudget;
        List<EnemySpawnProfile> temp = new List<EnemySpawnProfile>();
        temp.AddRange(spawnProfiles);
        while (true)
        {
            int index = Random.Range(0, temp.Count - 1);
            if (temp[index].profile.cost <= budget)
            {
                //Increment count
                temp[index].IncrementCount();
                budget -= temp[index].profile.cost;
            }
            else
            {
                //Remove profile from temp, no longer affordable
                temp.Remove(temp[index]);
            }
            if (temp.Count <= 1)
            {
                int fillerCost = fillerProfile.profile.cost;
                int maxFillerCount = budget / fillerCost;
                fillerProfile.IncrementCount(maxFillerCount);
                break;
            }
        }
        return spawnProfiles;
    }
    private EnemySpawnProfile GetEliteEnemy()
    {
        List<EnemyProfile> validElites = new List<EnemyProfile>();
        foreach(EnemyProfile profile in Enemies)
        {
            if(!profile.canSpawnAsElite){continue;}
            validElites.Add(profile);
        }
        int rnd = Random.Range(0,validElites.Count);
        EnemySpawnProfile eliteProfile = new EnemySpawnProfile(validElites[rnd]);
        eliteProfile.spawnAsElite = true;
        return eliteProfile;
    }
    private List<BTModifier> GetEliteModifiers(int _modifierCount)
    {
        List<BTModifier> modifiers = new List<BTModifier>();
        List<BTModifier> avaliableModifiers = new List<BTModifier>();
        avaliableModifiers.AddRange(eliteModifiers);
        for(int i = 0; i < _modifierCount; i++)
        {
            int index = Random.Range(0,avaliableModifiers.Count);
            modifiers.Add(avaliableModifiers[index]);
            avaliableModifiers.RemoveAt(index);
        }
        return modifiers;
    }
    private void ApplyEliteModifers(List<BTModifier> _modifiers, BTAgent _agent)
    {
        foreach(BTModifier _modifier in _modifiers)
        {
            _agent.gameObject.AddComponent(_modifier.GetType());
        }
    }
    public void KillAllEnemies()
    {
        foreach(BTAgent subscriber in subscribers)
        {
            subscriber.Damage(999,true,false,true);
        }
    }
    #region Subscription
    public void SubscribeAgent(BTAgent agent)
    {
        if(subscribers.Contains(agent)){return;}
        subscribers.Add(agent);
    }
    public void UnsubscribeAgent(BTAgent agent)
    {
        if(!subscribers.Contains(agent)){return;}
        subscribers.Remove(agent);
        if(subscribers.Count <= 0)
        {
            if(currRound < rounds)
            {
                SpawnRound(currBudget, false);
                return;
            }
            ByleCoin.setCoinsToFly(true);
            roomCleared = true;
            foreach(ISpawnerSubscriber listener in listeners)
            {
                listener.AllEnemiesDead();
            }
        }
    }
    public void SubscribeListener(ISpawnerSubscriber _listener)
    {
        if(listeners.Contains(_listener)){return;}
        listeners.Add(_listener);
    }
    public void UnsubscribeListener(ISpawnerSubscriber _listener)
    {
        if(!listeners.Contains(_listener)){return;}
        listeners.Remove(_listener);
    }
    #endregion
    public List<BTAgent> GetEnemies() { return subscribers; }
    public bool isRoomCleared() { return roomCleared; }

    private class EnemySpawnProfile
    {
        private int count;
        public EnemyProfile profile;
        public bool spawnAsElite = false;
        public EnemySpawnProfile(EnemyProfile _profile)
        {
            count = 0;
            profile = _profile;
        }
        public void IncrementCount(int increment = 1)
        {
            count+= increment;
        }
        public int GetCount() { return count; }
    }
}


[System.Serializable]
public class EnemyProfile
{
    public string profileName = null;
    public GameObject enemyPrefab;
    public int cost;
    public bool canSpawnAsElite = false;
    public int minCount = 1;
    public int maxCount = 1;
}
