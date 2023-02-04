using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    protected bool reachedByRoots = false;

    public virtual void EnableBuilding()
    {
        reachedByRoots = true;
        Debug.Log("Enabled");
    }

    public virtual void DisableBuilding() 
    {
        reachedByRoots = false;    
    }

    protected virtual void OnRoundStarted() { }
    protected virtual void OnRoundEnded() { }

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
}
