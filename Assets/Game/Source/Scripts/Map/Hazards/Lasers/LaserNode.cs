// /* --------------------
// -----------------------
// Creation date: 13/11/2022
// Author: Alex
// Description: A laser node connects to other laser nodes via a LaserNodeConnection to create a laser beam between them
//              that can be turned on and off either manually or on a timer.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserNode : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] private List<LaserNodeConnection> m_connections;

    private void Start()
    {
        // Subscribes to the on new round event from battle manager,
        // since timer will be updated on the start of each round.
        BattleManager.Instance.OnRoundStart += UpdateLaser;

        // Initialize the connections
        m_connections.ForEach(c => c.Initialize(transform));
        UpdateLaser();
    }

    private void UpdateLaser()
    {
        foreach (LaserNodeConnection connection in m_connections)
        {
            connection.Update();
        }
    }
}
