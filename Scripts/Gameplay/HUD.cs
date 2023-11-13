using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class HUD : MonoBehaviour, IInitializable, ISubscriber
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _hudScore;
    [SerializeField] private Image _timeBar;

    public Image audioImage;

    private bool _isInitialized;

    private void Awake()
    {
        audioImage.GetComponent<Button>().onClick.AddListener(() => {
            AudioVibrationManager.Instance.ToggleMusic();
            AudioVibrationManager.Instance.ToggleSound();
            FindObjectOfType<MenuHUD>().UpdateAudio();
        });
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
    private void FixedUpdate()
    {
        _timeBar.fillAmount = GameTimer.Instance.Timer / GameTimer.Instance.DefaultTimer;
    }
    public void Initialize()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        _isInitialized = true;

        _scoreText.text = "0";

        UpdateScore();
    }
    public void SubscribeAll()
    {
        GameState.Instance.GameStarted += Show;
        GameState.Instance.GameFinished += Hide;
        GameState.Instance.GameUnpaused += Show;

        GameState.Instance.ScoreAdded += UpdateScore;
    }
    public void UnsubscribeAll()
    {
        GameState.Instance.GameStarted -= Show;
        GameState.Instance.GameFinished -= Hide;
        GameState.Instance.GameUnpaused -= Show;

        GameState.Instance.ScoreAdded -= UpdateScore;
    }
    private void UpdateScore()
    {
        _scoreText.transform.DOScale(1.5f, 0.3f).SetLink(_scoreText.gameObject).SetUpdate(true);
        _scoreText.transform.DOScale(1, 0.3f).SetLink(_scoreText.gameObject).SetDelay(0.3f).SetUpdate(true);

        _hudScore.text = "+1";

        _hudScore.color = new Color32(255, 255, 255, 255);
        _hudScore.DOFade(0, 0.5f).SetLink(_hudScore.gameObject);

        RectTransform CanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(FindObjectOfType<Ball>().transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
        _hudScore.rectTransform.anchoredPosition = WorldObject_ScreenPosition;

        _scoreText.text = PlayerScore.Instance.Score.ToString();
    }
    private void Show()
    {
        _panel.SetActive(true);
    }
    private void Hide()
    {
        _panel.SetActive(false);
    }
    public void OnRestartButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Gameplay");
    }
    public void OnPauseButtonClicked()
    {
        GameState.Instance.PauseGame();
    }
}