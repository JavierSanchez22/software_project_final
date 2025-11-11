using UnityEngine;
using System.Collections.Generic;

public class StageOrchestrator : MonoBehaviour {
	public GameUtilities.Stages CurrentStageEnum { get; private set; } = GameUtilities.Stages.GameNotStarted;
	private GameStage _currentStage;
	private GameState _gameState;
	private ScoreTracker _ScoreTracker;
	
	[Header("Stage Settings")]
	[SerializeField] private int _stage1Threshold = 60;
	[SerializeField] private int _stage2Threshold = 120;
	[SerializeField] private int _remakeStageThreshold = 240;

	[Header("Bridge References")]
	[SerializeField] private GameObject _bridgeCommon;
	[SerializeField] private GameObject _bridgeDamaged;

	[Header("Water References")]
	[SerializeField] private List<GameObject> _waters = new List<GameObject>();
	[SerializeField] private List<Color> _skyboxColors = new List<Color>();
	
	[Tooltip("Amount of speed which the skybox will rotate")]
	[SerializeField] private float _skyboxSpeed = .8f;
	[Tooltip("Duration in seconds to blend from one skybox to another")]
	[SerializeField] private float _skyboxBlendDuration = 2f;

	private GameUtilities.Stages _lastStageEnum = GameUtilities.Stages.GameNotStarted;
	private bool _canUpdateSkybox = false;
	private bool _hasSwapedBridges = false;
	private bool _canSwapBridges = false;
	private float _skyboxLerpFactor = 0f;
	private int _lastStageIndex = 0;

	private void OnEnable() => PlayerCoordinat.OnReachedSwapPoint += SwapBridge;
	private void OnDisable() => PlayerCoordinat.OnReachedSwapPoint -= SwapBridge;

	private void Start() {
		_gameState = FindObjectOfType<GameState>();
		_ScoreTracker = FindObjectOfType<ScoreTracker>();
		
		InitializeReset();
		
		_currentStage = new StageNotStarted();
		if (_currentStage != null) {
			_currentStage.OnEnter(this);
		}
	}

	private void Update() {
		if (_gameState == null || !_gameState.IsGameRunning)
			return;
			
		RenderSettings.skybox.SetFloat("_Rotation", Time.time * _skyboxSpeed);
		
		if (_gameState.IsGamePlayable) {
			if (_canUpdateSkybox)
				UpdateSkybox();
		}
		
		HandleStages();
	}

	private void HandleStages() {
        // --- ESTA ES LA PROTECCIÓN CLAVE ---
		if (_currentStage == null)
			return; // Si el estado es nulo, no hagas nada.

		GameStage nextStage = _currentStage.CheckTransitions(this);
		
		if (nextStage != null && nextStage.GetType() != _currentStage.GetType()) {
			_lastStageEnum = CurrentStageEnum;
			_currentStage = nextStage;
			_currentStage.OnEnter(this);
		}
		
		_currentStage?.OnUpdate(this);
	}

	public void PrepareStageChange(GameUtilities.Stages newStage, bool canSwapBridges) {
		_hasSwapedBridges = false;
		_canSwapBridges = canSwapBridges;
		CurrentStageEnum = newStage;

		_lastStageIndex = (int)_lastStageEnum - 1 <= 0 ? 0 : (int)_lastStageEnum - 1;

		Hitable.currentStage = CurrentStageEnum;
		
		HitableSpawner sm = FindObjectOfType<HitableSpawner>();
		if (sm != null) {
			sm.repeatRate = 3f;
		}

		if (_lastStageEnum != GameUtilities.Stages.GameNotStarted) {
			_canUpdateSkybox = true;
			HandleWater();
		}
	}

	public void SetHitableStage(GameUtilities.Stages stage) => Hitable.currentStage = stage;
	public int GetShadowScore() => _ScoreTracker?.ShadowScore ?? 0;
	public int GetStage1Threshold() => _stage1Threshold;
	public int GetStage2Threshold() => _stage2Threshold;
	public int GetRemakeStageThreshold() => _remakeStageThreshold;

