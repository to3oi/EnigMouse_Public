using UnityEngine;

public class StageObjectWood : BaseStageObject
{
    public StageObjectWood(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        if (type == MagicType.Fire)
        {
            stageObjectType = StageObjectType.Flame;
            SoundManager.Instance.PlaySE(SEType.SE21);
            return true;
        }
        stageObjectType = StageObjectType.Wood;
        return false;
    }
    public override bool isValidMove()
    {
        //ネズミが移動可能か判定する
        return false;
    }
}