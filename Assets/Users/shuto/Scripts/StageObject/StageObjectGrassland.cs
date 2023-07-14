using UnityEngine;

public class StageObjectGrassland : BaseStageObject
{
    GameObject Object;
    StageObjectWood stageObjectWood;

    private void Start()
    {
        //最初、ネズミが通れる
        this.gameObject.SetActive(true);

        //StageObjectWoodを取得
        Object = StageObjectList.Instance.GetGameObject(StageObjectType.Wood);

        stageObjectWood = Object.GetComponent<StageObjectWood>();
    }

    public StageObjectGrassland(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
        //最初、ネズミが通れる
        this.gameObject.SetActive(true);
        
        //StageObjectWoodを取得
        Object = StageObjectList.Instance.GetGameObject(StageObjectType.Wood);

        stageObjectWood = Object.GetComponent<StageObjectWood>();
    }

    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        //水魔法を使うことで通れなくする
        if (type == MagicType.Water)
        {
            this.gameObject.SetActive(false);
            //StageObjectWoodを呼び出す
            stageObjectWood.stageObjectGrassland = 15f;
            stageObjectType = StageObjectType.Wood;
            return true;
        }

        stageObjectType = StageObjectType.Grassland;
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