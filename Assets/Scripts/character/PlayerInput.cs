using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
	public event Action OnJumpPressed;
	public event Action OnSwitchPressed;
	public event Action<bool> OnJumpHeld;

	private bool _isPaused = false;

    // --- ¡MÉTODO NUEVO! ---
	public void ResetState() {
		_isPaused = false;
	}
    // --- FIN DEL MÉTODO NUEVO ---

	private void OnEnable() {
		InputHandler.OnJump += HandleJump;
		InputHandler.OnSwitch += HandleSwitch;
		InputHandler.OnHoldingJump += HandleHold;

		GameEvents.OnPause += () => _isPaused = true;
		GameEvents.OnResume += () => _isPaused = false;
		GameEvents.OnGameOver += (isNewBest) => _isPaused = true;
		GameEvents.OnPlay += () => _isPaused = false;
		GameEvents.OnContinue += () => _isPaused = false;
	}

	private void OnDisable() {
		InputHandler.OnJump -= HandleJump;
		InputHandler.OnSwitch -= HandleSwitch;
		InputHandler.OnHoldingJump -= HandleHold;
		
		GameEvents.OnPause -= () => _isPaused = true;
		GameEvents.OnResume -= () => _isPaused = false;
		GameEvents.OnGameOver -= (isNewBest) => _isPaused = true;
		GameEvents.OnPlay -= () => _isPaused = false;
		GameEvents.OnContinue -= () => _isPaused = false;
	}

	private void HandleJump() {
		if (_isPaused) return;
		OnJumpPressed?.Invoke();
	}

	private void HandleSwitch() {
		if (_isPaused) return;
		OnSwitchPressed?.Invoke();
	}

	private void HandleHold(bool isHolding) {
		if (_isPaused) return;
		OnJumpHeld?.Invoke(isHolding);
	}
}