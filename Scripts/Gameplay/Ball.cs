using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour, ISubscriber
{
    [SerializeField] private GameObject _particlePrefab;

    [SerializeField] private float _timeToScoreGoal = 0.5f;

    private Rigidbody2D _rigidBody;
    private TrailRenderer _trailRenderer;
    private GameObject _shadow;

    private bool _canScoreGoal;
    private float _currentGoalTime;

    private void Awake()
    {   
        _rigidBody = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _shadow = transform.GetChild(1).gameObject;

        _shadow.transform.parent = null;
    }
    private void FixedUpdate()
    {
        _shadow.transform.position = new Vector2(transform.position.x, -4.18f);

        if (GameState.Instance.CurrentState != GameState.State.InGame)
            return;

        if (!_canScoreGoal)
        {
            if (_currentGoalTime <= 0)
            {
                _currentGoalTime = _timeToScoreGoal;
                _canScoreGoal = true;
            }
        }
        if (_rigidBody.velocity.y <= 0.2f)
        {
            if (_rigidBody.velocity.x >= 0.1f)
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x * 0.97f, _rigidBody.velocity.y);
        }
    }
    public void SubscribeAll()
    {
        PlayerInput.Instance.PlayerMouseDown += OnPlayerMouseDown;
        GameState.Instance.GameFinished += () => { _rigidBody.velocity = Vector2.zero; };
    }
    public void UnsubscribeAll()
    {
        PlayerInput.Instance.PlayerMouseDown -= OnPlayerMouseDown;
        GameState.Instance.GameFinished -= () => { _rigidBody.velocity = Vector2.zero; };
    }
    private void SpawnParticle()
    {
        var particle = Instantiate(_particlePrefab).GetComponent<ParticleSystem>();

        particle.transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
        particle.Play();

        Destroy(particle.gameObject, 2f);
    }
    private void OnPlayerMouseDown()
    {
        _rigidBody.velocity = Vector2.zero;

        if(PlayerScore.Instance.Score % 2 == 0)
            _rigidBody.AddForce(new Vector2(1, 3) * 50f);
        else
            _rigidBody.AddForce(new Vector2(-1, 3) * 50f);

        AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.BallJump, 1f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Right"))
        {
            var left = GameObject.FindGameObjectWithTag("Left");

            transform.position = new Vector2(left.transform.position.x-1, transform.position.y);
            _trailRenderer.Clear();
        }
        if (collision.CompareTag("Left"))
        {
            var left = GameObject.FindGameObjectWithTag("Right");

            transform.position = new Vector2(left.transform.position.x+1, transform.position.y);
            _trailRenderer.Clear();
        }
        if (collision.CompareTag("Basket"))
        {
            if (_rigidBody.velocity.y < 0)
            {
                PlayerScore.Instance.AddScore();
                SpawnParticle();
                _canScoreGoal = false;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.BallSound, Random.Range(0.9f, 1.1f));
        }
        if (collision.gameObject.CompareTag("BasketNet"))
        {
            collision.transform.DOShakeRotation(0.2f, Mathf.Clamp(10f * (_rigidBody.velocity.y+0.2f), 0, 10)).SetLink(collision.gameObject);
        }
    }
}
