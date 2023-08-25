using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageObjectMouse : BaseStageObject
{
    public StageObjectMouse(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }
    public override async UniTask InitAnimation()
    {
        //アニメーションの実行開始
        isPlaying = true;
        _animator.SetTrigger(AnimationName.Init);

        await UniTask.WaitUntil(() => !isPlaying);
        _animator.enabled = false;
        Mouse.Instance.transform.SetParent(null,true);
    }

    public override bool isValidMove()
    {
        return true;
    }
}
