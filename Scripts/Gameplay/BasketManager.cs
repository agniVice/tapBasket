using DG.Tweening;
using UnityEngine;

public class BasketManager : MonoBehaviour, ISubscriber
{
    [SerializeField] private Transform _positionLeftTop;
    [SerializeField] private Transform _positionLeftDown;
    [SerializeField] private Transform _positionRightTop;
    [SerializeField] private Transform _positionRightDown;

    [SerializeField] private Collider2D _trigger;

    private Transform _basket;

    private void Awake()
    {
        _basket = transform.GetChild(0);
    }
    public void SubscribeAll()
    {
        GameState.Instance.ScoreAdded += ReplaceBasket;
    }

    public void UnsubscribeAll()
    {
        GameState.Instance.ScoreAdded -= ReplaceBasket;
    }
    private void ReplaceBasket()
    {
        _basket.DOScale(0, 0.1f).SetLink(_basket.gameObject).SetDelay(0.2f);
        _trigger.enabled = false;

        Invoke("Replace", 0.5f);
    }
    private void Replace()
    {
        Vector2 position;
        Vector2 scale;
        if (PlayerScore.Instance.Score % 2 == 0)
        {
            scale = new Vector2(1, 1);
            position = new Vector2(_positionRightDown.position.x, Random.Range(_positionRightDown.position.y, _positionRightTop.position.y));
        }
        else
        {
            scale = new Vector2(-1, 1);
            position = new Vector2(_positionLeftDown.position.x, Random.Range(_positionLeftDown.position.y, _positionLeftTop.position.y));
        }

        _basket.DOScale(scale, 0.1f).SetLink(_basket.gameObject).SetEase(Ease.OutBack).OnKill(() => { _trigger.enabled = true; });
        _basket.rotation = Quaternion.identity;
        _basket.position = position;
    }
}
