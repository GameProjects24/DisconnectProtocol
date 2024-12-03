using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float speedMove = 5;
    public float speedRotation = 30;    

    private CharacterController m_characterController;
    
    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector2 moveInput)
    {
        if (m_characterController)
        {
            Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y);
            m_characterController.SimpleMove(dir * speedMove);
            
            if (moveInput.sqrMagnitude > 0)
            {
                // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), speedRotation * Time.deltaTime);
            }
        }
    }
}