	private void HandleWater() {
		if (_waters == null || _waters.Count == 0) return;

		if (_lastStageIndex == 2)
			_waters[0].SetActive(true);
		else if(_lastStageIndex + 1 < _waters.Count)
			_waters[_lastStageIndex+1].SetActive(true);
	}

	private void InitializeReset() {
		ResetSkybox();
		ResetBridges();
	}

	private void ResetSkybox() {
		if (_skyboxColors != null && _skyboxColors.Count > 0) {
			RenderSettings.skybox.SetColor("_Tint", _skyboxColors[0]);
		}

		if (_waters == null) return;
		
		for (int i = 0; i < _waters.Count; i++) {
			if (_waters[i] == null) continue;
			
			_waters[i].SetActive(i == 0);
			Renderer rend = _waters[i].GetComponent<Renderer>();
			if(rend != null) {
				rend.material.SetFloat("_Alpha", 1);
			}
		}
	}

	private void UpdateSkybox() {
		if (_skyboxColors == null || _skyboxColors.Count == 0 || _waters == null || _waters.Count == 0) return;

		int nextStageIndex = (int)CurrentStageEnum - 1;
		if (nextStageIndex < 0 || nextStageIndex >= _skyboxColors.Count) nextStageIndex = 0;
		
		int repeatWaterIndex = _lastStageIndex == 2 ? 0 : _lastStageIndex + 1;
		if (repeatWaterIndex >= _waters.Count) repeatWaterIndex = 0;
		if (_lastStageIndex >= _waters.Count) _lastStageIndex = 0;


		if (_skyboxLerpFactor < _skyboxBlendDuration) {
			_skyboxLerpFactor += Time.deltaTime;
			RenderSettings.skybox.SetColor("_Tint", Color.Lerp(_skyboxColors[_lastStageIndex], _skyboxColors[nextStageIndex], _skyboxLerpFactor));

			Renderer lastWaterRend = _waters[_lastStageIndex].GetComponent<Renderer>();
			if(lastWaterRend != null) {
				lastWaterRend.material.SetFloat("_Alpha", Mathf.Lerp(1, 0, _skyboxLerpFactor));
			}
			
			Renderer nextWaterRend = _waters[repeatWaterIndex].GetComponent<Renderer>();
			if(nextWaterRend != null) {
				nextWaterRend.material.SetFloat("_Alpha", Mathf.Lerp(0, 1, _skyboxLerpFactor));
			}
		}
		else {
			Renderer lastWaterRend = _waters[_lastStageIndex].GetComponent<Renderer>();
			if (lastWaterRend != null && lastWaterRend.material.GetFloat("_Alpha") <= 0)
				_waters[_lastStageIndex].SetActive(false);

			_canUpdateSkybox = false;
			_skyboxLerpFactor = 0f;
		}
	}

	private void ResetBridges() {
		_hasSwapedBridges = false;
		_canSwapBridges = false;
		
		if (_bridgeCommon != null)
			_bridgeCommon.SetActive(true);
		
		if (_bridgeDamaged != null)
			_bridgeDamaged.SetActive(false);
	}

	private void SwapBridge() {
		if (!_hasSwapedBridges && _canSwapBridges) {
			if (CurrentStageEnum == GameUtilities.Stages.BridgeCollapse) {
				if (_bridgeCommon != null) _bridgeCommon.SetActive(false);
				if (_bridgeDamaged != null) _bridgeDamaged.SetActive(true);
			}
			else if (CurrentStageEnum == GameUtilities.Stages.DynamicCars){
				if (_bridgeCommon != null) _bridgeCommon.SetActive(true);
				if (_bridgeDamaged != null) _bridgeDamaged.SetActive(false);
			}
			_hasSwapedBridges = true;
		}
	}
	
	public void RemakeStages() {
		if(_ScoreTracker != null)
			_ScoreTracker.ResetShadowScore();
			
		_stage1Threshold += 30;
		_stage2Threshold += 30;
		_remakeStageThreshold += 30;
	}
}