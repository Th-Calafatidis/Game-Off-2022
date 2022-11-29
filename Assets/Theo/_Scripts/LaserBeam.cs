using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour
{
    private LineRenderer m_lineRenderer;

    private Transform m_playerTransform;
    private SniperEnemy m_sniperTransform;

    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();

        m_sniperTransform = GetComponentInParent<SniperEnemy>();

        m_playerTransform = GameObject.Find("Player").transform; // needs a better way to get the player.
                                                                 // It might also be more usefull if we get the position of the preview too
                                                                 // and switch to the preview when the player tries to move.
    }

    private void Update()
    {


        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                m_lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance));
            }
            else
            {
                m_lineRenderer.SetPosition(1, new Vector3(0, 0, m_sniperTransform.MaxRange));
            }
        }
    }
}
