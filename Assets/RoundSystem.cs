using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundSystem : MonoBehaviour
{
    [SerializeField] List<RoundData> roundDataList;

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
        return roundDataList[roundNumber - 1];
    }


    void Awake()
    {
        roundNumber = 1;
        roundDataList = new List<RoundData>();
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
