using System;

public static class GameEvents {
	public static event Action OnPlay;
	public static event Action OnPause;
	public static event Action OnResume;
	public static event Action<bool> OnGameOver;
	public static event Action<SaveData> OnAssignSaveData;
	public static event Action<int, float> OnUpdateVolume;
	public static event Action<int> OnUpdateBestScore;
	public static event Action<int> OnUpdateScore;
	public static event Action<int> OnUpdateCoins;
	public static event Action<int> OnUpdateFinalScore;
	public static event Action OnChangedQuality;
	public static event Action OnPrepareContinue;
	public static event Action OnContinue;
	public static event Action OnReplay;

	public static void InvokePlay() => OnPlay?.Invoke();
	public static void InvokePause() => OnPause?.Invoke();
	public static void InvokeResume() => OnResume?.Invoke();
	public static void InvokeGameOver(bool isNewBest) => OnGameOver?.Invoke(isNewBest);
	public static void InvokeAssignSaveData(SaveData data) => OnAssignSaveData?.Invoke(data);
	public static void InvokeUpdateVolume(int track, float volume) => OnUpdateVolume?.Invoke(track, volume);
	public static void InvokeUpdateBestScore(int best) => OnUpdateBestScore?.Invoke(best);
	public static void InvokeUpdateScore(int score) => OnUpdateScore?.Invoke(score);
	public static void InvokeUpdateCoins(int coins) => OnUpdateCoins?.Invoke(coins);
	public static void InvokeUpdateFinalScore(int finalScore) => OnUpdateFinalScore?.Invoke(finalScore);
	public static void InvokeChangedQuality() => OnChangedQuality?.Invoke();
	public static void InvokePrepareContinue() => OnPrepareContinue?.Invoke();
	public static void InvokeContinue() => OnContinue?.Invoke();
	public static void InvokeReplay() => OnReplay?.Invoke();
}