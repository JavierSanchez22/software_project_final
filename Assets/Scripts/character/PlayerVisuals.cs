using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerVisuals : MonoBehaviour {

	[Header("Effect Settings")]
	[SerializeField] private float _teleportSpeed = 15f;
	[SerializeField] private float _idleAnimationRepeatRate = 5f;

	[Header("Components")]
	[SerializeField] private Animator _animator;
	[SerializeField] private Renderer _renderer;

	private List<Material> _materials = new List<Material>();
	private bool _canTeleport = true;
	private float _idleAnimationsTimer = 0;
	private GameState _GameState;

	public void Initialize(PlayerInput input, PlayerMovement movement) {
		// Suscribirse a los eventos de Input y Movimiento
		input.OnJumpPressed += OnJumpPressed;
		input.OnSwitchPressed += StartDissolving;
		movement.OnLanded += OnLanded;
	}
	
	private void OnEnable() {
		GameEvents.OnPlay += StartRun;
		GameEvents.OnGameOver += OnGameOver;
		SkinsSystem.OnEndOfChangeSkin += AssignMaterials;
		GameEvents.OnPrepareContinue += OnPrepareContinue;
		GameEvents.OnReplay += StartRun;
	}

	private void OnDisable() {
		GameEvents.OnPlay -= StartRun;
		GameEvents.OnGameOver -= OnGameOver;
		SkinsSystem.OnEndOfChangeSkin -= AssignMaterials;
		GameEvents.OnPrepareContinue -= OnPrepareContinue;
		GameEvents.OnReplay -= StartRun;
	}

	private void Awake() {
		AssignMaterials();
	}
	
	private void Start() {
		_GameState = FindObjectOfType<GameState>();
	}

	private void Update() {
		if (_GameState != null && !_GameState.IsGameRunning)
			GameUtilities.CallRepeating(HandleIdleAnimation, ref _idleAnimationsTimer, _idleAnimationRepeatRate);
	}

    // --- MÉTODOS DE EVENTOS "BLINDADOS" ---

	private void OnJumpPressed() {
		if (this == null || _animator == null) return;
		_animator.SetBool("Jump", true);
	}

	private void OnLanded() {
		if (this == null || _animator == null) return;
		_animator.SetBool("Jump", false);
	}

	private void OnGameOver(bool isNewBest) {
		if (this == null || _animator == null) return;
		_animator.SetTrigger("Die");
	}

	private void OnPrepareContinue() {
		if (this == null || _animator == null) return;
		_animator.SetTrigger("Fall");
	}

	private void StartRun() {
		if (this == null || _animator == null) return;
		_animator.SetTrigger("Run");
	}

    // --- FIN DE MÉTODOS DE EVENTOS "BLINDADOS" ---

	public void AssignMaterials() {
		_materials.Clear();
		foreach (Material material in _renderer.materials)
			_materials.Add(material);
	}

	private void StartDissolving() {
		if (PlayerMovement.IsGrounded && _canTeleport && _GameState.IsGamePlayable) {
			StartCoroutine(DissolveDown());
		}
	}
	
	private IEnumerator DissolveDown() {
		_canTeleport = false;
		float height = 3.4f;
		while (height >= -1) {
			height -= Time.deltaTime * _teleportSpeed;
			SetCutoffHeights(height);
			yield return null;
		}
		yield return null;
		
		PlayerCoordinat pc = FindObjectOfType<PlayerCoordinat>();
		if (pc != null) {
			pc.InvokeInvertedPosition();
		}
		StartCoroutine(DissolveUp());
	}

	private void SetCutoffHeights(float height) {
		foreach (Material material in _materials)
			material.SetFloat("_CutoffHeight", height);
	}

	private IEnumerator DissolveUp() {
		float height = -1f;
		while (height <= 3.4f) {
			height += Time.deltaTime * _teleportSpeed;
			SetCutoffHeights(height);
			yield return null;
		}
		yield return null;
		_canTeleport = true;
	}

	private void HandleIdleAnimation() {
		float idleEventMarking = Random.Range(0f, 1f);
		if (idleEventMarking > 0f && idleEventMarking < 0.1)
			_animator.SetTrigger("IdleEvent1");
		if (idleEventMarking > 0.9 && idleEventMarking < 1)
			_animator.SetTrigger("IdleEvent2");
	}
}