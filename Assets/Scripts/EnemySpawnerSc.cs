using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerSc : MonoBehaviour
{
    [SerializeField] bool active = false;
    [SerializeField] EnemyType enemyTypeThatSpawns;
    [SerializeField] float spawnsPerMinute = 60;


    bool oneTime = true;
    void Update()
    {
        if (oneTime && active)
        {
            oneTime = false;
            EnemySpawningSystem.Instance.RegisterEnemySpawner(spawnsPerMinute, transform.position, enemyTypeThatSpawns);
        }
    }
}
