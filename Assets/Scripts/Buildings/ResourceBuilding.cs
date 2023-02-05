using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuilding : Building
{
    [SerializeField] int buildingIncome = 1;

    public override void EnableBuilding()
    {
        if (!reachedByRoots)
        {
            base.EnableBuilding();
            CurrencyManager.Instance.AddIncome(buildingIncome);
        }
    }
    public override void DisableBuilding()
    {
        if (reachedByRoots)
        {
            base.DisableBuilding();
            CurrencyManager.Instance.AddIncome(-buildingIncome);
        }
    }

    public int GetBuildingIncome()
    {
        return buildingIncome;
    }

    protected override void OnRoundEnded()
    {
        if (reachedByRoots)
        {
            base.OnRoundEnded();
            //CurrencyManager.Instance.AddRoots(buildingIncome);is done from currency manager
        }
    }
}
