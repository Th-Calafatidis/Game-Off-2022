// /* --------------------
// -----------------------
// Creation date: 14/11/2022
// Author: Alex
// Description: This action will move the unit towards the given position, and stop at first obstacle.
//              If possible, the thing that stopped the charge will take damage and be knocked back.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAction : ICombatAction
{
    private Unit m_unit;
    private Vector2Int m_targetPosition;
    private int m_damage;
    private int m_knockbackForce;
    private int m_speed;

    public IEnumerator Execute()
    {
        // Keeps moving in the direction of the target position until either unit arrives at that
        // position, or something is hit. If something is hit, apply knockback and damage.
        Vector2Int currentPosition = m_unit.GridPosition;
        Direction direction = Grid.Instance.GetDirectionTo(m_targetPosition, currentPosition);

        Vector2Int nextPosition = Grid.Instance.PositionWithDirection(currentPosition, direction);
        while (nextPosition != m_targetPosition)
        {
            // Is the next position blocked?
            if (!Grid.Instance.IsTileFree(nextPosition))
            {
                // If so, apply knockback and damage to the thing that was hit.
                Unit unit = Grid.Instance.GetUnitAt(nextPosition);
                if (unit != null)
                {
                    unit.TakeDamage(m_damage);
                    unit.Push(direction, m_knockbackForce);
                }

                // Stop the charge.
                break;
            }

            else
            {
                // Move to the next position.
                yield return m_unit.MoveTo(new List<Vector2Int> { nextPosition }, m_speed);

                // Update the current position.
                currentPosition = m_unit.GridPosition;
                nextPosition = Grid.Instance.PositionWithDirection(currentPosition, direction);
            }
        }
    }

    public ChargeAction(Unit unit, Vector2Int targetPosition, int damage, int knockbackForce, int speed)
    {
        m_unit = unit;
        m_targetPosition = targetPosition;
        m_damage = damage;
        m_knockbackForce = knockbackForce;
        m_speed = speed;
    }
}
