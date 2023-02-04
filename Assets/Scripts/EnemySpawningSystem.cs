using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


class EnemySpawnerData
{
    public float spawnsPerMinute { get; private set; }
    public Vector3 spawnPosition { get; private set; }
    public EnemyType enemyType { get; private set; }

    float spawnVal;
    public void ResetEnemySpawning(float timeOffset = 0)
    {
        if (spawnsPerMinute == 0)
        {
            spawnVal = Mathf.Infinity;
        }
        else
        {
            spawnVal = 1 / (spawnsPerMinute / 60f) - timeOffset;
        }
    }
    public int HowManyEnemiesToSpawn(float passedTime)
    {
        int ans = 0;
        spawnVal -= passedTime;

        while (spawnVal <= 0)
        {
            ResetEnemySpawning(Mathf.Abs(spawnVal));
            ans++;
        }

        return ans;
    }

    public EnemySpawnerData(float spawnsPerMinute, Vector3 spawnPosition, EnemyType enemyType = 0)
    {
        this.spawnsPerMinute = spawnsPerMinute;
        this.spawnPosition = spawnPosition;
        this.enemyType = enemyType;
    }
}

public enum EnemyType
{
    Drone,
    Crab,
}

public class EnemySpawningSystem : MonoBehaviour
{

    public static EnemySpawningSystem Instance;

    [SerializeField] RoundSystem roundSystem;
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();

    List<EnemySpawnerData> enemySpawnerDataList = new List<EnemySpawnerData>();
    List<IObjectPool<GameObject>> enemyPool = new List<IObjectPool<GameObject>>();

    public static int enemiesLeft { get; private set; }
    int enemiesLeftToSpawn;

    public void RegisterEnemySpawner(float spawnsPerMinute, Vector3 spawnPosition, EnemyType enemyType = 0)
    {
        enemySpawnerDataList.Add(new EnemySpawnerData(spawnsPerMinute, spawnPosition, enemyType));
    }

    //bool roundStarted = false;
    void RoundStarted()
    {
        enemiesLeftToSpawn = roundSystem.GetThisRoundData().enemyCount;

        foreach (EnemySpawnerData data in enemySpawnerDataList)
        {
            data.ResetEnemySpawning();
        }
        //  roundStarted = true;
    }

    List<GameObject> aliveEnemies = new List<GameObject>();
    void SpawnEnemies(Vector3 position, int count, EnemyType type = 0)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = enemyPool[(int)type].Get();
            obj.transform.position = position;
            aliveEnemies.Add(obj);
        }
        enemiesLeft += count;
    }


    public void RemoveEnemy(GameObject obj, EnemyType type = 0)
    {
        enemyPool[(int)type].Release(obj);
        aliveEnemies.Remove(obj);
        enemiesLeft--;
    }


    public GameObject GetNearestEnemy(Vector3 position)
    {
        GameObject ans = null;
        float dist = Mathf.Infinity;
        float temp;
        foreach (GameObject obj in aliveEnemies)
        {
            temp = (position - obj.transform.position).sqrMagnitude;
            if (temp < dist)
            {
                ans = obj;
                dist = temp;
            }
        }
        return ans;
    }


    void Start()
    {
        enemyPool.Add(new ObjectPool<GameObject>(CreateEnemyObject1, GetEnemyObject, ReserveEnemyObject, DestroyEnemyObject));
        enemyPool.Add(new ObjectPool<GameObject>(CreateEnemyObject2, GetEnemyObject, ReserveEnemyObject, DestroyEnemyObject));

        RoundSystem.roundStartEvent.AddListener(RoundStarted);

    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (RoundSystem.roundStarted)
        {
            if (enemiesLeftToSpawn > 0)
            {
                foreach (EnemySpawnerData data in enemySpawnerDataList)
                {
                    int enemyCount = data.HowManyEnemiesToSpawn(Time.deltaTime);

                    enemyCount = Mathf.Min(enemiesLeftToSpawn, enemyCount);
                    enemiesLeftToSpawn -= enemyCount;

                    SpawnEnemies(data.spawnPosition, enemyCount, data.enemyType);
                }
            }
            if (enemiesLeft <= 0 && enemiesLeftToSpawn <= 0)
            {
                RoundSystem.EndRound();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            enemiesLeft = 0;
        }
    }


    GameObject CreateEnemyObject1()
    {
        GameObject obj = Instantiate(enemyPrefabs[0]);
        obj.GetComponent<Enemy>().ResetData();
        return obj;
    }
    GameObject CreateEnemyObject2()
    {
        GameObject obj = Instantiate(enemyPrefabs[1]);
        obj.GetComponent<Enemy>().ResetData();
        return obj;
    }
    void GetEnemyObject(GameObject obj)
    {
        obj.SetActive(true);
        obj.GetComponent<Enemy>().ResetData();
    }
    void ReserveEnemyObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    void DestroyEnemyObject(GameObject obj)
    {
        Destroy(obj);
    }
}
