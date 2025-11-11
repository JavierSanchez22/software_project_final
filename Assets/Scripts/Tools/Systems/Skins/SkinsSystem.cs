using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinsSystem : MonoBehaviour {
	public static event Action OnEndOfChangeSkin;
	public static bool isCurrentSkinUnlocked = true;

    // --- LÍNEA MODIFICADA ---
	[SerializeField] private GameObject _playerObject;
    // --- FIN DE LA MODIFICACIÓN ---

	[SerializeField] private List<Skin> _skins = new List<Skin>();
	[SerializeField] private GameObject _unlockGroup;
	[SerializeField] private Button _unlockButton;
	[SerializeField] private TMP_Text _unlockPhraseText;

	private int _currentIndex = 0;
	private PlayerWallet _PlayerWallet;
	private ScoreTracker _ScoreTracker;
	private SkinAbility _currentAbility;
    private Renderer _playerRenderer; // Lo encontraremos automáticamente

	private void OnEnable() => MainMenuView.OnChangeSkin += OnChangeSkin;
	private void OnDisable() => MainMenuView.OnChangeSkin -= OnChangeSkin;

	private void Start() {
		_PlayerWallet = FindObjectOfType<PlayerWallet>();
		_ScoreTracker = FindObjectOfType<ScoreTracker>();
        
        // --- LÍNEA NUEVA ---
        if (_playerObject != null) {
            _playerRenderer = _playerObject.GetComponentInChildren<Renderer>();
        }
        // --- FIN DE LÍNEA NUEVA ---
		
		_currentIndex = 0;
		HandleUnlockGroup();

		SkinsSaveData data = SaveSystem.LoadSkins();
		if (data != null)
			AssignSkinsSaveData(data);
		
		if (_skins.Count > 0 && _skins[0].ability != null && _skins[0].unlockCondition.isUnlocked) {
			_currentAbility = _skins[0].ability;
            // --- LÍNEA MODIFICADA ---
			_currentAbility.ApplyAbility(_playerObject);
		}
	}


	public void VerifyConditionForAmount() {
		if (_PlayerWallet == null || _ScoreTracker == null) return;
		
		Condition condition = _skins[_currentIndex].unlockCondition;
		if (condition.conditionByAmount.isAmountCoin)
			_unlockButton.enabled = condition.ConditionForAmountGreaterThanTarget(_PlayerWallet.Coins,
				condition.conditionByAmount.targetAmount);
		else
			_unlockButton.enabled = condition.ConditionForAmountGreaterThanTarget(_ScoreTracker.BestScore,
				condition.conditionByAmount.targetAmount);
	}

	public void UnlockSkin() {
		_skins[_currentIndex].unlockCondition.isUnlocked = true;
		if (_skins[_currentIndex].unlockCondition.conditionByAmount.isAmountCoin)
			_PlayerWallet?.SpendCoins(_skins[_currentIndex].unlockCondition.conditionByAmount.targetAmount);

		ChangeUnlockGroupVisibility(false);
		UpdateIsCurrentSkinUnlocked();

		SaveSystem.SaveSkins(GetUnlockedArray());

		ApplyCurrentSkinAbility();
	}

	private void AssignSkinsSaveData(SkinsSaveData data) {
		for (int i = 0; i < _skins.Count; i++)
			_skins[i].unlockCondition.isUnlocked = data.skinsUnlocked[i];
	}

	private bool[] GetUnlockedArray() {
		bool[] unlockedArray = new bool[_skins.Count];
		for (int i = 0; i < _skins.Count; i++)
			unlockedArray[i] = _skins[i].unlockCondition.isUnlocked;
		return unlockedArray;
	}

	private void OnChangeSkin(bool direction) {
		if (_currentAbility != null) {
            // --- LÍNEA MODIFICADA ---
			_currentAbility.RemoveAbility(_playerObject);
			_currentAbility = null;
		}

		WalkOnSkinList(direction);
		HandleUnlockGroup();

		ApplyCurrentSkinAbility();

		if (_playerRenderer != null)
			_playerRenderer.materials = _skins[_currentIndex].skinsMaterials;

		OnEndOfChangeSkin?.Invoke();
	}

	private void ApplyCurrentSkinAbility() {
		if (_skins[_currentIndex].ability != null && _skins[_currentIndex].unlockCondition.isUnlocked) {
			_currentAbility = _skins[_currentIndex].ability;
            // --- LÍNEA MODIFICADA ---
			_currentAbility.ApplyAbility(_playerObject);
		}
	}

	private void WalkOnSkinList(bool direction) {
		if (direction)
			_currentIndex++;
		else
			_currentIndex--;

		if (_currentIndex < 0)
			_currentIndex = _skins.Count - 1;

		if (_currentIndex > _skins.Count - 1)
			_currentIndex = 0;
	}

	private void HandleUnlockGroup() {
		if (_unlockGroup != null)
			ChangeUnlockGroupVisibility(!_skins[_currentIndex].unlockCondition.isUnlocked);

		_skins[_currentIndex].unlockCondition.condition.Invoke();

		UpdateIsCurrentSkinUnlocked();
		UpdateUnlockPhrase();
	}

	private void ChangeUnlockGroupVisibility(bool visibility) => _unlockGroup.SetActive(visibility);

	private void UpdateIsCurrentSkinUnlocked() => isCurrentSkinUnlocked = _skins[_currentIndex].unlockCondition.isUnlocked;

	private void UpdateUnlockPhrase() => _unlockPhraseText.text = _skins[_currentIndex].unlockCondition.phrase;
}