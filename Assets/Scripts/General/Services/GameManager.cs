using System;
using UnityEngine;

public class GameUtilities : MonoBehaviour {
	public enum Stages {
		GameNotStarted,
		DynamicCars,
		CarCrashAndCrystals,
		BridgeCollapse,
		EveryStage
	}
	public static void CallRepeating(Action action, ref float timer, float repeatRate) {
		timer -= Time.deltaTime;
		if (timer < 0) {
			timer = repeatRate;
			action();
		}
	}
}