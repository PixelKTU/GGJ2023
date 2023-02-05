using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public Action<int> OnCurrencyUpdated;
    public Action<int> OnIncomeUpdated;

    private int currentRoots;
    private int currentIncome;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetInfo();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RoundSystem.roundStartEvent.AddListener(OnRoundStarted);
        RoundSystem.roundEndEvent.AddListener(OnRoundEnded);
    }

    private void OnDestroy()
    {
        RoundSystem.roundStartEvent.RemoveListener(OnRoundStarted);
        RoundSystem.roundEndEvent.RemoveListener(OnRoundEnded);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            AddRoots(1);
            AddIncome(1);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            AddRoots(-1);
            AddIncome(-1);
        }
    }

    private void OnRoundStarted()
    {

    }

    private void OnRoundEnded()
    {
        AddRoots(currentIncome);
    }


    /// <summary>
    /// Can be used to add or remove roots from the player
    /// </summary>
    /// <param name="rootCount">Number of roots to add/remove</param>
    public void AddRoots(int rootCount)
    {
        currentRoots += rootCount;
        OnCurrencyUpdated?.Invoke(currentRoots);
    }

    /// <summary>
    /// Gets the current roots count the player has
    /// </summary>
    /// <returns>The number of roots the player has</returns>
    public int GetRoots()
    {
        return currentRoots;
    }

    /// <summary>
    /// Gets the current income
    /// </summary>
    /// <returns>The current income the player has at the start of each round</returns>
    public int GetCurrentIncome()
    {
        return currentIncome;
    }

    /// <summary>
    /// Adds to the current income (can be used to remove income)
    /// </summary>
    /// <param name="num">The number of additional income each round</param>
    public void AddIncome(int num)
    {
        currentIncome += num;
        OnIncomeUpdated?.Invoke(currentIncome);
    }

    //just reset info
    private void ResetInfo()
    {
        currentRoots = 0;
        currentIncome = 0;
    }
}
