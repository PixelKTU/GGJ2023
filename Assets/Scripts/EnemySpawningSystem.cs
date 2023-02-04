using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


class EnemySpawnerData
{
    public float spawnsPerMinute { get; private set; }
    public Vector3 spawnPosition { get; private set; }

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

    public EnemySpawnerData(float spawnsPerMinute, Vector3 spawnPosition)
    {
        this.spawnsPerMinute = spawnsPerMinute;
        this.spawnPosition = spawnPosition;
    }
}

public class EnemySpawningSystem : MonoBehaviour
{
    [SerializeField] RoundSystem roundSystem;
    [SerializeField] GameObject enemyPrefab;

    List<EnemySpawnerData> enemySpawnerDataList = new List<EnemySpawnerData>();
    IObjectPool<GameObject> enemyPool;

    public static int enemiesLeft { get; private set; }
    int enemiesLeftToSpawn;

    public void RegisterEnemySpawner(float spawnsPerMinute, Vector3 spawnPosition)
    {
        enemySpawnerDataList.Add(new EnemySpawnerData(spawnsPerMinute, spawnPosition));
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


    void SpawnEnemies(Vector3 position, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = enemyPool.Get();
            obj.transform.position = position;
        }
        enemiesLeft += count;
    }


    public void RemoveEnemy(GameObject obj)
    {
        enemyPool.Release(obj);
        enemiesLeft--;
    }


    void Start()
    {
        enemyPool = new ObjectPool<GameObject>(CreateEnemyObject, GetEnemyObject, ReserveEnemyObject, DestroyEnemyObject);
        RoundSystem.roundStartEvent.AddListener(RoundStarted);

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

                    SpawnEnemies(data.spawnPosition, enemyCount);
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


    GameObject CreateEnemyObject()
    {
        return Instantiate(enemyPrefab);
    }
    void GetEnemyObject(GameObject obj)
    {
        obj.SetActive(true);
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
