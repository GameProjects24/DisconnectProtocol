using UnityEngine;
using DisconnectProtocol;
using System.Data.Common;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class PlayerController : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 4.0f;
	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 6.0f;
	[Tooltip("Rotation speed of the character")]
	public float RotationSpeed = 1.0f;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;
	public event System.Action<bool> OnAim;
	
	public event System.Action<bool> OnSprint;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.1f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;
	public bool isGrounded = true;

	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 90.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -90.0f;

	// cinemachine
	private float _cinemachineTargetPitch;

	// player
	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;
	private Vector3 _currentMoveDirection;


	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;


#if ENABLE_INPUT_SYSTEM
	private PlayerInput _playerInput;
#endif
	private CharacterController _controller;
	private PlayerControls _controls;
	private GameObject _mainCamera;
	private Interactor _interactor;

	private WeaponController _weaponController;
	public WeaponController weaponController { get {
		if (_weaponController == null) {
			_weaponController = GetComponentInChildren<WeaponController>();
		}
		return _weaponController;
	}}

	private Inventory _inventory;
	public Inventory inventory {get {
		if (_inventory == null) {
			_inventory = GetComponentInChildren<Inventory>();
		}
		return _inventory;
	}}

	private const float _threshold = 0.01f;

	private float targetSpeed;

	private bool IsCurrentDeviceMouse
	{
		get
		{
#if ENABLE_INPUT_SYSTEM
			return _playerInput.currentControlScheme == "KeyboardMouse";
#else
			return false;
#endif
		}
	}


	private void Awake()
	{
		// get a reference to our main camera
		if (_mainCamera == null)
		{
			_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}
	}

	private void Start()
	{

		_weaponController = GetComponentInChildren<WeaponController>();
		_controller = GetComponent<CharacterController>();
		_controls = GetComponent<PlayerControls>();
#if ENABLE_INPUT_SYSTEM
		_playerInput = GetComponent<PlayerInput>();
#else
		Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
		_interactor = GetComponentInChildren<Interactor>();
	}

	private void Update()
	{
		Move();
		JumpAndGravity();

		if (_controls.fire)
		{
			_weaponController.StartFire();
		}
		else
		{
			_weaponController.StopFire();
		}


		if (_controls.reload)
		{
			_weaponController.Reload();
		}

		if (_controls.changeWeapon)
		{
			_weaponController.ChangeWeapon();
			_controls.changeWeapon = false; // Сброс флага после переключения
		}

		if (_controls.aim && !_weaponController.IsCurWeaponReloading() && isGrounded)
		{
			OnAim?.Invoke(true);
		}
		else
		{
			OnAim?.Invoke(false);
		}

		if (_controls.sprint && !_controls.aim && !_weaponController.IsCurWeaponReloading())
		{
			targetSpeed = SprintSpeed;
			OnSprint?.Invoke(true);
		}
		else
		{
			targetSpeed = MoveSpeed;
			OnSprint?.Invoke(false);
		}

		if (_controls.interact)
		{
			_interactor.Interact();
		}
	}

	private void LateUpdate()
	{
		CameraRotation();
	}

	private void CameraRotation()
	{
		// if there is an input
		if (_controls.look.sqrMagnitude >= _threshold)
		{
			//Don't multiply mouse input by Time.deltaTime
			float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

			_cinemachineTargetPitch += _controls.look.y * RotationSpeed * deltaTimeMultiplier;
			_rotationVelocity = _controls.look.x * RotationSpeed * deltaTimeMultiplier;

			// clamp our pitch rotation
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// Update Cinemachine camera target pitch
			CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

			// rotate the player left and right
			transform.Rotate(Vector3.up * _rotationVelocity);
		}
	}

	private void Move()
	{
		// если игрок не на земле, используем только горизонтальную скорость из предыдущего движения
		if (!isGrounded)
		{
			targetSpeed = _currentMoveDirection.magnitude * targetSpeed; // сохраняем текущую скорость
		}

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (_controls.move == Vector2.zero) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = _controls.analogMovement ? _controls.move.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		// normalise input direction
		Vector3 inputDirection = new Vector3(_controls.move.x, 0.0f, _controls.move.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (_controls.move != Vector2.zero)
		{
			// move
			inputDirection = transform.right * _controls.move.x + transform.forward * _controls.move.y;
		}

		// сохраняем горизонтальное направление при прыжке или падении (эффект импульса)
		if (isGrounded)
		{
			_currentMoveDirection = inputDirection;
		}

		// move the player
		_controller.Move(_currentMoveDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
	}

	private void JumpAndGravity()
	{
		isGrounded = _controller.isGrounded;
		if (isGrounded)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;

			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			// Jump
			if (_controls.jump && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
			}

			// jump timeout
			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			_jumpTimeoutDelta = JumpTimeout;

			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}

			// if we are not grounded, do not jump
			_controls.jump = false;
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}


	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}
}
