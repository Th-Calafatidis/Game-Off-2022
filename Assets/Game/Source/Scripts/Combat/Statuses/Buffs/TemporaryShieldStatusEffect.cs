// -----------------------
// Creation date: 08/11/2022
// Author: Alex
// Description: Adds a shield that will expire after a certain amount of time.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryShieldStatusEffect : StatusEffect
{
    private int m_shieldAmount;
    private int m_duration;

    public override void OnEndTurn(Unit unit)
    {
        // Every time the turn of the unit ends, we reduce the duration of the shield by 1 and remove it when
        // the duration reaches 0.
        if (m_duration > 0)
        {
            m_duration--;
        }
        else
        {
            unit.RemoveStatusEffect(this);
        }
    }

    public override void OnEffectAdd(Unit unit)
    {
        unit.SetShield(m_shieldAmount);
    }

    public override void OnEffectRemoved(Unit unit)
    {
        unit.SetShield(0);
    }

    public TemporaryShieldStatusEffect(int shieldAmount, int duration)
    {
        m_shieldAmount = shieldAmount;
        m_duration = duration;
    }
}
