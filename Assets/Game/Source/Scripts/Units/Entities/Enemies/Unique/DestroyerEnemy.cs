// /* --------------------
// -----------------------
// Creation date: 14/11/2022
// Author: Alex
// Description: This enemy will attempt to charge towards the player if within range in a straight line.
//              If the player is one tile away, a normal melee attack will be performed instead.
//              If not close enough for melee, but not in a straight line, enemy will throw a grenade causing poison hazard.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerEnemy : Enemy
{
    [Header("Charge")]
    public int m_chargeRange;
    public int m_chargeDamage;
    public int m_chargeSpeed;
    public int m_chargeKnockback;

    [Header("Iron Fist")]
    public int m_fistDamage;
    private bool m_fistQueued;

    [Header("Toxic Grenade")]
    public int m_grenadeRange;
    public int m_grenadeRadius;
    public Hazard m_grenadeHazardType;
    public int m_grenadeHazardDuration;

    public override void DetermineAction()
    {
        ClearHighlights();
        
        // Charge will be prioritized if the enemy is in a straight line, and is more than 1 tile away.
        // If the player is 1 tile away, a normal melee attack will be performed instead.
        // Else, ranged attack will be performed.
        Vector2Int playerPosition = GetPlayer().GridPosition;
        int playerDistance = Grid.Instance.GetDistanceBetweenUnits(this, GetPlayer());

        if (playerDistance <= m_chargeRange && !Grid.Instance.IsAdjacent(GridPosition, playerPosition) 
            && Grid.Instance.InStraightLine(GridPosition, playerPosition))
            Charge();

        else if (Grid.Instance.IsAdjacent(GridPosition, playerPosition))
            Fist();

        else if (playerDistance <= m_grenadeRange)
            Grenade();
    }

    /// <summary>
    /// Charges towards the player. Deals damage to the first unit or obstacle hit.
    /// </summary>
    private void Charge()
    {
        // First we have to determine if we can charge the whole way, or if we have to stop early.
        // If we have to stop, determine what we hit and deal damage to the tile stopping enemy.
        Vector2Int playerPosition = GetPlayer().GridPosition;

        // Determine the direction to the player
        Direction direction = Grid.Instance.GetDirectionTo(playerPosition, GridPosition);

        // Find the tile that enemy will end up on after moving charge distance
        Vector2Int endPosition = Grid.Instance.GetTileInDirection(GridPosition, direction, m_chargeRange);
        CreateHighlight(Grid.Instance.GetTilesBetween(GridPosition, endPosition), Color.red);

        ICombatAction charge = new ChargeAction(this, endPosition, m_chargeDamage, m_chargeKnockback, m_chargeSpeed);
        SetAction(charge);

        SetLine("charge", m_chargeDamage);
    }

    private void Fist()
    {
        // Performs melee attack where player currently is.
        Direction direction = Grid.Instance.GetDirectionTo(GetPlayer().GridPosition, GridPosition);
        Vector2Int attackPosition = Grid.Instance.PositionWithDirection(GridPosition, direction);

        CreateHighlight(attackPosition, Color.red);

        ICombatAction damage = new SingleDamageAction(attackPosition, m_fistDamage);
        SetAction(damage);

        SetLine("fist", m_fistDamage);
    }

    private void Grenade()
    {
        List<Vector2Int> targetTiles = Grid.Instance.GetSurroundingTiles(GetPlayer().GridPosition, m_grenadeRadius);
        targetTiles.Add(GetPlayer().GridPosition);

        CreateHighlight(targetTiles, Color.red);

        // Now we have to create the hazard
        ICombatAction hazardCreation = new CreateHazardAction(targetTiles, m_grenadeHazardType, m_grenadeHazardDuration);
        SetAction(hazardCreation);

        SetLine("grenade");
    }

    public override void DetermineMove()
    {
        Debug.Log(gameObject.name + " moves");

        // Ignore if we are already next to the player
        if (Grid.Instance.IsAdjacent(GridPosition, GetPlayer().GridPosition))
            return;

        // Enemy will always attempt to move to an adjacent tile to the player.
        // Don't move if there are no available positions.
        // TODO: Pick closest possible position instead.
        List<Vector2Int> adjacentPositions = Grid.Instance.GetFreeAdjacentTiles(GetPlayer().GridPosition);
        if (adjacentPositions.Count == 0)
            return;


        // Go through each adjacent tile to determine which one is the closest to enemy current position.
        // Target position is set to 1000 1000 since it will never get to that value naturally, so we can use
        // it to check if it has been set.
        Vector2Int targetPosition = new Vector2Int(1000, 1000);
        foreach (Vector2Int potentialPosition in adjacentPositions)
        {
            int distance = Grid.Instance.GetDistance(GridPosition, potentialPosition);
            if (distance != 0)
            {
                if (targetPosition == new Vector2Int(1000, 1000))
                    targetPosition = potentialPosition;

                else if (distance < Grid.Instance.GetDistance(GridPosition, targetPosition))
                    targetPosition = potentialPosition;
            }
        }
        
        // Move to the closest position
        List<Vector2Int> path = Grid.Instance.GetPath(GridPosition, targetPosition);
        
        if (path == null) return;

        // Remove all points in path that is outside of movement range of enemy.
        if (path.Count > MovementSpeed)
        {
            path.RemoveRange(MovementSpeed, path.Count - MovementSpeed);
        }

        // Perform the move
        ICombatAction move = new MoveAction(this, path, m_physicalMovementSpeed);
        SetAction(move);
    }

    public override void OnFinishedMoving()
    {
        
    }

    public override void OnPushed()
    {
        if (IntendedAction == null) return;

        // Store the type of action intended, as we will make a new of same type later.
        ICombatAction action = IntendedAction;
        ClearAction();

        if (action.GetType() == typeof(ChargeAction))
        {
            Charge();
        }

        else if (action.GetType() == typeof(SingleDamageAction))
        {
            Fist();
        }

        else if (action.GetType() == typeof(CreateHazardAction))
        {
            Grenade();
        }
    }
}
