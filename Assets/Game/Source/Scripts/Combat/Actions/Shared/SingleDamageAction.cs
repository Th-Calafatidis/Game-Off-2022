// /* --------------------
// -----------------------
// Creation date: 05/11/2022
// Author: Alex
// Description: This action will deal damage to anything at a single tile.
// 
// Edited: By Theodore 29/11/2022
// Added: The two Action delegates and the parameters for them in the constructor, defaulted to null.
// -----------------------
// ------------------- */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDamageAction : ICombatAction
{
    public Vector2Int TargetPosition { get; private set; }
    public int DamageAmount { get; private set; }

    private Action m_onExecute;
    private Action m_onCompleted;

    public IEnumerator Execute()
    {
        m_onExecute();

        // Check if there is a target at the position.
        IDamagable target = Grid.Instance.GetUnitAt(TargetPosition);
        if (target != null)
        {
            target.TakeDamage(DamageAmount);
        }

        yield return 0;

        if (m_onCompleted == null)
            yield break;

        m_onCompleted();
    }

    public SingleDamageAction(Vector2Int targetPosition, int damageAmount, Action onStart = null, Action onComplete = null)
    {
        m_onExecute = onStart;
        m_onCompleted = onComplete;
        TargetPosition = targetPosition;
        DamageAmount = damageAmount;
    }
}
