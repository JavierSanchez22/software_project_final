using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuView : MonoBehaviour {
	public static event Action<bool> OnChangeSkin;

	[Header("Components")]
	[SerializeField] private TMP_Text _initBestScoreText;
	[SerializeField] private TMP_Text _initCoinsText;
	[SerializeField] private Button _initSettingsButton;
	[SerializeField] private Button _playButton;
	[SerializeField] private Button _arrowLeftButton;
	[SerializeField] private Button _arrowRightButton;
	[SerializeField] private Button _unlockButton;

	private GameState _GameState;
    private ScoreTracker _ScoreTracker;
    private PlayerWallet _PlayerWallet;

	private void OnEnable() {
		GameEvents.OnAssignSaveData += UpdateSavedPoints;
		GameEvents.OnUpdateBestScore += UpdateBestScore;
		GameEvents.OnUpdateCoins += UpdateCoins;
	}

	private void OnDisable() {
		GameEvents.OnAssignSaveData -= UpdateSavedPoints;
		GameEvents.OnUpdateBestScore -= UpdateBestScore;
		GameEvents.OnUpdateCoins -= UpdateCoins;
	}

	private void Start() {
		_GameState = FindObjectOfType<GameState>();
        _ScoreTracker = FindObjectOfType<ScoreTracker>();
        _PlayerWallet = FindObjectOfType<PlayerWallet>();

        // Esto arregla el bug de "Score 0". Carga los datos que Bootstrapper
        // acaba de poner en los nuevos managers.
        if (_ScoreTracker != null) {
            UpdateBestScore(_ScoreTracker.BestScore);
        }
        if (_PlayerWallet != null) {
            UpdateCoins(_PlayerWallet.Coins);
        }
        
        // --- ¡AQUÍ ESTÁ LA CORRECCIÓN DEL BOTÓN PLAY! ---
        // Conectamos el botón "Play" por código CADA VEZ que se carga la escena.
        _playButton.onClick.RemoveAllListeners();
        if (_GameState != null) {
             _playButton.onClick.AddListener(_GameState.Play);
             _playButton.onClick.AddListener(PlayButtonClick); // Añade el sonido
        }
	}

	private void Update() {
		if (_GameState == null) return;
		
		_playButton.interactable = SkinsSystem.isCurrentSkinUnlocked;
		HandleButtonsInteractibility(_GameState.IsGamePaused);
	}

	public void ChangeSkinByLeft() => OnChangeSkin?.Invoke(false);
	public void ChangeSkinByRight() => OnChangeSkin?.Invoke(true);
	public void PlayButtonClick() => AudioService.Instance.PlaySoundOneShot(Sound.Type.UIClick, 2);

	private void UpdateSavedPoints(SaveData data) {
		UpdateCoins(data.coins);
		UpdateBestScore(data.bestScore);
	}

	private void UpdateCoins(int coins) {
		if (_initCoinsText != null)
			_initCoinsText.text = coins.ToString();
	}

	private void UpdateBestScore(int bestScore) {
		if (_initBestScoreText != null)
			_initBestScoreText.text = bestScore.ToString();
	}
	
	private void HandleButtonsInteractibility(bool isPaused) {
		_initSettingsButton.interactable = !isPaused;
		_arrowLeftButton.interactable = !isPaused;
		_arrowRightButton.interactable = !isPaused;
		_unlockButton.interactable = !isPaused;
	}
}