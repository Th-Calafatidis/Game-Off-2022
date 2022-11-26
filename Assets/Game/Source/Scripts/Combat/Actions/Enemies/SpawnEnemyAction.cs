// /* --------------------
// -----------------------
// Creation date: 10/11/2022
// Author: Alex
// Description: Spawns an enemy on the map.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyAction : ICombatAction
{
    // The enemy that will be spawned.
    private GameObject m_enemyPrefab;

    // Position to spawn enemy
    private Vector2Int m_spawnPosition;

    public IEnumerator Execute()
    {
        Grid.Instance.SpawnUnit(m_enemyPrefab, m_spawnPosition);
        yield return 0;
    }

    public SpawnEnemyAction(GameObject enemyPrefab, Vector2Int spawnPosition)
    {
        m_enemyPrefab = enemyPrefab;
        m_spawnPosition = spawnPosition;
    }

}
