using UnityEngine;

public class ScoreTracker : MonoBehaviour {
	public int Score { get; private set; }
	public int BestScore { get; private set; }
	public int ShadowScore { get; private set; }
	
	[SerializeField] private float _scoreRate = 1f;
	private float _scoreTimer = 0f;
	private GameState _gameState;

	private void Start() {
		_gameState = FindObjectOfType<GameState>();
		GameEvents.OnUpdateScore += OnScoreUpdated;
	}

	private void OnDestroy() {
		GameEvents.OnUpdateScore -= OnScoreUpdated;
	}

	private void Update() {
		if (_gameState.IsGamePlayable) {
			GameUtilities.CallRepeating(IncreaseScore, ref _scoreTimer, _scoreRate);
		}
	}

	public void LoadBestScore(int loadedBestScore) {
		BestScore = loadedBestScore;
	}

	public void IncreaseScore() {
		Score++;
		GameEvents.InvokeUpdateScore(Score);
	}

	private void OnScoreUpdated(int newScore) {
		ShadowScore++;
	}

	public void ResetShadowScore() {
		ShadowScore = 0;
	}

    // --- ¡AQUÍ ESTÁ EL CÓDIGO NUEVO! ---
	public void ResetScore() {
		Score = 0;
		ShadowScore = 0;
	}
    // --- FIN DEL CÓDIGO NUEVO ---

	public bool CheckForNewBestScore() {
		if (Score > BestScore) {
			BestScore = Score;
			GameEvents.InvokeUpdateBestScore(BestScore);
			return true;
		}
		return false;
	}
}