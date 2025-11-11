using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour {
	public bool IsGameRunning { get; private set; }
	public bool IsGamePaused { get; private set; }
	public bool IsGamePlayable { get { return (IsGameRunning && !IsGamePaused); } }
    
    public bool HasExtraLife { get; private set; } = false;

	[Header("Transitions")]
	[SerializeField] private Animator _endTransitionAnimator;
	[SerializeField] private Animator _pauseAnimator;
	[SerializeField] private Animator _gameOverAnimator;
	[SerializeField] private float _endTransitionDuration = 1f;
	[SerializeField] private float _scaleTransitionDuration = .5f;

	[Header("Game Over Logic")]
	public bool isFirstLose = true;
	public float continueChance = .2f;

	private ScoreTracker _ScoreTracker;

	private void Start() {
		_ScoreTracker = FindObjectOfType<ScoreTracker>();
		UnfreezeTime();
	}
    
    // --- ¡MÉTODO NUEVO! ---
    // Este es el "botón de reinicio" que el Bootstrapper usará
	public void ResetState() {
		IsGameRunning = false;
		IsGamePaused = false;
		_ScoreTracker?.ResetScore();
		UnfreezeTime();
	}

	public void SetExtraLife(bool hasLife) {
		HasExtraLife = hasLife;
	}
    
    public void Play() {
		AudioService.Instance.PlaySound(Sound.Type.BGM1, 1);
		IsGameRunning = true;
        IsGamePaused = false;
        _ScoreTracker?.ResetScore();
		GameEvents.InvokePlay();
		UnfreezeTime();
	}

	public void Replay() {
		AudioService.Instance.ResumeTrack(1);
		IsGamePaused = false;
		GameEvents.InvokeReplay();
	}

	public void Pause() {
		AudioService.Instance.PauseAllTracks();
		IsGamePaused = true;
		StartCoroutine(TransitionedPause());
	}

	public void Resume() {
		AudioService.Instance.ResumeAllTracks();
		IsGamePaused = false;
		StartCoroutine(TransitionedResume());
	}

	public void GameOver(Sound.Type gameOverSound = Sound.Type.None) {
		AudioService.Instance.PlaySoundOneShot(gameOverSound, 2);
		if (!IsGamePlayable)
			return;

		IsGameRunning = false;
		IsGamePaused = true;

		bool isNewBest = _ScoreTracker.CheckForNewBestScore();
		GameEvents.InvokeGameOver(isNewBest);
		GameEvents.InvokeUpdateFinalScore(_ScoreTracker.Score);

		SaveSystem.Save(_ScoreTracker.BestScore, FindObjectOfType<PlayerWallet>().Coins);
	}

	public void PlayAgain() {
        // Ahora solo llamamos al reinicio y cargamos la escena
        ResetState();
		StartCoroutine(ReloadSceneAfterTransition());
	}

	public void PrepareContinue() => StartCoroutine(PrepareContinueAfterFade());

	public void Continue() => StartCoroutine(ContinueAfterFade());

	private IEnumerator PrepareContinueAfterFade() {
		yield return FadeTransition("out");
		GameEvents.InvokePrepareContinue();
	}

	private IEnumerator ContinueAfterFade() {
		AudioService.Instance.ResumeTrack(2);
		IsGameRunning = true;
        IsGamePaused = false;
		yield return FadeTransition("in");
		yield return TransitionedContinue();
	}

	private IEnumerator ReloadSceneAfterTransition() {
		_endTransitionAnimator.SetTrigger("FadeOut");
		yield return new WaitForSecondsRealtime(_endTransitionDuration);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public IEnumerator FadeTransition(string fadeType) {
		if (fadeType == "in") {
			yield return null;
			_endTransitionAnimator.SetTrigger("FadeIn");
		}
		else if (fadeType == "out")
			_endTransitionAnimator.SetTrigger("FadeOut");
		
		yield return new WaitForSecondsRealtime(_endTransitionDuration);
	}

	private IEnumerator TransitionedPause() {
		GameEvents.InvokePause();
		yield return WaitScaleUpTransition(_pauseAnimator);
		FreezeTime();
	}

	private IEnumerator TransitionedResume() {
		UnfreezeTime();
		yield return WaitScaleDownTransition(_pauseAnimator);
		GameEvents.InvokeResume();
	}

	private IEnumerator TransitionedContinue() {
		yield return WaitScaleDownTransition(_gameOverAnimator);
		GameEvents.InvokeContinue();
	}

	private IEnumerator WaitScaleUpTransition(Animator animator) {
		animator.SetTrigger("ScaleUpBouncy");
		yield return new WaitForSeconds(_scaleTransitionDuration);
	}

	private IEnumerator WaitScaleDownTransition(Animator animator) {
		animator.SetTrigger("ScaleDown");
		yield return new WaitForSeconds(_scaleTransitionDuration);
	}

	private void FreezeTime() => Time.timeScale = 0f;
	private void UnfreezeTime() => Time.timeScale = 1f;
}