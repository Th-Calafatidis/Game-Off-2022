// /* --------------------
// -----------------------
// Creation date: 05/11/2022
// Author: Alex
// Description: This action will deal damage to anything at a single tile.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDamageAction : ICombatAction
{
    public Vector2Int TargetPosition { get; private set; }
    public int DamageAmount { get; private set; }

    public IEnumerator Execute()
    {
        // Check if there is a target at the position.
        IDamagable target = Grid.Instance.GetUnitAt(TargetPosition);
        if (target != null)
        {
            target.TakeDamage(DamageAmount);
        }

        yield return 0;
    }

    public SingleDamageAction(Vector2Int targetPosition, int damageAmount)
    {
        TargetPosition = targetPosition;
        DamageAmount = damageAmount;
    }
}
