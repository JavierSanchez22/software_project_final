using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePlayHUD : MonoBehaviour {

	[Header("Components")]
	[SerializeField] private TMP_Text _scoreText;
	[SerializeField] private TMP_Text _coinsText;
	[SerializeField] private GameObject _switch;
	[SerializeField] private GameObject _jump;

	private GameState _GameState;

	private void OnEnable() {
		GameEvents.OnUpdateScore += UpdateScore;
		GameEvents.OnUpdateCoins += UpdateCoins;
		GameEvents.OnPrepareContinue += () => ContinueHubVisibility(true);
		GameEvents.OnContinue += () => ContinueHubVisibility(false);
		GameEvents.OnReplay += () => ContinueHubVisibility(false);
	}

	private void OnDisable() {
		GameEvents.OnUpdateScore -= UpdateScore;
		GameEvents.OnUpdateCoins -= UpdateCoins;
		GameEvents.OnPrepareContinue -= () => ContinueHubVisibility(true);
		GameEvents.OnContinue -= () => ContinueHubVisibility(false);
		GameEvents.OnReplay -= () => ContinueHubVisibility(false);
	}

	private void Start() {
		_GameState = FindObjectOfType<GameState>();
	}

	private void Update() {
		if (_GameState == null) return;
		HandleButtonsInteractibility(_GameState.IsGamePaused);
	}
	
	private void UpdateScore(int score) {
		if (_scoreText != null)
			_scoreText.text = score.ToString();
	}

	private void UpdateCoins(int coins) {
		if (_coinsText != null)
			_coinsText.text = coins.ToString();
	}

	private void HandleButtonsInteractibility(bool isPaused) {
		if (_switch != null && _switch.GetComponent<Button>() != null)
			_switch.GetComponent<Button>().interactable = !isPaused;
		if (_jump != null && _jump.GetComponent<Button>() != null)
			_jump.GetComponent<Button>().interactable = !isPaused;
	}

	private void ContinueHubVisibility(bool visibility) {
		if (_switch != null)
			_switch.SetActive(!visibility);
		if (_jump != null)
			_jump.SetActive(!visibility);
	}
}