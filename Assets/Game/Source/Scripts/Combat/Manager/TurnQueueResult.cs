using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TurnQueueResult
{
    public Unit Unit;
    public ICombatAction Action;

    public TurnQueueResult(Unit unit, ICombatAction action)
    {
        this.Unit = unit;
        this.Action = action;
    }
}
