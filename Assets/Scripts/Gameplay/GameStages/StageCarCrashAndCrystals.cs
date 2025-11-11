using System;
using UnityEngine;

public class StageCarCrashAndCrystals : GameStage {
	public void OnEnter(StageOrchestrator manager) {
		AudioService.Instance.ChangeSoundWithFade(Sound.Type.BGM2, 1);
		manager.PrepareStageChange(GameUtilities.Stages.CarCrashAndCrystals, false);
	}

	public void OnUpdate(StageOrchestrator manager) {
	}

	public GameStage CheckTransitions(StageOrchestrator manager) {
		int currentShadowScore = manager.GetShadowScore();
		int stage2Threshold = manager.GetStage2Threshold();
		int remakeThreshold = manager.GetRemakeStageThreshold();

		if (currentShadowScore > stage2Threshold && currentShadowScore <= remakeThreshold) {
			return new StageBridgeCollapse();
		}
		return this;
	}
}