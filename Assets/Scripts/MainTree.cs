using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTree : MonoBehaviour
{
    public static MainTree Instance;
    public Action<float, float> OnTakeDamage;//current health and damage taken is passed in order

    [SerializeField] private float defaultHealth = 100;

    private float health;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            health = defaultHealth;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(1);
        }
    }

    #region Health

    public float GetHealth()
    {
        return health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        OnTakeDamage?.Invoke(health, damage);

        if (health <= 0)
        {
            health = 0;
            GameManager.Instance.EndGame(false);
        }
    }

    #endregion
}
