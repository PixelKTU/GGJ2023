using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    WaitingForRound,
    RoundInprogress,
    Defeat,
    Won
};


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action<GameState> OnGameStateChange;

    public GameState GameState
    {
        get
        {
            return gameState;
        }

        private set
        {
            gameState = value;
            OnGameStateChange?.Invoke(gameState);
        }
    }
    private GameState gameState;

    [Header("Test")]
    [SerializeField] private AudioClip testSound;
    [SerializeField] private AudioClip testLoopSound;

    private float health;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        RoundSystem.roundStartEvent.AddListener(OnRoundStarted);
        RoundSystem.roundEndEvent.AddListener(OnRoundEnded);

        //SoundManager.Instance.PlaySound(testLoopSound, Camera.main.transform, true);
    }


    private void OnDestroy()
    {
        RoundSystem.roundStartEvent.RemoveListener(OnRoundStarted);
        RoundSystem.roundEndEvent.RemoveListener(OnRoundEnded);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SoundManager.Instance.PlaySoundOneShot(testSound);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SoundManager.Instance.PlaySound(testSound, Vector3.zero);
        }
    }

    public void EndGame(bool didWin)
    {
        if (GameState == GameState.Defeat || GameState == GameState.Won) return;

        if (didWin)
        {
            GameState = GameState.Won;
        }
        else
        {
            GameState = GameState.Defeat;
        }
    }


    #region Rounds
    private void OnRoundStarted()
    {
        GameState = GameState.RoundInprogress;
    }

    private void OnRoundEnded()
    {
        GameState = GameState.WaitingForRound;
    }
    #endregion
}