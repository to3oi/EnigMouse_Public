using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class StageObjectKey : BaseStageObject
{
    DynamicStageObject dynamicStageObject;
    [SerializeField] private GameObject keyGameObject;
    public StageObjectKey(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    private void Start()
    {
        keyGameObject.transform.DOLocalRotate(new Vector3(45, 360, 0), 5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    public override bool isValidMove()
    {
        //ネズミが移動可能か判定する
        return true;
    }

    public override async UniTask MoveToCell()
    {
        await Mouse.Instance.KeyGet();
        dynamicStageObject = StageManager.Instance.GetDynamicStageObject((int)Position.x,(int)Position.y);
        dynamicStageObject.ReplaceBaseStageObject(StageObjectType.None);
    }
}
