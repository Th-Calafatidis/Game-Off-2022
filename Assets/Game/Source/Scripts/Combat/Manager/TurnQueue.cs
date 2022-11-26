// /* --------------------
// -----------------------
// Creation date: 06/11/2022
// Author: Alex
// Description: A turn queue is a collection of ICombatActions that can be iterated through.
//              Keep in mind that this is only for the enemies. Player will not use a queue system.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnQueue
{
    // Store all actions in a list
    private List<TurnQueueItem> m_queue;

    /// <summary>
    /// Amount of actions currently in the queue.
    /// </summary>
    public int Count { get { return m_queue.Count; } }

    /// <summary>
    /// Initializes the turn queue.
    /// </summary>
    public void Initialize()
    {
        m_queue = new List<TurnQueueItem>();
    }

    /// <summary>
    /// Adds an action to the queue.
    /// </summary>
    /// <param name="action">The action to add</param>
    public void AddAction(Unit unit, ICombatAction action)
    {
        // Check if the unit already has an action in the queue.
        // If it does, add the action to the list of actions.
        // If it doesn't, create a new queue item and add it to the queue.
        for (int i = 0; i < m_queue.Count; i++)
        {
            if (m_queue[i].Unit == unit)
            {
                m_queue[i].Actions.Add(action);
                return;
            }
        }

        m_queue.Add(new TurnQueueItem(unit, action));
    }

    /// <summary>
    /// Removes an action from the queue.
    /// </summary>
    /// <param name="action">Action to remove</param>
    public void RemoveAction(ICombatAction action)
    {
        // Loop through all actions in the queue and remove turn queue item with the same action.
        for (int i = 0; i < m_queue.Count; i++)
        {
            if (m_queue[i].Actions == action)
            {
                m_queue.RemoveAt(i);
                break;
            }
        }
    }

    public TurnQueueResult GetNext()
    {
        // Get the first item in the queue and remove it.
        TurnQueueItem item = m_queue[0];

        // Extracts the action from the item and removes the item if there are no more
        // actions.
        TurnQueueResult result = new TurnQueueResult(item.Unit, item.Actions[0]);
        if (item.Actions.Count == 1)
        {
            m_queue.RemoveAt(0);
        }
        else
        {
            item.Actions.RemoveAt(0);
        }

        return result;
    }
}
