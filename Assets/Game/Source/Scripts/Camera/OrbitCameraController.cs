// /* --------------------
// -----------------------
// Creation date: 06/11/2022
// Author: Alex
// Description: Implements a camera controller where the camera rotates around a focus point.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class OrbitCameraController : MonoBehaviour
{
    [Header("Camera settings")]
    [SerializeField] private Transform m_focalPoint;
    [SerializeField] private float m_sensitivity;
    [SerializeField] private float m_zoomSpeed;
    [SerializeField] private float m_zoomLerp = 0.25f;
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float m_lerpSpeed = 0.25f;

    private Camera cam;

    private void LateUpdate()
    {
        MoveCamera();
        CursorHiding();
        Rotation();
        Zooming();
    }

    private void Awake()
    {
        m_focalPoint.position = transform.position;

        cam = GetComponent<Camera>();
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

        var desiredSize = cam.orthographicSize;

        desiredSize += scroll * -m_zoomSpeed * Time.deltaTime;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, desiredSize, m_zoomLerp);

    }

    private void MoveCamera()
    {

        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(xInput, 0, zInput);

        if (input != Vector3.zero)
        {
            m_focalPoint.Translate(input * m_moveSpeed * Time.deltaTime);

            Vector3 leprPosition = new Vector3(m_focalPoint.position.x, m_focalPoint.position.y, m_focalPoint.position.z);

            transform.position = Vector3.Lerp(transform.position, leprPosition, m_lerpSpeed);
        }
    }

    public void ToggleCameraMode()
    {
        cam.orthographic = !cam.orthographic;
    }

}
