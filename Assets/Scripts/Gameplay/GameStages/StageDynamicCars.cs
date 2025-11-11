using System;
using UnityEngine;

public class StageDynamicCars : GameStage {
	public void OnEnter(StageOrchestrator manager) {
		AudioService.Instance.ChangeSoundWithFade(Sound.Type.BGM1, 1);
		manager.PrepareStageChange(GameUtilities.Stages.DynamicCars, true);
	}

	public void OnUpdate(StageOrchestrator manager) {
	}

	public GameStage CheckTransitions(StageOrchestrator manager) {
		int currentShadowScore = manager.GetShadowScore();
		int stage1Threshold = manager.GetStage1Threshold();
		int stage2Threshold = manager.GetStage2Threshold();

		if (currentShadowScore > stage1Threshold && currentShadowScore <= stage2Threshold) {
			return new StageCarCrashAndCrystals();
		}
		return this;
	}
}