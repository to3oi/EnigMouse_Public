using UnityEngine;

public class StageObjectNone : BaseStageObject
{
    public StageObjectNone(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
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
        //ネズミが移動可能か判定する
        return true;
    }
}