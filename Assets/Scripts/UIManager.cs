using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text rootsText;
    [SerializeField] private TMP_Text incomeText;
    [SerializeField] private TMP_Text roundsText;

    [SerializeField] private List<GameObject> UIToHide;

    [Header("Health bars")]
    [SerializeField] private Image enemyBaseBar;
    [SerializeField] private Image playerTreeBar;

    [Header("UI Screens")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [Header("Next round button")]
    [SerializeField] private RectTransform nextRoundButtonTransform;
    [SerializeField] private float nextRoundButtonAnimTime = 0.5f;
    [SerializeField] private AnimationCurve nextRoundButtonCurve;

    private float timeVal = 0;
    private bool nextRoundButtonClosing;

    void Start()
    {
        GameManager.Instance.OnGameStateChange += OnGameStateChange;
        RoundSystem.roundStartEvent.AddListener(OnRoundStarted);
        RoundSystem.roundEndEvent.AddListener(OnRoundEnded);
        CurrencyManager.Instance.OnCurrencyUpdated += OnCurrencyUpdated;
        CurrencyManager.Instance.OnIncomeUpdated += OnIncomeUpdated;

        MainTree.Instance.OnTakeDamage += OnPlayerTakeDamage;
        EnemyBase.Instance.OnTakeDamage += OnEnemyBaseTakeDamage;

        loseScreen.SetActive(false);
        winScreen.SetActive(false);
        SetInitialText();
    }

    private void Update()
    {
        if (timeVal > 0)
        {
            timeVal -= Time.deltaTime * (1 / nextRoundButtonAnimTime);
            timeVal = Mathf.Max(0, timeVal);

            float val = (nextRoundButtonClosing) ? nextRoundButtonCurve.Evaluate(1 - timeVal) : 1 - nextRoundButtonCurve.Evaluate(1 - timeVal);

            nextRoundButtonTransform.pivot = new Vector2(1, Mathf.Lerp(0, 1.5f, val));
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChange -= OnGameStateChange;
        RoundSystem.roundStartEvent.RemoveListener(OnRoundStarted);
        RoundSystem.roundEndEvent.RemoveListener(OnRoundEnded);
        CurrencyManager.Instance.OnCurrencyUpdated -= OnCurrencyUpdated;
        CurrencyManager.Instance.OnIncomeUpdated -= OnIncomeUpdated;
        MainTree.Instance.OnTakeDamage -= OnPlayerTakeDamage;
        EnemyBase.Instance.OnTakeDamage -= OnEnemyBaseTakeDamage;
    }

    private void SetInitialText()
    {
        rootsText.text = CurrencyManager.Instance.GetRoots().ToString();
        incomeText.text = $"+{CurrencyManager.Instance.GetCurrentIncome()}";
        roundsText.text = $"Upcoming Round {RoundSystem.roundNumber}";
        playerTreeBar.fillAmount = 1;
        enemyBaseBar.fillAmount = 1;
    }

    #region Events

    private void OnPlayerTakeDamage(float health, float damageTaken)
    {
        playerTreeBar.fillAmount = health / MainTree.Instance.GetDefaultHealth();
    }

    private void OnEnemyBaseTakeDamage(float health, float damageTaken)
    {
        enemyBaseBar.fillAmount = health / EnemyBase.Instance.GetDefaultHealth();
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
        nextRoundButtonClosing = false;
        timeVal = 1;

        roundsText.text = $"Upcoming Round {RoundSystem.roundNumber}";
    }

    private void OnGameStateChange(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Defeat:
                ShowUI(false);
                loseScreen.SetActive(true);
                break;
            case GameState.Won:
                ShowUI(false);
                winScreen.SetActive(true);
                break;
        }
    }

    #endregion

    public void TransitionToNextRound()
    {
        if (timeVal <= 0)
        {
            nextRoundButtonClosing = true;
            timeVal = 1;

            RoundSystem.StartRound();
        }
    }

    public void ShowUI(bool show = true)
    {
        for (int i = 0; i < UIToHide.Count; i++)
        {
            if (UIToHide[i] != null)
            {
                UIToHide[i].SetActive(show);
            }
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
