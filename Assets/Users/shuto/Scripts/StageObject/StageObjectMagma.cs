using UnityEngine;
using Cysharp.Threading.Tasks;
public class StageObjectMagma : BaseStageObject
{
    public StageObjectMagma(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        //水魔法を使うことで通れなくする
        if (type == MagicType.Water)
        {
            stageObjectType = StageObjectType.Rock;
            SoundManager.Instance.PlaySE(SEType.MagmaToRock);
            return true;
        }

        stageObjectType = StageObjectType.Magma;
        return false;
    }

    public override bool isValidMove()
    {
        //ネズミが移動可能か判定する
        return true;
    }

    public override async UniTask MoveToCell()
    {
        await Mouse.Instance.Death();

    }
    public override bool isMovedDeath()
    {
        return true;
    }
}