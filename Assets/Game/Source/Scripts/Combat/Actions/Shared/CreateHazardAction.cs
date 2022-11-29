using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHazardAction : ICombatAction
{
    private List<Vector2Int> m_positions;
    private Hazard m_hazardType;
    private int m_hazardDamage;
    private int m_hazardDuration;

    private Action m_onExecute;
    private Action m_onCompleted;

    public IEnumerator Execute()
    {
        m_onExecute?.Invoke();

        foreach (Vector2Int position in m_positions)
        {
            // There has to be a check if the tile is not obstructed by a wall, but since units counts as a wall
            // technically, we will first check if there is a unit in the position, as then it is certain we can place,
            // without having to check if tile is obstructed. If no unit, we will just go off obsturction.
            if (Grid.Instance.GetUnitAt(position) != null || Grid.Instance.IsTileFree(position))
            {
                EnvironmentHazard.CreateHazard(m_hazardType, m_hazardDuration, position);
            }
            yield return 0;
        }

        m_onCompleted?.Invoke();
    }

    public CreateHazardAction(List<Vector2Int> positions, Hazard hazardType, int hazardDuration, Action onStart = null, Action onComplete = null)
    {
        m_onExecute = onStart;
        m_onCompleted = onComplete;
        m_positions = positions;
        m_hazardType = hazardType;
        m_hazardDuration = hazardDuration;
    }

    public CreateHazardAction(Vector2Int position, Hazard hazardType, int hazardDuration, Action onStart = null, Action onComplete = null)
    {
        m_onExecute = onStart;
        m_onCompleted = onComplete;
        m_positions = new List<Vector2Int>();
        m_positions.Add(position);
        m_hazardType = hazardType;
        m_hazardDuration = hazardDuration;
    }
}
