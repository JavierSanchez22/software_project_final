using System;
using UnityEngine;

public class PlayerCoordinat : MonoBehaviour {
	// Eventos públicos que otros scripts (como la Cámara) escuchan
	public static event Action OnInvertedPosition;
	public static event Action OnReachedSwapPoint;
	public static event Action OnShouldResetCamera;

	[Header("Child Components")]
	[SerializeField] private PlayerInput _input;
	[SerializeField] private PlayerMovement _movement;
	[SerializeField] private PlayerVisuals _visuals;

	[Header("Scene References")]
	[SerializeField] private CameraFollow _camera;
	[SerializeField] private Transform _swapBridgePoint;

	// Almacenamos la pos/rot inicial para resetear
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;

	private void Awake() {
		// "Cableamos" los componentes entre sí
		_movement.Initialize(_input);
		_visuals.Initialize(_input, _movement);

		_initialPosition = transform.position;
		_initialRotation = transform.rotation;
	}
	
	private void OnEnable() {
		GameEvents.OnPrepareContinue += () => OnShouldResetCamera?.Invoke();
	}
	
	private void OnDisable() {
		GameEvents.OnPrepareContinue -= () => OnShouldResetCamera?.Invoke();
	}

	private void Update() {
		// El coordinador es responsable de la lógica que
		// no pertenece a ningún hijo, como comprobar puntos de referencia.
		if (HaveReachedSwapPoint())
			OnReachedSwapPoint?.Invoke();
	}
	
	private void OnTriggerEnter(Collider other) {
		// Lógica de colisión que NO es de obstáculos (ej. muertes, triggers)
		GameState gsm = FindObjectOfType<GameState>();
		if (gsm == null) return;
		
		if (other.gameObject.CompareTag("Finish"))
			gsm.GameOver();
		if (other.gameObject.CompareTag("Water"))
			gsm.GameOver(Sound.Type.WaterDrip);
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("Lower Floor")) {
			// Podríamos pasar esto a PlayerVisuals si quisiéramos
			_visuals.GetComponent<Animator>().SetTrigger("Fall"); 
		}
	}

	// --- Métodos Públicos que los hijos pueden llamar ---

	public void InvokeInvertedPosition() {
		OnInvertedPosition?.Invoke();
	}
	
	public void ResetPositionAndRotation() {
		// Usado por PlayerMovement durante el Replay
		transform.position = new Vector3(_initialPosition.x, 5.59f, _initialPosition.z);
		transform.rotation = _initialRotation;
	}
	
	private bool HaveReachedSwapPoint() {
		if (_swapBridgePoint == null) return false;
		return _swapBridgePoint.position.x <= transform.position.x;
	}
}