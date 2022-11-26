// /* --------------------
// -----------------------
// Creation date: 15/11/2022
// Author: Alex
// Description: This enemy will attempt to put a status effect on enemies that will redirect the damage they take onto this enemy.
//              Priority goes as follows: Enemy in range? Go adjacent and apply status effect. Player in range, no enemy? Attack player
//              No enemy no player? TBD
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardianEnemy : Enemy
{
    [Header("Protect")]
    [SerializeField] private int m_protectionDuration;

    [Header("Melee settings")]
    [SerializeField] private int m_meleeDamage;

    private int m_protectionCooldown;

    public override void DetermineAction()
    {
        ClearHighlights();

        // Find all enemies that are adjacent to this.
        List<Enemy> enemies = new List<Enemy>();
        foreach (Enemy enemy in Grid.Instance.GetUnitsOfType<Enemy>())
        {
            if (Grid.Instance.IsAdjacent(GridPosition, enemy.GridPosition))
                if (enemy != this)
                    enemies.Add(enemy);
        }
        if (enemies.Count > 0)
        {
            if (m_protectionCooldown <= 0)
            {
                // Apply protection status effect to one of the adjacent enemies
                Enemy enemy = enemies[Random.Range(0, enemies.Count)];
                ProtectedStatusEffect protect = new ProtectedStatusEffect(this, enemy, m_protectionDuration);
                ICombatAction status = new ApplyStatusAction(enemy.GridPosition, protect);
                CreateHighlight(enemy.GridPosition, Color.blue);
                SetAction(status);
                m_protectionCooldown = m_protectionDuration;

                return;
            }
        }

        // The enemy will try to move towards player in the case that no enemies are adjacent or need protecting.
        // Therefore, we will try to perform melee towards player if adjacent.
        if (Grid.Instance.IsAdjacent(GridPosition, GetPlayer().GridPosition))
        {
            Melee();
            return;
        }
        
    }

    public override void DetermineMove()
    {
        Debug.Log(gameObject.name + " moves");

        // Checks if there are any enemies in walking distance. Move towards them if so.
        // If we had a large amount of enemies in the game, this way of getting all nearby enemies
        // would not be optimal, but since we aren't going to have that many, getting all enemies
        // and checking inidvidual distance is fine.
        List<Vector2Int> enemyPositions = new List<Vector2Int>();
        foreach (Enemy enemy in Grid.Instance.GetUnitsOfType<Enemy>())
        {
            if (Grid.Instance.GetDistanceBetweenUnits(this, enemy) <= MovementSpeed)
                if (enemy != this)
                    enemyPositions.Add(enemy.GridPosition);
        }
        if (enemyPositions.Count > 0 && m_protectionCooldown == 0)
        {
            // We just pick a random enemy and move towards the closest adjacent tile.
            Vector2Int enemyPosition = enemyPositions[Random.Range(0, enemyPositions.Count)];        
            MoveTo(GetClosestAdjacentTile(enemyPosition));
            return;
        }


        // Attempt to move to adjacent tile to player if getting here
        MoveTo(GetClosestAdjacentTile(GetPlayer().GridPosition));
    }

    public override void OnPushed()
    {
        // Check if enemy is intending to melee
        if (IntendedAction == null) return;

        // Store the type of action intended, as we will make a new of same type later.
        ICombatAction action = IntendedAction;
        

        if (action.GetType() == typeof(SingleDamageAction))
        {
            ClearAction();
            Melee();
        }
    }

    private void Melee()
    {
        // Performs melee attack where player currently is.
        Direction direction = Grid.Instance.GetDirectionTo(GetPlayer().GridPosition, GridPosition);
        Vector2Int attackPosition = Grid.Instance.PositionWithDirection(GridPosition, direction);

        CreateHighlight(attackPosition, Color.red);

        ICombatAction damage = new SingleDamageAction(attackPosition, m_meleeDamage);
        SetAction(damage);
    }

    private Vector2Int GetClosestAdjacentTile(Vector2Int position)
    {
        if (Grid.Instance.IsAdjacent(position, GridPosition))
            return GridPosition;

        List<Vector2Int> adjacentPositions = Grid.Instance.GetFreeAdjacentTiles(position);
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
        return targetPosition;
    }
    private void MoveTo(Vector2Int pos)
    {
        if (pos == GridPosition)
            return;

        List<Vector2Int> path = Grid.Instance.GetPath(GridPosition, pos);
        if (path == null) return;

        // Remove all points in path that is outside of movement range of enemy.
        if (path.Count > MovementSpeed)
        {
            path.RemoveRange(MovementSpeed, path.Count - MovementSpeed);
        }

        ICombatAction moveAction = new MoveAction(this, path, m_physicalMovementSpeed);
        SetAction(moveAction);
    }
}
