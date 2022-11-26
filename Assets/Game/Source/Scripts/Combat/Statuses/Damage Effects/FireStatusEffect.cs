// /* --------------------
// -----------------------
// Creation date: 07/11/2022
// Author: Alex
// Description: Will deal damage to the unit at the start of their turn.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStatusEffect : StatusEffect
{
    private int m_damage = 1;
    private int m_duration = 2;

    public FireStatusEffect()
    {
        
    }

    public FireStatusEffect(int damage, int duration)
    {
        m_damage = damage;
        m_duration = duration;
    }

    public override void OnStartTurn(Unit unit)
    {
        Debug.Log("Burned " + unit.name + " for " + m_damage + " damage!");

        // As always, check that there is a IDamagable interface
        IDamagable damagable = unit.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.TakeDamage(m_damage);
        }

        // Update the duration
        m_duration--;
        if (m_duration <= 0)
        {
            unit.RemoveStatusEffect(this);
        }
    }
}
