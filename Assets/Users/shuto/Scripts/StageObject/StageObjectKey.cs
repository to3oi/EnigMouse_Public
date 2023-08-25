using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

public class StageObjectKey : BaseStageObject
{
    GameObject Object;
    StageObjectNone stageObjectNone;

    private void Start()
    {
        //最初、ネズミが通れる
        this.gameObject.SetActive(true);

        //StageObjectNoneを取得
        //stageObjectNone = Object.GetComponent<StageObjectNone>();
    }

    public StageObjectKey(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
        //最初、ネズミが通れる
        this.gameObject.SetActive(true);

        //StageObjectNoneを取得
        //stageObjectNone = Object.GetComponent<StageObjectNone>();   
    }

    public void KeyGet()
    {
            this.gameObject.SetActive(false);
            //StageObjectNoneを取得
            //stageObjectNone.stageObjectKey = 15f;
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
    public override async UniTask MoveToCell()
    {
        await Mouse.Instance.KeyGet();
        
    }
}
