using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class StageObjectMouse : BaseStageObject
{
    [SerializeField] private GameObject Object;
    [SerializeField] private GameObject mouse;

    public StageObjectMouse(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    public override async UniTask InitAnimation()
    {
        if (Mouse.Instance != null)
        {
            Mouse.Instance.Reset(Position);

            Mouse.Instance.transform.position += Vector3.up * -1;
            Mouse.Instance.transform.SetParent(Object.transform, true);
        }
        else
        {
            var m = Instantiate(mouse, Object.transform);
            m.transform.localScale = Vector3.zero;
            m.transform.DOScale(Vector3.one, 1);
        }

        //アニメーションの実行開始
        isPlaying = true;
        _animator.SetTrigger(AnimationName.Init);
        SoundManager.Instance.PlaySE(SEType.SE3);
        await UniTask.WaitUntil(() => !isPlaying);
        _animator.enabled = false;
        Mouse.Instance.transform.SetParent(null, true);
        _animator.enabled = true;
    }

    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        if (type == MagicType.Ice)
        {
            stageObjectType = StageObjectType.Ice;
            return true;
        }

        stageObjectType = StageObjectType.None;
        return false;
    }

    public override bool isValidMove()
    {
        return true;
    }
}