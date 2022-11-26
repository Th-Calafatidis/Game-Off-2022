// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: This effect will transfer the damage taken by the affected unit to the allocated
//              "protector" unit. The actual logic is coded into unit script, so this is just a
//              container for the data, and tracking duration.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectedStatusEffect : StatusEffect
{
    public Unit Target { get; private set; }
    public Unit Protector { get; private set; }

    private int m_duration;

    public override void OnEndTurn(Unit unit)
    {
        // Tick down the duration and remove effect if 0
        m_duration--;
        if (m_duration <= 0)
            unit.RemoveStatusEffect(this);
    }

    public ProtectedStatusEffect(Unit protector, Unit target, int duration)
    {
        Protector = protector;
        Target = target;
        m_duration = duration;
    }
}
