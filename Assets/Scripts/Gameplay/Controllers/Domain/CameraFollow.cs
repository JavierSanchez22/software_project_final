using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	[Tooltip("The amount of smootheness for the camera movement")]
	[SerializeField] private float _smoothTime = .3f;

	[Tooltip("The default position for when the game starts running")]
	[SerializeField] private Vector3 _defaultPosition;

	private Vector3 _targetPosition = Vector3.zero;
	private Vector3 _velocity = Vector3.zero;
	private GameState _GameState;

	private void OnEnable() {
		PlayerCoordinat.OnInvertedPosition += InvertCamera;
		PlayerCoordinat.OnShouldResetCamera += ResetCamera;
	}

	private void OnDisable() {
		PlayerCoordinat.OnInvertedPosition -= InvertCamera;
		PlayerCoordinat.OnShouldResetCamera -= ResetCamera;
	}

	private void Start() {
		_GameState = FindObjectOfType<GameState>();
		ResetTarget();
	}

	private void Update() {
		if (_GameState != null && _GameState.IsGameRunning)
			transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, _smoothTime);
	}

	public void InvertCamera() => _targetPosition = InvertPosition();

	private Vector3 InvertPosition() {
		_targetPosition = new Vector3(_targetPosition.x, _targetPosition.y * -1, _targetPosition.z);
		return _targetPosition;
	}

	private void ResetTarget() => _targetPosition = _defaultPosition;

	private void ResetCamera() {
		transform.position = _defaultPosition;
		ResetTarget();
	}
}