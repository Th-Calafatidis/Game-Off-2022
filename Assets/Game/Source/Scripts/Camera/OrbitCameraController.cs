// /* --------------------
// -----------------------
// Creation date: 06/11/2022
// Author: Alex
// Description: Implements a camera controller where the camera rotates around a focus point.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCameraController : MonoBehaviour
{
    [Header("Camera settings")]
    [SerializeField] private Transform m_focalPoint;
    [SerializeField] private float m_sensitivity;
    [SerializeField] private float m_zoomSpeed;

    private void LateUpdate()
    {
        CursorHiding();
        Rotation();
        Zooming();
    }

    /// <summary>
    /// Hides and locks the cursor when holding down right click.
    /// </summary>
    private void CursorHiding()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            // Here we use confined instead of lock, so that cursor will stay in place when
            // stopping to move camera.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Handles the rotation of the camera. Will not rotate unless the right mouse button is held down.
    /// </summary>
    private void Rotation()
    {
        if (!Input.GetKey(KeyCode.Mouse1)) return;

        // Mouse input does not have to be multiplied by deltaTime, as it is already done by Unity.
        float mouseX = Input.GetAxis("Mouse X") * m_sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * m_sensitivity;

        // Rotate camera around focal point
        transform.RotateAround(m_focalPoint.position, Vector3.up, mouseX);
        transform.RotateAround(m_focalPoint.position, transform.right, -mouseY);
    }

    /// <summary>
    /// Moves the camera foward or backwards depending on the scroll wheel input.
    /// </summary>
    private void Zooming()
    {
        // Zoom camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * m_zoomSpeed;
    }
}
