using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class GraphicsSettings : MonoBehaviour {
	public bool IsCurrentlyLowGraphics { get; private set; }

	[Header("Quality Settings")]
	[SerializeField] private RenderPipelineAsset _lowGraphicsPipeline;
	[SerializeField] private RenderPipelineAsset _mediumGraphicsPipeline;
	[SerializeField] private int _targetFrameRate = 60;
	
	[Header("Water References")]
	[SerializeField] private List<GameObject> _waters = new List<GameObject>();

	private GameState _GameState;

	private void Awake() {
		LimitFrameRate();
		_GameState = FindObjectOfType<GameState>();
	}
	
	private void Start()
	{
		// Esta llamada se movió desde Awake porque _GameState puede no estar listo
		SettingsSaveData settingsData = SaveSystem.LoadSettings();
		if (settingsData != null)
		{
			LoadGraphicsSettings(settingsData.isLowGraphics);
		}
	}

	public void LoadGraphicsSettings(bool isLow) {
		IsCurrentlyLowGraphics = isLow;
		AssignQuality();
	}

	public void ChangeQuality() => StartCoroutine(ChangeAndAssignQuality());

	private void AssignQuality() {
		if (IsCurrentlyLowGraphics)
			ChangeQualityToLow();
		else
			ChangeQualityToMedium();

		ChangeWaterReflectionsResolution(IsCurrentlyLowGraphics);
		GameEvents.InvokeChangedQuality();
	}

	private IEnumerator ChangeAndAssignQuality() {
		yield return _GameState.FadeTransition("out");

		IsCurrentlyLowGraphics = !IsCurrentlyLowGraphics;
		AssignQuality();

		yield return _GameState.FadeTransition("in");
	}

	private void ChangeQualityToLow() {
		QualitySettings.SetQualityLevel(0);
		QualitySettings.renderPipeline = _lowGraphicsPipeline;
	}

	private void ChangeQualityToMedium() {
		QualitySettings.SetQualityLevel(2);
		QualitySettings.renderPipeline = _mediumGraphicsPipeline;
	}

	private void LimitFrameRate() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = _targetFrameRate;
	}

	private void ChangeWaterReflectionsResolution(bool isLowGraphics) {
		foreach (GameObject water in _waters) {
			ReflectionProbe probe = water.GetComponent<ReflectionProbe>();
			if(probe == null) continue;
			
			if (isLowGraphics)
				probe.resolution = 16;
			else
				probe.resolution = 64;
			
			UpdateWaterReflectionProbe(probe);
		}
	}

	private void UpdateWaterReflectionProbe(ReflectionProbe probe) {
		if (!probe.isActiveAndEnabled) {
			probe.gameObject.SetActive(true);
			probe.RenderProbe();
			probe.gameObject.SetActive(false);
		}
		else
			probe.RenderProbe();
	}
}