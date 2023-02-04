using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Attacking,
    Walking,
    Dying
};

public class Enemy : MonoBehaviour
{
    public float imaginaryHealth { get; private set; }
    public float health { get; private set; }
    public bool dead { get; private set; }


    [SerializeField] protected float maxHealth;
    [SerializeField] protected EnemyType enemyType;

    [Header("AI")]
    [SerializeField] protected NavMeshAgent agent;


    protected EnemyState state;

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
