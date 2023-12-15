using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class EXUnLockPerformance : MonoBehaviour
{
    private Animator _animator;
    private static readonly int ExStart = Animator.StringToHash("EXStart");
    
    
    /// <summary>
    /// エクストラ演出を表示するRawImageの参照
    /// </summary>
    [SerializeField] private RawImage _exViewer;

    void Start()
    {
        _animator = GetComponent<Animator>();
        SetEXViewAlpha(0);
    }

    [ContextMenu("StartEXUnLockPerformance")]
    public async UniTask StartEXUnLockPerformance()
    {
        await DOVirtual.Float(0, 1, 1f, SetEXViewAlpha);
        _animator.SetTrigger(ExStart);
    }

    /// <summary>
    /// 鍵をアンロックする演出が終了間近になったら呼び出す関数
    /// </summary>
    public void CompletEXUnLockPerformance ()
    {
        GameManager.Instance.CompletEXUnLockPerformance();
        DOVirtual.Float(1, 0, 1f, SetEXViewAlpha);
    }

    private void SetEXViewAlpha(float alpha)
    {
        var c = _exViewer.color;
        c.a = alpha;
        _exViewer.color = c;
    }

    /// <summary>
    /// 錠前が解除されるSEを再生
    /// </summary>
    private void UnlockSE()
    {
        SoundManager.Instance.PlaySE(SEType.SE38);
    }
    
    /// <summary>
    /// 鎖が破壊されるSEを再生
    /// </summary>
    private void ChainBreakingSE()
    {
        SoundManager.Instance.PlaySE(SEType.SE39);
    }
}
