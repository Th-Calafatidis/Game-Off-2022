// /* --------------------
// -----------------------
// Creation date: 07/11/2022
// Author: Alex
// Description: The base for any status effect that can be applied to a unit.
//              It works by adding a status effect to a list on the unit,
//              then the list will be iterated over at the start and end of turn.
//              The status effect will then be able to do whatever it wants.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    /// <summary>
    /// Called when the unit's turn starts
    /// </summary>
    /// <param name="unit"></param>
    public virtual void OnStartTurn(Unit unit) { }

    /// <summary>
    /// Called when the unit's turn ends
    /// </summary>
    /// <param name="unit"></param>
    public virtual void OnEndTurn(Unit unit) { }

    /// <summary>
    /// Called when the status effect is applied to a unit
    /// </summary>
    /// <param name="unit"></param>
    public virtual void OnEffectAdd(Unit unit) { }

    /// <summary>
    /// Called when the status effect is removed from a unit
    /// </summary>
    /// <param name="unit"></param>
    public virtual void OnEffectRemoved(Unit unit) { }
}
