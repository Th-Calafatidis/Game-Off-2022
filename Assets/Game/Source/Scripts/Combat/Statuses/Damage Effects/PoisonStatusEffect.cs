// -----------------------
// Creation date: 07/11/2022
// Author: Alex
// Description: Will deal damage to the unit at the end of their turn.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonStatusEffect : StatusEffect
{
    private int m_damage = 1;
    private int m_duration = 2;

    public PoisonStatusEffect()
    {
        
    }

    public PoisonStatusEffect(int damage, int duration)
    {
        m_damage = damage;
        m_duration = duration;
    }

    public override void OnEndTurn(Unit unit)
    {
        // As always, check that there is a IDamagable interface
        IDamagable damagable = unit.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.TakeDamage(m_damage);
        }

        // Spawn a poison particle effect at position of unit
        GameObject prefab = Resources.Load<GameObject>("Particles/StatusEffects/Poison Particles");
        GameObject particle = GameObject.Instantiate(prefab, unit.transform.position + Vector3.up, Quaternion.identity);
        particle.AddComponent<TimedDestruction>().Lifetime = 5f;

        // Update the duration
        m_duration--;
        if (m_duration <= 0)
        {
            unit.RemoveStatusEffect(this);
        }
    }
}
