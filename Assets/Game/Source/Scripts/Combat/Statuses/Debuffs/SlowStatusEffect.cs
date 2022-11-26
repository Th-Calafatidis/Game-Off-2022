// /* --------------------
// -----------------------
// Creation date: 09/11/2022
// Author: Alex
// Description: This effect will reduce movement speed of an entity by 50%. Because movement is defined by int values,
//              the movement speed will be divided by 2, then rounded down to floor value. It will always be
//              clamped to a minimum of 1.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowStatusEffect : StatusEffect
{
    // Only entity will have the method for changing movement speed.
    // Therefore, we will check if the unit is an entity, and only proceed
    // if it is.
    private Entity m_entity;

    private int m_duration;
    private int m_originalSpeed;

    public override void OnEffectAdd(Unit unit)
    {
        // Is the unit an entity?
        m_entity = unit.GetComponent<Entity>();

        // If it is, then we can proceed.
        if (m_entity != null)
        {
            m_originalSpeed = m_entity.MovementSpeed;

            // We need to perform an extra check to see if it's a player or enemy.
            // With enemies, we want to reduce the movement speed. With player, since
            // speed is used as cost, we need to double it instead.
            if (m_entity is Enemy)
            {
                // Divide by 2 and floor. Clamped to minimum 1
                int newSpeed = Mathf.FloorToInt((m_entity.MovementSpeed / 2));
                newSpeed = (newSpeed < 1) ? 1 : newSpeed;
                m_entity.MovementSpeed = newSpeed;
            }
            else
            {
                m_entity.MovementSpeed *= 2;
            }  
        }
    }

    public override void OnEffectRemoved(Unit unit)
    {
        m_entity.MovementSpeed = m_originalSpeed;
    }

    public override void OnEndTurn(Unit unit)
    {
        m_duration--;

        if (m_duration <= 0)
        {
            unit.RemoveStatusEffect(this);
        }
    }

    public SlowStatusEffect(int duraction)
    {
        m_duration = duraction;
    }
}
