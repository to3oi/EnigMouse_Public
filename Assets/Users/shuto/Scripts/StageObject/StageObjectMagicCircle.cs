using UnityEngine;
using Cysharp.Threading.Tasks;

public class StageObjectMagicCircle : BaseStageObject
{
    [SerializeField] private ParticleSystem magicCircle;

    public StageObjectMagicCircle(Vector2 position, int stageCreateAnimationIndex) : base(position,
        stageCreateAnimationIndex)
    {
    }

    public override async UniTask InitAnimation()
    {
        magicCircle.Play();
        await base.InitAnimation();
    }

    public override bool isValidMove()
    {
        //ネズミが移動可能か判定する
        return true;
    }

    public override async UniTask MoveToCell()
    {
        if (GameManager.Instance.IsLimitCheck())
        {
            Mouse.Instance.ClearCheck();
        }
    }
}