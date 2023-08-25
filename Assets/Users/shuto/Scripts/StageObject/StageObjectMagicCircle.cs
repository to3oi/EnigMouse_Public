using UnityEngine;
using Cysharp.Threading.Tasks;
public class StageObjectMagicCircle : BaseStageObject
{
    public StageObjectMagicCircle(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    public override bool isValidMove()
    {
        //ネズミが移動可能か判定する
        return true;
    }
    public override async UniTask MoveToCell()
    {
        Mouse.Instance.ClearChack();
    }
}