// /* --------------------
// -----------------------
// Creation date: 09/11/2022
// Author: Alex
// Description: This action will apply a status effect to a unit.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusAction : ICombatAction
{
    private Vector2Int m_targetPosition;
    private StatusEffect m_statusEffect;

    public IEnumerator Execute()
    {
        // Check that there is a unit in the target position.
        Unit target = Grid.Instance.GetUnitAt(m_targetPosition);
        if (target != null)
        {
            // Apply the status effect to the target.
            target.AddStatusEffect(m_statusEffect);
        }

        yield return 0;
    }

    public ApplyStatusAction(Vector2Int targetPosition, StatusEffect statusEffect)
    {
        m_targetPosition = targetPosition;
        m_statusEffect = statusEffect;
    }
}
