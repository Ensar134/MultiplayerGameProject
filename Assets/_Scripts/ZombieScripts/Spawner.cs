using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    public static Spawner Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of Spawner found!");
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    private float _nextSpawnTime;
    private float _nextWaveTime;
    private int _currentWave = 0;
    private int _zombiesKilled = 0;

    [SerializeField] float _delayBetweenWaves = 10f; 
    [SerializeField] float _delay = 2f;
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField] private List<GameObject> _classicZombies = new List<GameObject>();
    [SerializeField] private List<GameObject> _classicDummies = new List<GameObject>();
    [SerializeField] private List<GameObject> _patlayanZombies = new List<GameObject>();
    [SerializeField] private List<GameObject> _patlayanDummies = new List<GameObject>();
    [SerializeField] private List<GameObject> _BossMale = new List<GameObject>();
    [SerializeField] private List<GameObject> _DummyBossMale = new List<GameObject>();
    [SerializeField] private List<GameObject> _BossRanged = new List<GameObject>();
    [SerializeField] private List<GameObject> _DummyBossRanged = new List<GameObject>();

    [Serializable]
    public class Wave
    {
        public int numClassicZombies;
        public int numExplosiveZombies;
        public int numBossZombies;
        public int numRangedBossZombies;
    }
    
    [SerializeField] private List<Wave> waves = new List<Wave>();

    void Start()
    {
        if (isServer)
        {
            StartNextWave();
        }
    }
    
    void Update()
    {
        if (!isServer) return;
        
        if (AllZombiesDead() && Time.time >= _nextWaveTime)
        {
            StartNextWave();
        }
    }
    
    void SpawnFromWave()
    {
        Wave currentWave = waves[_currentWave];
    
        for(int i = 0; i < currentWave.numClassicZombies; i++)
        {
            SpawnClassicZombieAndDummy();
            _nextSpawnTime = Time.time + _delay;
        }

        for(int i = 0; i < currentWave.numExplosiveZombies; i++)
        {
            SpawnExplosiveZombieAndDummy();
            _nextSpawnTime = Time.time + _delay;
        }

        for(int i = 0; i < currentWave.numBossZombies; i++)
        {
            SpawnBossZombieAndDummy();
            _nextSpawnTime = Time.time + _delay;
        }

        for(int i = 0; i < currentWave.numRangedBossZombies; i++)
        {
            SpawnRangedBossZombieAndDummy();
            _nextSpawnTime = Time.time + _delay;
        }
    }
    
    bool AllZombiesDead()
    {
        if (_currentWave >= waves.Count)
            return false;  
    
        int totalZombiesInWave = waves[_currentWave].numClassicZombies +
                                 waves[_currentWave].numExplosiveZombies +
                                 waves[_currentWave].numBossZombies +
                                 waves[_currentWave].numRangedBossZombies;

        bool allDead = _zombiesKilled >= totalZombiesInWave;
        Debug.Log("AllZombiesDead: " + allDead + ", Ölü Zombi Sayısı: " + _zombiesKilled + ", Wave'deki Toplam Zombi Sayısı: " + totalZombiesInWave);
        return allDead;
    }
    
    void StartNextWave()
    {
        _zombiesKilled = 0;

        if (_currentWave < waves.Count)
        {
            SpawnFromWave();
            _currentWave++; 
            _nextWaveTime = Time.time + _delayBetweenWaves;
        }
    }
    
    public void ZombieKilled()
    {
        _zombiesKilled++;
        
        Debug.Log("Zombi öldürüldü. Toplam ölü zombi sayısı: " + _zombiesKilled);
    }

    private int ChooseClassicDummy()
    {
        int classicDummyChoose = UnityEngine.Random.Range(0, _classicDummies.Count);
        return classicDummyChoose;
    }
    
    private int ChooseExplosiveDummy()
    {
        int explosiveDummyChoose = UnityEngine.Random.Range(0, _patlayanDummies.Count);
        return explosiveDummyChoose;
    }
    
    private int ChooseBossDummy()
    {
        int bossDummyChoose = UnityEngine.Random.Range(0, _DummyBossMale.Count);
        return bossDummyChoose;
    }
    
    private int ChooseRangedBossDummy()
    {
        int bossDummyChoose = UnityEngine.Random.Range(0, _DummyBossRanged.Count);
        return bossDummyChoose;
    }

    private void SpawnClassicZombieAndDummy()
    {
        _nextSpawnTime = Time.time + _delay;
        Transform spawnPoint = ChooseSpawnPoint();

        GameObject zombie = Instantiate(_classicZombies[ChooseClassicDummy()], spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(zombie);

        GameObject dummy = Instantiate(_classicDummies[ChooseClassicDummy()], spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(dummy);

        dummy.GetComponent<AIFollower>().aiTarget = zombie.transform;
        dummy.GetComponent<AIFollower>().currentState = zombie.GetComponent<ZombieAI>().currentState;
        dummy.GetComponent<AIFollower>().zombieAI = zombie.GetComponent<ZombieAI>();
    }
    
    private void SpawnExplosiveZombieAndDummy()
    {
        _nextSpawnTime = Time.time + _delay;
        Transform spawnPoint = ChooseSpawnPoint();

        GameObject zombie = Instantiate(_patlayanZombies[ChooseExplosiveDummy()], new Vector3(spawnPoint.position.x, 0 , spawnPoint.position.z), spawnPoint.rotation);
        NetworkServer.Spawn(zombie);

        GameObject dummy = Instantiate(_patlayanDummies[ChooseExplosiveDummy()], spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(dummy);

        dummy.GetComponent<PatlayanAIFollower>().aiTarget = zombie.transform;
        dummy.GetComponent<PatlayanAIFollower>().currentState = zombie.GetComponent<PatlayanZombieAI>().currentState;
        dummy.GetComponent<PatlayanAIFollower>().patlayanZombieAI = zombie.GetComponent<PatlayanZombieAI>();
    }
    
    private void SpawnBossZombieAndDummy()
    {
        _nextSpawnTime = Time.time + _delay;
        Transform spawnPoint = ChooseSpawnPoint();

        GameObject bossZombie = Instantiate(_BossMale[ChooseBossDummy()], new Vector3(spawnPoint.position.x, 0 , spawnPoint.position.z), spawnPoint.rotation);
        NetworkServer.Spawn(bossZombie);

        GameObject bossDummy = Instantiate(_DummyBossMale[ChooseBossDummy()], spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(bossDummy);

        bossDummy.GetComponent<BossAIFollower>().aiTarget = bossZombie.transform;
        bossDummy.GetComponent<BossAIFollower>().currentState = bossZombie.GetComponent<BossAI>().currentState;
        bossDummy.GetComponent<BossAIFollower>().bossAI = bossZombie.GetComponent<BossAI>();
    }
    
    private void SpawnRangedBossZombieAndDummy()
    {
        _nextSpawnTime = Time.time + _delay;
        Transform spawnPoint = ChooseSpawnPoint();

        GameObject bossZombie = Instantiate(_BossRanged[ChooseRangedBossDummy()], new Vector3(spawnPoint.position.x, 0 , spawnPoint.position.z), spawnPoint.rotation);
        NetworkServer.Spawn(bossZombie);

        GameObject bossDummy = Instantiate(_DummyBossRanged[ChooseRangedBossDummy()], spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(bossDummy);

        bossDummy.GetComponent<RangedBossAIFollower>().aiTarget = bossZombie.transform;
        bossDummy.GetComponent<RangedBossAIFollower>().currentState = bossZombie.GetComponent<RangedBossAI>().currentState;
        bossDummy.GetComponent<RangedBossAIFollower>().rangedBossAI = bossZombie.GetComponent<RangedBossAI>();
    }

    Transform ChooseSpawnPoint()
    {
        int randomIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);
        var spawnPoint = _spawnPoints[randomIndex];
        return spawnPoint;
    }

    bool ShouldSpawn()
    {
        return Time.time >= _nextSpawnTime;
    }
}