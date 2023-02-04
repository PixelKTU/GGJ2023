using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text rootsText;
    [SerializeField] private TMP_Text incomeText;
    [SerializeField] private TMP_Text roundsText;

    void Start()
    {
        RoundSystem.roundStartEvent.AddListener(OnRoundStarted);
        RoundSystem.roundEndEvent.AddListener(OnRoundEnded);
        CurrencyManager.Instance.OnCurrencyUpdated += OnCurrencyUpdated;
        CurrencyManager.Instance.OnIncomeUpdated += OnIncomeUpdated;

        SetInitialText();
    }


    private void OnDestroy()
    {
        RoundSystem.roundStartEvent.RemoveListener(OnRoundStarted);
        RoundSystem.roundEndEvent.RemoveListener(OnRoundEnded);
        CurrencyManager.Instance.OnCurrencyUpdated -= OnCurrencyUpdated;
        CurrencyManager.Instance.OnIncomeUpdated -= OnIncomeUpdated;
    }

    private void SetInitialText()
    {
        rootsText.text = CurrencyManager.Instance.GetRoots().ToString();
        incomeText.text = $"+{CurrencyManager.Instance.GetCurrentIncome()}";
        roundsText.text = $"Upcoming Round {RoundSystem.roundNumber}";
    }

    private void OnCurrencyUpdated(int currencyCount)
    {
        rootsText.text = currencyCount.ToString();
    }

    private void OnIncomeUpdated(int incomeCount)
    {
        incomeText.text = $"+{incomeCount}";
    }

    private void OnRoundStarted()
    {
        roundsText.text = $"Round {RoundSystem.roundNumber}";
    }

    private void OnRoundEnded()
    {
        roundsText.text = $"Upcoming Round {RoundSystem.roundNumber}";
    }
}
