using System;

public class StageNotStarted : GameStage {
	public void OnEnter(StageOrchestrator manager) {
		manager.SetHitableStage(GameUtilities.Stages.GameNotStarted);
	}

	public void OnUpdate(StageOrchestrator manager) {
	}

	public GameStage CheckTransitions(StageOrchestrator manager) {
		int currentShadowScore = manager.GetShadowScore();
		int stage1Threshold = manager.GetStage1Threshold();

		if (currentShadowScore > 0 && currentShadowScore <= stage1Threshold) {
			return new StageDynamicCars();
		}
		return this;
	}
}