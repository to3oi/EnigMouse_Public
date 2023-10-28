using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class StageObjectMouse : BaseStageObject
{
    [SerializeField] private GameObject Object;
    [SerializeField] private GameObject mouse;

    public StageObjectMouse(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    public override async UniTask InitAnimation()
    {
        if (Mouse.Instance != null)
        {
            Mouse.Instance.Reset(Position);
        }
        else
        {
            var m = Instantiate(mouse);
            //初回のみネズミの位置をマスクする処理を実行
            GameManager.Instance.SetMouseMask(transform.position + Vector3.up);
        }
        
        Mouse.Instance.transform.position = new Vector3(transform.position.x,8,transform.position.z);
        
        //アニメーションの実行開始
        isPlaying = true;
        _animator.SetTrigger(AnimationName.Init);
        List<UniTask> task = new List<UniTask>();
        task.Add(Mouse.Instance.transform.DOMoveY(transform.position.y + 0.5f, 2).SetEase(Ease.OutBounce).ToUniTask());
        task.Add(UniTask.WaitUntil(() => !isPlaying));
        
        await UniTask.WhenAll(task);

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
        return true;
    }
}