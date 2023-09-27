using UnityEngine;

public class TestFadeMask : MonoBehaviour
{
    [SerializeField] private FadeMask _fadeMask;
    [SerializeField] private GameObject _target;
    [SerializeField] private float duration;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTransform;

    private void OnGUI()
    {
        if (GUILayout.Button("FadeStart"))
        {
            _fadeMask.FadeStart(RectTransformUtility.WorldToScreenPoint(Camera.main, _target.transform.position),
                duration);
        }

        if (GUILayout.Button("Reset"))
        {
            _canvasGroup.alpha = 0;
            _rectTransform.localScale = Vector3.zero;
        }
    }
}