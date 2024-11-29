using UnityEngine;
using UnityEngine.InputSystem;

// [RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float speedMove;
    public InputActionAsset inputActions;
    public Transform cameraTransform;

    private CharacterController m_characterController;
    private Rigidbody m_rigidbody;
    private Vector3 m_cameraOffset;
    private InputAction m_moveAction;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_moveAction = inputActions.FindAction("Player/Move");
        m_moveAction.Enable();

        m_cameraOffset = cameraTransform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = m_moveAction.ReadValue<Vector2>();
        // if (move.magnitude > 0)
        if (m_characterController)
        {
            Vector3 dir = new Vector3(move.x, 0f, move.y);
            m_characterController.Move(Physics.gravity * Time.deltaTime);
            m_characterController.Move(dir * Time.deltaTime * speedMove);
            //m_characterController.SimpleMove(dir * speedMove);

            // transform.position += dir * Time.deltaTime * speedMove;
            // transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void FixedUpdate()
    {
        Vector2 move = m_moveAction.ReadValue<Vector2>();
        if (m_rigidbody && move.sqrMagnitude > 0)
        {
            Vector3 dir = new Vector3(move.x, 0f, move.y);
            Vector3 newPos = m_rigidbody.position + (dir * Time.deltaTime * speedMove);
            // m_rigidbody.MovePosition(newPos);
            // m_rigidbody.linearVelocity = (dir * Time.deltaTime * speedMove);
            // if (m_rigidbody.linearVelocity.magnitude > speedMove)
            // {
            //     m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * speedMove;
            // }
            m_rigidbody.AddForce(dir * speedMove, ForceMode.Acceleration);
            // Debug.Log($"{dir} - {newPos}");
        }
    }

    private void LateUpdate()
    {
        cameraTransform.position = transform.position + m_cameraOffset;
    }
}
