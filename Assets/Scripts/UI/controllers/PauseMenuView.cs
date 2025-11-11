using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour {
	
	[Header("Components")]
	[SerializeField] private Slider _bgmSlider;
	[SerializeField] private Slider _sfxSlider;
	[SerializeField] private GameObject _lowGraphicsSign;

	private bool _isPointerOnPauseMenu = false;
	private GameState _GameState;
	private GraphicsSettings _GraphicsSettings;

	private void OnEnable() {
		GameEvents.OnUpdateVolume += UpdateVolumeSlider;
		GameEvents.OnChangedQuality += SelectPauseMenu;
	}

	private void OnDisable() {
		GameEvents.OnUpdateVolume -= UpdateVolumeSlider;
		GameEvents.OnChangedQuality -= SelectPauseMenu;
	}

	private void Start() {
		_GameState = FindObjectOfType<GameState>();
		_GraphicsSettings = FindObjectOfType<GraphicsSettings>();
	}
	
	private void Update() {
		if (_GraphicsSettings != null)
			_lowGraphicsSign.SetActive(_GraphicsSettings.IsCurrentlyLowGraphics);
	}

	public void OnDeselectPauseMenu() {
		if (!_isPointerOnPauseMenu) {
			AudioService.Instance.PlaySoundOneShot(Sound.Type.UIClick, 2);
			if (_GameState != null)
				_GameState.Resume();
		}
	}

	public void OnPointerEnterPauseMenu() => _isPointerOnPauseMenu = true;
	public void OnPointerExitPauseMenu() => _isPointerOnPauseMenu = false;
	public void SelectPauseMenu() => EventSystem.current.SetSelectedGameObject(this.gameObject);

	private void UpdateVolumeSlider(int track, float volume) {
		if (track == 1 && _bgmSlider != null)
			_bgmSlider.value = volume;
		else if (track == 2 && _sfxSlider != null)
			_sfxSlider.value = volume;
	}

	public void OnResumeButton() {
		if (_GameState != null) {
			_GameState.Resume();
		}
		
		if (_GraphicsSettings != null) {
			SaveSystem.SaveSettings(AudioService.Instance.GetTrackVolume(1), AudioService.Instance.GetTrackVolume(2), _GraphicsSettings.IsCurrentlyLowGraphics);
		}
	}
}