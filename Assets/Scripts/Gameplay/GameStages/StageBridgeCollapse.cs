using System;
using UnityEngine;

public class StageBridgeCollapse : GameStage {
	public void OnEnter(StageOrchestrator manager) {
		AudioService.Instance.ChangeSoundWithFade(Sound.Type.BGM3, 1);
		manager.PrepareStageChange(GameUtilities.Stages.BridgeCollapse, true);
	}

	public void OnUpdate(StageOrchestrator manager) {
	}

	public GameStage CheckTransitions(StageOrchestrator manager) {
		int currentShadowScore = manager.GetShadowScore();
		int remakeThreshold = manager.GetRemakeStageThreshold();

		if (currentShadowScore > remakeThreshold) {
			manager.RemakeStages();
			return new StageDynamicCars();
		}
		return this;
	}
}