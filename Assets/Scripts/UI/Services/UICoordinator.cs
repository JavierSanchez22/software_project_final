using System;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	
	[Header("Panel References")]
	[SerializeField] private GameObject _initMenu;
	[SerializeField] private GameObject _gameUI;
	[SerializeField] private GameObject _pauseMenu;
	[SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _loadingScreen;

	private GameState _GameState;

	private void OnEnable() {
		GameEvents.OnPlay += OnPlay;
		GameEvents.OnPause += OnPause;
		GameEvents.OnResume += OnResume;
		GameEvents.OnGameOver += isThereNewBestScore => OnGameOver();
	}

	private void OnDisable() {
		GameEvents.OnPlay -= OnPlay;
		GameEvents.OnPause -= OnPause;
		GameEvents.OnResume -= OnResume;
		GameEvents.OnGameOver -= isThereNewBestScore => OnGameOver();
	}

	private void Start() {
		_GameState = FindObjectOfType<GameState>();
        
        // El Start AHORA ESTÁ CASI VACÍO.
        // El Bootstrapper tiene el control.
        // Nos aseguramos de que todo esté apagado EXCEPTO el loading screen
        // (que se supone está encendido por defecto en la escena).
        SetMenuVisibility(_initMenu, false);
		_gameUI.SetActive(false);
		SetMenuVisibility(_pauseMenu, false);
		SetMenuVisibility(_gameOverMenu, false);
	}
	
    // --- ¡MÉTODO NUEVO! ---
    // El Bootstrapper llama a esto cuando la carga termina.
    public void ShowMainMenu() {
        if (_loadingScreen != null) {
            _loadingScreen.SetActive(false);
        }
        SetMenuVisibility(_initMenu, true);
    }
	
	private void Update() {
		if (_GameState != null) {
			_gameUI.SetActive(_GameState.IsGameRunning);
		}
	}

	private void OnPlay() => SetMenusVisibility(false, false, false);

	private void OnPause() {
		SetMenusVisibility(false, true, false);
		if (_GameState != null && !_GameState.IsGameRunning)
			SetMenuVisibility(_initMenu, true);
	}

	private void OnResume() {
		if (_GameState != null) {
			if (_GameState.IsGamePaused)
				SetMenuVisibility(_initMenu, false);
			else if (!_GameState.IsGameRunning)
				SetMenuVisibility(_initMenu, true);
		}
		SetMenuVisibility(_pauseMenu, false);
	}

	private void OnGameOver() {
		SetMenusVisibility(false, false, true);
	}

	private void SetMenusVisibility(bool mainVisibility, bool pauseVisibility, bool gameOverVisibility) {
		SetMenuVisibility(_initMenu, mainVisibility);
		SetMenuVisibility(_pauseMenu, pauseVisibility);
		SetMenuVisibility(_gameOverMenu, gameOverVisibility);
	}

	private void SetMenuVisibility(GameObject menu, bool visibility) {
		if (menu != null)
			menu.SetActive(visibility);
	}
}