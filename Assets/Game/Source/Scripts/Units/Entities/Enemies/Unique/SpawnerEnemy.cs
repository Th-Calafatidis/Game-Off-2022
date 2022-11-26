// /* --------------------
// -----------------------
// Creation date: 10/11/2022
// Author: Alex
// Description: This is a type of enemy that will spawn other enemies on the map.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerEnemy : Enemy
{
    [Header("Spawning")]
    [SerializeField] private int m_spawningFrequency;
    [SerializeField] private List<WeightedItem<GameObject>> m_spawnPool;

    private int m_spawnTimer = 0;
    private WeightedSelector<GameObject> m_spawnSelector;

    public override void Awake()
    {
        base.Awake();

        // We set up the selector with the potential enemies to spawn
        m_spawnSelector = new WeightedSelector<GameObject>(m_spawnPool);
    }

    public override void DetermineAction()
    {
        // Since this method is called once every round, we can use it to track rounds for spawning
        if (m_spawnTimer > 0)
        {
            m_spawnTimer -= 1;
            return;
        }

        // If we get here, it means that the spawn timer has reached 0, so we can spawn an enemy.
        // First, we get a random enemy from the spawn pool.
        GameObject enemyToSpawn = m_spawnSelector.Select();

        // Now we pick a random adjecent tiles. Performs a check to eliminate tiles that is not available,
        // and picks from the remaining.
        List<Vector2Int> availableTiles = new List<Vector2Int>();
        foreach (Direction direction in typeof(Direction).GetEnumValues())
        {
            // Is the position free?
            Vector2Int potentialPossition = Grid.Instance.PositionWithDirection(GridPosition, direction);
            if (Grid.Instance.IsTileFree(potentialPossition))
            {
                availableTiles.Add(potentialPossition);
            }
        }

        // Make sure there are at least one available tile
        if (availableTiles.Count > 0)
        {
            // Pick a random tile from the available tiles and spawn an enemy there.
            Vector2Int spawnPosition = availableTiles[Random.Range(0, availableTiles.Count)];

            ICombatAction spawn = new SpawnEnemyAction(enemyToSpawn, spawnPosition);
            SetAction(spawn);
        }

        // Reset the spawn timer
        m_spawnTimer = m_spawningFrequency;
    }

    public override void DetermineMove()
    {
        // Since the spawner is supposed to be immobile, we can simply leave
        // this method empty.
    }
}
