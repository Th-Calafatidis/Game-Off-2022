// /* --------------------
// -----------------------
// Creation date: 05/11/2022
// Author: Alex
// Description: This action will move an entity towards one or multiple positions on the grid.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : ICombatAction
{
    // Tracking of all the points that we want to visit.
    public List<Vector2Int> Destinations { get; private set; }

    // The speed at which movement towards destinations go at.
    private float m_speed;

    // The entity that wants to move
    private Entity m_entity;

    public IEnumerator Execute()
    {
        yield return m_entity.MoveTo(Destinations, m_speed);
    }

    /// <summary>
    /// Constructor for multiple destinations.
    /// </summary>
    /// <param name="entity">The entity that will move</param>
    /// <param name="destinations">All points it will visit.</param>
    /// <param name="speed">Speed to move at.</param>
    public MoveAction(Entity entity, List<Vector2Int> destinations, float speed)
    {
        m_entity = entity;
        Destinations = destinations;
        m_speed = speed;
    }

    /// <summary>
    /// Constructor for only a single destination. Functionally identical to the other constructor.
    /// </summary>
    /// <param name="entity">The entity that will move.</param>
    /// <param name="destination">The destination it will visit.</param>
    /// <param name="speed">Speed to move at.</param>
    public MoveAction(Entity entity, Vector2Int destination, float speed)
    {
        m_entity = entity;
        Destinations = new List<Vector2Int>() { destination };
        m_speed = speed;
    }
}
