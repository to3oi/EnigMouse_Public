using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(RectTransform))]
public class FadeMask : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private RawImage _rawImage;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rawImage = GetComponent<RawImage>();
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.localScale = Vector3.zero;
    }

    /// <summary>
    /// フェードを開始
    /// </summary>
    /// <param name="rectPosition"></param>
    /// <param name="duration"></param>
    public Sequence FadeStart(Vector2 rectPosition, float duration)
    {
        var sequence = DOTween.Sequence();
        _rectTransform.position = rectPosition;

        sequence.Append(_rectTransform.DOScale(50, duration));
        sequence.Join(DOVirtual.Float(
            0f,
            1.0f,
            duration,
            (tweenValue) => { _canvasGroup.alpha = tweenValue; }
        ));

        //sequence.Play();
        return sequence;
    }
    public void Reset()
    {
        _rectTransform.localScale = Vector3.zero;
        _canvasGroup.alpha = 0;
    }
}