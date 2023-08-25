using UnityEngine;

public class StageObjectWood : BaseStageObject
{
    public StageObjectWood(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    public override bool isValidMove()
    {
        //ネズミが移動可能か判定する
        return false;
    }
}