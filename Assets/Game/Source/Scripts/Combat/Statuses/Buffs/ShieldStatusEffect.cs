// -----------------------
// Creation date: 08/11/2022
// Author: Alex
// Description: Gives the unit a shield that will last until broken. Because we only need to run this once,
//              the status effect will remove itself from the unit after the shield is applied.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldStatusEffect : StatusEffect
{
    private int m_shieldAmount;

    public override void OnEffectAdd(Unit unit)
    {
        unit.SetShield(m_shieldAmount);
        unit.RemoveStatusEffect(this);
    }

    public ShieldStatusEffect(int shieldAmount)
    {
        m_shieldAmount = shieldAmount;
    }
}
