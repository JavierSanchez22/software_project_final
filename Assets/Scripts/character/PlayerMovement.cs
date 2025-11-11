using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public static bool IsGrounded { get; private set; }

	[Header("Ground Settings")]
	[SerializeField] private Transform _groundCheck;
	[SerializeField] private LayerMask _groundLayerMask;
	private const float _GROUND_CHECK_RADIUS = .2f;

	[Header("Movement Settings")]
	[SerializeField] private float _baseJumpForce = 10f;
	[SerializeField] private float _lowJumpMultiplier = 2f;
	[SerializeField] private float _fallMultiplier = 2.5f;

	[Header("Throwback Settings")]
	[SerializeField] private float _rigibodyThrowbackForceLeft = 2000f;
	[SerializeField] private float _rigibodyThrowbackForceUp = 500f;
	
	[Header("Components")]
	[SerializeField] private Rigidbody _rigidbody;

	private bool _isHoldingJump = false;
	private float _airTime = 0f;
	private int _gravityDirection = 1;
	private int _jumps = 1;
	private int _maxJumps = 1;
    
    private float _currentJumpMultiplier = 1f;
    private float _currentJumpForce => _baseJumpForce * _currentJumpMultiplier;

	private GameState _GameState;

	public event Action OnLanded;

	public void Initialize(PlayerInput input) {
		input.OnJumpPressed += Jump;
		input.OnSwitchPressed += Switch;
		input.OnJumpHeld += (isHolding) => _isHoldingJump = isHolding;
	}

	private void OnEnable() {
		GameEvents.OnGameOver += OnGameOver;
		GameEvents.OnPrepareContinue += ResetPlayerForReplay;
		GameEvents.OnReplay += OnReplay;
	}

	private void OnDisable() {
		GameEvents.OnGameOver -= OnGameOver;
		GameEvents.OnPrepareContinue -= ResetPlayerForReplay;
		GameEvents.OnReplay -= OnReplay;
	}
	
	private void Start() {
		_GameState = FindObjectOfType<GameState>();
		ResetPlayerPhysics();
	}

	private void FixedUpdate() {
		if (_GameState == null || !_GameState.IsGamePlayable)
			return;

		HandleGravity();
		OnLanding();
		OnGround();
		OnAir();
	}

	private void OnGameOver(bool isNewBest) {
		if (this == null) return;
		PlayerDeath();
	}
	
	private void OnReplay() {
		if (this == null || _rigidbody == null) return;
		_rigidbody.detectCollisions = true;
		_rigidbody.useGravity = true;
	}

	private void ResetPlayerForReplay() {
		if (this == null || _rigidbody == null) return;
		_rigidbody.useGravity = false;
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.detectCollisions = false;
		ResetPositionAndRotation();
		ResetConstraint();
		ResetGravity();
	}

	private void Jump() {
		if (_jumps > 0 && _GameState.IsGamePlayable) {
			_rigidbody.velocity = Vector3.up * _currentJumpForce * _gravityDirection;
			AudioService.Instance.PlaySoundOneShot(Sound.Type.Jump, 2);
			_jumps--;
		}
	}

	private void Switch() {
		if (IsGrounded && _GameState.IsGamePlayable) {
			InvertPosition();
		}
	}

	public void RechargeJumps() {
		_jumps = _maxJumps;
	}
    
	public void SetJumpMultiplier(float multiplier) {
		_currentJumpMultiplier = multiplier;
	}
    
    public void AddExtraJumps(int amount) {
		_maxJumps += amount;
		RechargeJumps();
	}

	public void RemoveExtraJumps(int amount) {
		_maxJumps -= amount;
		if (_maxJumps < 1) _maxJumps = 1;
		RechargeJumps();
	}
    
    private void HandleGravity() {
		if (_gravityDirection == 1) {
			if (_rigidbody.velocity.y < 0 && !_isHoldingJump)
				ApplyCustomGravityFall(_rigidbody);
			else if (_rigidbody.velocity.y > 0 && !_isHoldingJump)
				ApplyCustomGravityLowJumpFall(_rigidbody);
		} else {
			if (_rigidbody.velocity.y > 0 && !_isHoldingJump)
				ApplyCustomGravityFall(_rigidbody);
			else if (_rigidbody.velocity.y < 0 && !_isHoldingJump)
				ApplyCustomGravityLowJumpFall(_rigidbody);
		}
	}

	private void ApplyCustomGravityFall(Rigidbody rigidbody) {
		rigidbody.velocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
	}

	private void ApplyCustomGravityLowJumpFall(Rigidbody rigidbody) {
		rigidbody.velocity += Vector3.up * Physics.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
	}

	private void OnLanding() {
		if (_airTime > 0 && IsGrounded) {
			RechargeJumps();
			_airTime = 0;
			OnLanded?.Invoke();
		}
	}

	private void OnGround() => IsGrounded = Physics.CheckSphere(_groundCheck.position, _GROUND_CHECK_RADIUS, _groundLayerMask);

	private void OnAir() {
		if (!IsGrounded)
			_airTime += Time.deltaTime;
	}

	private void InvertPosition() {
		AudioService.Instance.PlaySoundOneShot(Sound.Type.Switch, 2);
		transform.Rotate(new Vector3(0, 0, 180), Space.Self);
		transform.position = new Vector3(transform.position.x, transform.position.y * -1, transform.position.z);
		if (_GameState.IsGamePlayable)
			InvertGravity();
		RechargeJumps();
	}

	private void InvertGravity() {
		_gravityDirection *= -1;
		Physics.gravity *= -1;
	}

	private void ResetGravity() {
		if (Physics.gravity.y > 0)
			InvertGravity();
	}

	private void ResetPlayerPhysics() {
		ResetConstraint();
		ResetGravity();
	}

	private void PlayerDeath() {
		ResetGravity();
		ThrowPlayerBackwards();
	}

	private void ThrowPlayerBackwards() {
		DeconstraintPlayer();
		if (_rigidbody != null)
			_rigidbody.AddForce(Vector3.left * _rigibodyThrowbackForceLeft * Time.fixedDeltaTime +
				transform.up * _rigibodyThrowbackForceUp * Time.fixedDeltaTime, ForceMode.VelocityChange);
	}

	private void DeconstraintPlayer() {
		if (_rigidbody != null)
			_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	private void ResetConstraint() {
		if (_rigidbody != null)
			_rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
	}
    
	private void ResetPositionAndRotation() {
        PlayerCoordinat pc = FindObjectOfType<PlayerCoordinat>();
		if (pc != null) {
			pc.ResetPositionAndRotation();
		}
	}
}