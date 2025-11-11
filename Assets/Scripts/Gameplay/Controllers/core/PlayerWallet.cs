using UnityEngine;

public class PlayerWallet : MonoBehaviour {
	public int Coins { get; private set; }
	private ScoreTracker _ScoreTracker;

	private void Start() {
		_ScoreTracker = FindObjectOfType<ScoreTracker>();
	}

	public void LoadCoins(int loadedCoins) {
		Coins = loadedCoins;
	}

	public void IncreaseCoin() {
		Coins++;
		GameEvents.InvokeUpdateCoins(Coins);
	}

	public void SpendCoins(int amount) {
		Coins -= amount;
		if (Coins < 0) {
			Coins = 0;
		}
		GameEvents.InvokeUpdateCoins(Coins);
		SaveSystem.Save(_ScoreTracker.BestScore, Coins);
	}
}