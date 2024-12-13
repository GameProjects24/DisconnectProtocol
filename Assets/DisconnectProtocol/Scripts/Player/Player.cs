using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float rotationSpeed = 10f;

    public Transform cameraTransform;
    public float lookSensitivity = 1f;
    public float maxLookAngle = 80f;

    private CharacterController m_characterController;
    private float currentSpeed;
    private float verticalRotation = 0f;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
    }

    public void Move(Vector2 moveInput)
    {
        if (m_characterController)
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
            moveDir = transform.TransformDirection(moveDir); // Преобразуем в локальные координаты
            m_characterController.SimpleMove(moveDir * currentSpeed);
        }
    }

    public void Look(Vector2 lookInput)
    {
        // Горизонтальный поворот (вокруг оси Y)
        transform.Rotate(Vector3.up, lookInput.x * lookSensitivity);

        // Вертикальный поворот (вокруг оси X)
        verticalRotation -= lookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    public void SetSprint(bool isSprinting)
    {
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }
}
