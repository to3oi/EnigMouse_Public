using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class StageObjectResetBottle : BaseStageObject
{
    DynamicStageObject dynamicStageObject;
    [SerializeField] private GameObject bottleGameObject;
    private Tween bottleRotate;

    public StageObjectResetBottle(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    private void Start()
    {
        //ResetBottleの回る処理
        bottleRotate = bottleGameObject.transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    public override bool isValidMove()
    {
        //ネズミが移動可能か判定する
        return true;
    }

    public override async UniTask MoveToCell()
    {
        await Mouse.Instance.ResetBottle();
        dynamicStageObject = StageManager.Instance.GetDynamicStageObject((int)Position.x, (int)Position.y);
        dynamicStageObject.ReplaceBaseStageObject(StageObjectType.None);
    }

    private void OnDestroy()
    {
        //ResetBottleの回る処理を消す
        bottleRotate.Kill();
    }
}
