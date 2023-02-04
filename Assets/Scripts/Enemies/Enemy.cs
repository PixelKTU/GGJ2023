using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] EnemyType enemyType;

    public float imaginaryHealth { get; private set; }
    public float health { get; private set; }
    public bool dead { get; private set; } 

    public void TakeDamage(float damage)
    {
        if (!dead)
        {
            health -= damage;

            if (health <= 0)
            {
                Death();
            }
        }
    }

    void Death()
    {
        dead = true;
        EnemySpawningSystem.Instance.RemoveEnemy(gameObject, enemyType);
    }

    public void TakeImaginaryDamage(float damage)
    {
        imaginaryHealth -= damage;
    }
    public void ResetData()
    {
        health = maxHealth;
        imaginaryHealth = maxHealth;
        dead = false;
    }

    /// TODO
    /// Animation controls
    /// Attack animations and code
    /// Movement
}
