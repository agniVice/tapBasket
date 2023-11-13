using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerScore : MonoBehaviour, ISubscriber, IInitializable
{
    public static PlayerScore Instance;

    public int Score { get; private set; }

    private bool _isInitialized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void OnEnable()
    {
        if (!_isInitialized)
            return;

        SubscribeAll();
    }
    private void OnDisable()
    {
        UnsubscribeAll();
    }
    public void SubscribeAll()
    {
        GameState.Instance.GameFinished += Save;
    }

    public void UnsubscribeAll()
    {
        GameState.Instance.GameFinished += Save;
    }

    public void Initialize()
    {
        _isInitialized = true;
    }
    public void AddScore()
    {
        if(GameState.Instance.CurrentState == GameState.State.InGame)
        {
            Camera.main.DOShakePosition(0.4f, 0.2f, fadeOut: true).SetUpdate(true);
            Camera.main.DOShakeRotation(0.4f, 0.2f, fadeOut: true).SetUpdate(true);

            GameTimer.Instance.ResetTimer();
            GameTimer.Instance.StartTimer();

            Score += 1;
            AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.ScoreAdd, 1f);
            GameState.Instance.ScoreAdded?.Invoke();
            Save();
        }
    }
    private void Save()
    {
        if(PlayerPrefs.GetInt("HighScore", 0) < Score)
            PlayerPrefs.SetInt("HighScore", Score);
    }
}