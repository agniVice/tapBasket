using DG.Tweening;
using UnityEngine;

public class StartTipAnim : MonoBehaviour
{
    private void Start()
    {
        Click();
    }
    private void Restart()
    {
        Invoke("Click", 2f);
        transform.DOShakeRotation(0.5f, 10).SetLink(gameObject);
    }
    private void Click()
    {
        transform.DOScale(0.8f, 0.2f).SetEase(Ease.InBack).SetLink(gameObject);
        transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetLink(gameObject).SetDelay(0.2f).OnKill(() => { Restart(); });
    }
}
