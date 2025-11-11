using UnityEngine;

public class DifficultyManager : MonoBehaviour {
	public float PlayerSpeed { get; private set; }
	
	[SerializeField] private float _initialPlayerSpeed = 12.5f;
	[SerializeField] private float _speedIncrease = .1f;
	[SerializeField] private float _spawnRateIncrease = .005f;
	[SerializeField] private float _dificultyRate = 2f;

	private float _difficultyTimer = 0f;
	private GameState _gameState;
	private HitableSpawner _obstacleHitableSpawner;

	private float _currentBaseSpeed;
	private float _currentSpeedMultiplier = 1f;

	private void Start() {
		_gameState = FindObjectOfType<GameState>();
		_obstacleHitableSpawner = FindObjectOfType<HitableSpawner>(); 
		
		_currentBaseSpeed = _initialPlayerSpeed;
		PlayerSpeed = _currentBaseSpeed * _currentSpeedMultiplier;
	}

	private void LateUpdate() {
		if (_gameState.IsGamePlayable) {
			GameUtilities.CallRepeating(IncreaseDificulty, ref _difficultyTimer, _dificultyRate);
		}
	}

	public void IncreaseDificulty() {
		_currentBaseSpeed += _speedIncrease;
		PlayerSpeed = _currentBaseSpeed * _currentSpeedMultiplier;
        
		if (_obstacleHitableSpawner != null && _obstacleHitableSpawner.repeatRate > 1f)
			_obstacleHitableSpawner.repeatRate -= _spawnRateIncrease;
	}
    
	public void SetSpeedMultiplier(float multiplier) {
		_currentSpeedMultiplier = multiplier;
		PlayerSpeed = _currentBaseSpeed * _currentSpeedMultiplier; 
	}
}