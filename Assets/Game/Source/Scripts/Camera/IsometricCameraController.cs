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

public class IsometricCameraController : MonoBehaviour
{
    [Header("Camera settings")]
    [SerializeField] private bool m_allowRotation = true;
    [SerializeField] private Transform m_focalPoint;
    [SerializeField] private float m_sensitivity;
    [SerializeField] private float m_zoomSpeed;
    [SerializeField] private float m_zoomLerp = 0.25f;
    [SerializeField] private float m_moveSpeed = 5f;

    private Vector3 m_input;
    private Camera m_mainCam;

    private void Update()
    {
        GatherInput();
    }

    private void GatherInput()
    {
        m_input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
    }

    private void LateUpdate()
    {
        MoveCamera();
        CursorHiding();
        Zooming();

        if (!m_allowRotation) return;

        Rotation();
    }

    private void Awake()
    {
        m_mainCam = GetComponentInChildren<Camera>();
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
        Vector3 mouseInput = new Vector3(mouseX, mouseY, 0f);

        Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 45f, 0f));

        Vector3 mouseSkewedInput = matrix.MultiplyPoint3x4(mouseInput);

        // Rotate camera around focal point
        transform.RotateAround(m_focalPoint.position, matrix.MultiplyPoint3x4(Vector3.up), mouseSkewedInput.x);
        transform.RotateAround(m_focalPoint.position, matrix.MultiplyPoint3x4(transform.right), -mouseSkewedInput.y);
    }

    /// <summary>
    /// Moves the camera foward or backwards depending on the scroll wheel input.
    /// </summary>
    private void Zooming()
    {
        // Zoom camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (m_mainCam.orthographic)
        {
            m_mainCam.orthographicSize += scroll * -m_zoomSpeed * Time.deltaTime;
        }
        else
        {
            var fov = m_mainCam.fieldOfView;

            fov += scroll * -m_zoomSpeed;

            m_mainCam.fieldOfView = Mathf.Lerp(m_mainCam.fieldOfView, fov, m_zoomLerp);
        }


    }

    private void MoveCamera()
    {
        if (m_input != Vector3.zero)
        {


            Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 45f, 0f));

            Vector3 skewedInput = matrix.MultiplyPoint3x4(m_input);

            //Transform desiredPosition = transform;

            //desiredPosition.Translate(skewedInput * m_moveSpeed * Time.deltaTime);

            //transform.position = Vector3.Lerp(transform.position, desiredPosition.position, m_lerpSpeed);

            transform.Translate(skewedInput * m_moveSpeed * Time.deltaTime);

        }
    }

    public void ToggleCameraMode()
    {
        m_mainCam.orthographic = !m_mainCam.orthographic;
    }

}
