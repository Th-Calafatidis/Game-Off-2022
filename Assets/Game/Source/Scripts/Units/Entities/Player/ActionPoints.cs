// /* --------------------
// -----------------------
// Creation date: 05/11/2022
// Author: Alex
// Description: Action points are used to determine how many actions the player can perform each turn.
//              
//              Examples:
//              Move 1 tile - 1 AP
//              Perform melee - 2 AP
// -----------------------
// ------------------- */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPoints
{
    public int MaxActionPoints { get; private set; }
    public int CurrentActionPoints { get; private set; }
    public int ActionPointPoolSize { get; private set; }
    public int PooledActionPoints { get; private set; }
    public int TotalActionPoints
    {
        get { return CurrentActionPoints + PooledActionPoints; }
    }

    public Action OnActionPointChange;

    public ActionPoints(int maxActionPoints, int startingActionPoints, int actionPointPoolSize)
    {
        MaxActionPoints = maxActionPoints;
        CurrentActionPoints = 0;
        ActionPointPoolSize = actionPointPoolSize;
        PooledActionPoints = 0;
    }

    /// <summary>
    /// Spends action points. Prioritizes normal points over pooled points
    /// </summary>
    /// <param name="amount">Amount of points to spend</param>
    public void SpendActionPoints(int amount)
    {
        if (CurrentActionPoints >= amount)
        {
            CurrentActionPoints -= amount;
        }
        else
        {
            PooledActionPoints -= amount - CurrentActionPoints;
            CurrentActionPoints = 0;
        }

        OnActionPointChange?.Invoke();
    }

    /// <summary>
    /// Resets the amount of current action points.
    /// Any unspent action points will go into pool.
    /// </summary>
    public void RestoreActionPoints()
    {
        PooledActionPoints += CurrentActionPoints;
        CurrentActionPoints = MaxActionPoints;

        if (PooledActionPoints > ActionPointPoolSize)
        {
            PooledActionPoints = ActionPointPoolSize;
        }

        OnActionPointChange?.Invoke();
    }
}
