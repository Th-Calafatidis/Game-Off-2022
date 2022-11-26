using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LaserNodeConnection
{
    public LaserNode ConnectedNode;
    public bool Permanent;
    public int Interval;
    public int Duration;

    public bool IsOn { get; private set; }

    private int m_timer;
    private LineRenderer m_line;
    private LaserNode m_origin;

    /// <summary>
    /// Creates the linerendered for the laser visuals.
    /// </summary>
    public void Initialize(Transform origin)
    {
        // Create the line renderer
        m_line = new GameObject("Laser").AddComponent<LineRenderer>();
        m_line.transform.SetParent(origin);
        m_line.transform.localPosition = Vector3.zero;
        m_line.transform.localRotation = Quaternion.identity;
        m_line.transform.localScale = Vector3.one;
        m_line.material = new Material(Shader.Find("Sprites/Default"));
        m_line.startWidth = 0.1f;
        m_line.endWidth = 0.1f;
        m_line.positionCount = 2;
        m_line.SetPosition(0, origin.position);
        m_line.SetPosition(1, ConnectedNode.transform.position);
        m_line.startColor = Color.red;
        m_line.endColor = Color.red;
        m_line.enabled = false;

        // Cache the origin
        m_origin = origin.GetComponent<LaserNode>();
    }

    /// <summary>
    /// Updates the connection
    /// </summary>
    public void Update()
    {
        bool lastOnStatus = IsOn;
        UpdateState();

        // When the laser goes from off to on, spawn the hazards on every tile between the two nodes.
        // This will probably break if the laser is not straight. So make sure it is.
        if (IsOn && !lastOnStatus)
        {           
            // Loop through each tile in between the two nodes.
            Vector2Int start = Grid.Instance.GetGridPosition(m_origin.transform.position);
            Vector2Int end = Grid.Instance.GetGridPosition(ConnectedNode.transform.position);
            
            foreach (Vector2Int position in Grid.Instance.GetTilesBetween(start, end))
            {
                // Hacky workaround since I can't be bothered to add permanent hazards to the grid.
                // If someone makes battle last 9999 turns, they deserve to have the laser be deactivated.
                int duration = (Permanent) ? 9999 : Duration;
                EnvironmentHazard.CreateHazard(Hazard.Laser, duration, position);
            }
        }
    }

    private void UpdateState()
    {
        // If the connection is permanent, it will always be on.
        if (Permanent)
        {
            IsOn = true;
            m_line.enabled = true;
            return;
        }

        // If the timer reaches 0, turn on the laser.
        if (m_timer <= 0)
        {
            IsOn = true;
            m_line.enabled = true;
            m_timer = Interval + Duration;
        }

        // If the timer is less than the duration, turn off the laser.
        if (m_timer <= Duration)
        {
            IsOn = false;
            m_line.enabled = false;
        }

        // Decrease the timer.
        m_timer--;
    }
}
