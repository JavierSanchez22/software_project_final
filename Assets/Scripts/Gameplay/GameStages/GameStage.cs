using System;

public interface GameStage {
	void OnEnter(StageOrchestrator manager);
	void OnUpdate(StageOrchestrator manager);
	GameStage CheckTransitions(StageOrchestrator manager);
}