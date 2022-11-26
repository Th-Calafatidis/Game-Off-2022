// /* --------------------
// -----------------------
// Creation date: 10/11/2022
// Author: Alex
// Description: A ranged enemy that will attack from a distance.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Ranged attack")]
    [SerializeField] private int m_range;
    [SerializeField] private Hazard m_hazardType;
    [SerializeField] private int m_hazardDuration;

    public override void DetermineAction()
    {
        //// First determine if the enemy is in range of the player. Skip action otherwise.
        //if (Grid.Instance.GetDistance(GridPosition, GetPlayer().GridPosition) > m_range)
        //    return;

        //// Get all tiles around the player
        //List<Vector2Int> targetTiles = Grid.Instance.GetSurroundingTiles(GetPlayer().GridPosition, 1);
        //targetTiles.Add(GetPlayer().GridPosition);

        //// Now we have to create the hazard
        //ICombatAction hazardCreation = new CreateHazardAction(targetTiles, m_hazardType, m_hazardDuration);
        //SetAction(hazardCreation);

        Debug.Log(LineOfSightToPlayer());
    }

    public override void DetermineMove()
    {
        
    }
}
