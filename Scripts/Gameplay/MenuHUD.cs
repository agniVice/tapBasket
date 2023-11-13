using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class MenuHUD : MonoBehaviour, IInitializable, ISubscriber
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private Image _audioImage;
    [SerializeField] private Image _secondAudioImage;
    [SerializeField] private TextMeshProUGUI _languageText;

    private bool _isInitialized;

    private void Start()
    {
        _secondAudioImage = FindObjectOfType<HUD>().audioImage;

        UpdateAudio();
        UpdateLanguage();
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
    public void Initialize()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        _isInitialized = true;

        UpdateScore();

        Show();
    }
    public void SubscribeAll()
    {
        GameState.Instance.GameStarted += Hide;
    }
    public void UnsubscribeAll()
    {
        GameState.Instance.GameStarted -= Hide;
    }
    private void UpdateScore()
    {
        _scoreText.transform.DOScale(1.5f, 0.3f).SetLink(_scoreText.gameObject).SetUpdate(true);
        _scoreText.transform.DOScale(1, 0.3f).SetLink(_scoreText.gameObject).SetDelay(0.3f).SetUpdate(true);

        _scoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
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
    private void UpdateLanguage()
    {
        if (LanguageManager.Instance.CurrentLanguage == Languange.Russian)
            _languageText.text = "РУС";
        else
            _languageText.text = "ENG";
    }
    public void UpdateAudio()
    {
        if (AudioVibrationManager.Instance.IsSoundEnabled)
        {
            _audioImage.color = new Color32(255, 255, 255, 255);
            _secondAudioImage.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            _audioImage.color = new Color32(255, 255, 255, 180);
            _secondAudioImage.color = new Color32(255, 255, 255, 180);
        }
    }
    public void OnAudioToggled()
    {
        AudioVibrationManager.Instance.ToggleMusic();
        AudioVibrationManager.Instance.ToggleSound();

        UpdateAudio();
    }
    public void OnLanguageToggled()
    {
        LanguageManager.Instance.ToggleLanguange();

        UpdateLanguage();
    }
    public void OnPrivacyPolicyOpened()
    {
        Application.OpenURL("https://doc-hosting.flycricket.io/tap-basket-privacy-policy/4bd07665-c0be-489a-ae39-8bd0bf8eeeda/privacy");
    }
}