using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera m_cam;

    private void Awake()
    {
        m_cam = Camera.main;
    }

    void Update()
    {
        transform.LookAt(m_cam.transform);
    }
}
