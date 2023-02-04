using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerSc : MonoBehaviour
{
    [SerializeField] bool active = false;
    [SerializeField] EnemySpawningSystem enemySpawningSystem;
    [SerializeField] float spawnsPerMinute = 60;


    bool oneTime = true;
    void Update()
    {
        if (oneTime && active)
        {
            oneTime = false;
            enemySpawningSystem.RegisterEnemySpawner(spawnsPerMinute, transform.position);
        }
    }
}
