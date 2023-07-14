using UnityEngine;

public class StageObjectMagma : BaseStageObject
{
    GameObject Object;
    StageObjectRock stageObjectRock;

    private void Start()
    {
        //最初、ネズミが通れる
        this.gameObject.SetActive(true);

        //StageObjectRockを取得
        Object = StageObjectList.Instance.GetGameObject(StageObjectType.Rock);

        stageObjectRock = Object.GetComponent<StageObjectRock>();
    }

    public StageObjectMagma(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
        //最初、ネズミが通れる
        this.gameObject.SetActive(true);

        //StageObjectRockを取得
        Object = StageObjectList.Instance.GetGameObject(StageObjectType.Rock);

        stageObjectRock = Object.GetComponent<StageObjectRock>();
    }

    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        //水魔法を使うことで通れなくする
        if (type == MagicType.Water)
        {
            this.gameObject.SetActive(false);
            //StageObjectRockを呼び出す
            stageObjectRock.stageObjectMagma = 15f;
            
            stageObjectType = StageObjectType.Rock;
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

    public override void Reset()
    {
        //初期化する
        this.gameObject.SetActive(true);
    }
}