using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public static EnemyBase Instance;

    public Action<float, float> OnTakeDamage;//current health and damage taken is passed in order

    [SerializeField] private float defaultHealth = 20;

    private float currentHealth;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            currentHealth = defaultHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnTakeDamage?.Invoke(currentHealth, damage);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.EndGame(true);
        }
    }

    public float GetDefaultHealth()
    {
        return defaultHealth;
    }

    public float GetHealth()
    {
        return currentHealth;
    }
}
