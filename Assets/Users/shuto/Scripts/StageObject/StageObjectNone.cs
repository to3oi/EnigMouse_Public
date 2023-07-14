using UnityEngine;

public class StageObjectNone : BaseStageObject
{
    public float stageObjectKey;
    private void Start()
    {
        //最初、ネズミが通れる
        this.gameObject.SetActive(true);
    }

    public StageObjectNone(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    public override bool isValidMove()
    {
        //ネズミが移動可能か判定する
        return true;
    }
}