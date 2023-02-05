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
    [SerializeField] private RectTransform nextRoundButtonTransform;
    [SerializeField] private float nextRoundButtonAnimTime = 0.5f;
    [SerializeField] private AnimationCurve nextRoundButtonCurve;


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

    float timeVal = 0;
    bool nextRoundButtonClosing;
    private void OnRoundEnded()
    {
        nextRoundButtonClosing = false;
        timeVal = 1;

        roundsText.text = $"Upcoming Round {RoundSystem.roundNumber}";
    }


    public void TransitionToNextRound()
    {
        if (timeVal <= 0)
        {
            nextRoundButtonClosing = true;
            timeVal = 1;

            RoundSystem.StartRound();
        }
    }


    private void Update()
    {
        if (timeVal > 0)
        {
            timeVal -= Time.deltaTime * (1/ nextRoundButtonAnimTime);
            timeVal = Mathf.Max(0, timeVal);

            float val = (nextRoundButtonClosing) ? nextRoundButtonCurve.Evaluate(1 - timeVal) : 1 - nextRoundButtonCurve.Evaluate(1 - timeVal);

            nextRoundButtonTransform.pivot = new Vector2(1, Mathf.Lerp(0, 1.5f, val));
        }
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
