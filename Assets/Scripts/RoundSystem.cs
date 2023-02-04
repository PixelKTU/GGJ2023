using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundSystem : MonoBehaviour
{
    public static RoundSystem Instance;
    [SerializeField] List<RoundData> roundDataList = new List<RoundData>();

    public static UnityEvent roundStartEvent { get; private set; }
    public static UnityEvent roundEndEvent { get; private set; }
    public static int roundNumber { get; private set; }

    public static bool roundStarted { get; private set; }
    public static void StartRound()
    {
        if (!roundStarted)
        {
            roundStarted = true;
            roundStartEvent.Invoke();
        }
    }

    public static void EndRound()
    {
        if (roundStarted)
        {
            roundNumber++;
            roundStarted = false;
            roundEndEvent.Invoke();
        }
    }

    
    public RoundData GetThisRoundData()
    {
       // Debug.Log(roundNumber - 1);
        return roundDataList[roundNumber - 1];
    }

    IEnumerator testas()
    {

        yield return new WaitForSeconds(5);
        StartRound();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        StartCoroutine(testas());
        roundNumber = 1;
        if (roundStartEvent == null)
        {
            roundStartEvent = new UnityEvent();
        }
        if (roundEndEvent == null)
        {
            roundEndEvent = new UnityEvent();
        }
    }
}
