using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TMP_Text rootsText;
    [SerializeField] private TMP_Text incomeText;
    [SerializeField] private TMP_Text roundsText;

    [Header("UI Screens")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [Header("Next round button")]
    [SerializeField] private GameObject nextRoundButtonObj;


    void Start()
    {
        GameManager.Instance.OnGameStateChange += OnGameStateChange;
        RoundSystem.roundStartEvent.AddListener(OnRoundStarted);
        RoundSystem.roundEndEvent.AddListener(OnRoundEnded);
        CurrencyManager.Instance.OnCurrencyUpdated += OnCurrencyUpdated;
        CurrencyManager.Instance.OnIncomeUpdated += OnIncomeUpdated;

        loseScreen.SetActive(false);
        winScreen.SetActive(false);
        SetInitialText();
    }


    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChange -= OnGameStateChange;
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
        nextRoundButtonObj.SetActive(true);
        roundsText.text = $"Upcoming Round {RoundSystem.roundNumber}";
    }


    public void TransitionToNextRound()
    {
        nextRoundButtonObj.SetActive(false);
        RoundSystem.StartRound();
    }


    private void OnGameStateChange(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Defeat:
                loseScreen.SetActive(true);
                break;
            case GameState.Won:
                winScreen.SetActive(true);
                break;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
