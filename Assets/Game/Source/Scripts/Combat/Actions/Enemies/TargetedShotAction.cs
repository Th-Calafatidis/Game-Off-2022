// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: This is an action that will target a unit (specifically the player) directly instead of a tile.
//              The shot will then be fired, but will stop at the first obstacle it hits, and deal damage if possible.
//              This way, the player can position between the enemy and an obstacle to block the shot.
//
// Edited: By Theodore on 28/11/2022
// Added: onCompleted Action in TargetedShotAction constructor and m_onCompleted Action variable.
// Also the call of this delegate when the Execute IEnumerator ends so that i can set the animations accordingly
// at that time through the according Enemy scripts.
// -----------------------
// ------------------- */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetedShotAction : ICombatAction
{
    private Unit m_sender;
    private int m_damage;
    private Action m_onExecute;
    private Action m_onCompleted;

    public IEnumerator Execute()
    {
        m_onExecute();

        // There will only ever be one player on the grid, so we can safely assume that the first unit in the list is the player.
        Unit player = Grid.Instance.GetUnitsOfType<Player>()[0];

        List<Vector2Int> tilesToPlayer = Grid.Instance.BresenhamLine(m_sender.GridPosition.x, m_sender.GridPosition.y,
                                                                     player.GridPosition.x, player.GridPosition.y);

        // First tile in list will be the sender's position, therefore we skip index 0.
        for (int i = 1; i < tilesToPlayer.Count; i++)
        {
            // Since a unit counts as occupying a tile, we have to check for that manually first.
            Unit hitUnit = Grid.Instance.GetUnitAt(tilesToPlayer[i]);
            if (hitUnit != null)
            {
                hitUnit.TakeDamage(m_damage);
                break;
            }

            if (!Grid.Instance.IsTileFree(tilesToPlayer[i]))
            {
                // Hit an obstructed tile that is not a unit, so we just stop here.
                break;
            }
        }

        yield return 0;

        m_onCompleted();
    }

    public TargetedShotAction(Unit sender, int damage, Action onStart, Action onComplete)
    {
        m_onExecute = onStart;
        m_onCompleted = onComplete;
        m_sender = sender;
        m_damage = damage;
    }
}
