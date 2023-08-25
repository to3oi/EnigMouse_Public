using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StageObjectTestAnimation : BaseStageObject
{
    private Animator _testAnimator;
    public StageObjectTestAnimation(Vector2 position, int stageCreateAnimationIndex) : base(position,
        stageCreateAnimationIndex)
    {
    }

    /// <summary>
    /// アニメーションが再生中かどうか
    /// </summary>
    private bool testIsPlaying = false;

    //Animatorの取得など
    private void Awake()
    {
        _testAnimator = GetComponent<Animator>();

        ObservableStateMachineTrigger[] triggers =
            _testAnimator.GetBehaviours<ObservableStateMachineTrigger>();


        foreach (var trigger in triggers)
        {
            // Stateの終了イベント
            IDisposable exitState = trigger
                .OnStateExitAsObservable()
                .Subscribe(onStateInfo =>
                {
                    AnimatorStateInfo info = onStateInfo.StateInfo;
                    if (info.IsName("Base Layer.Init") || info.IsName("Base Layer.End"))
                    {
                        testIsPlaying = false;
                    }
                }).AddTo(this);
        }
    }


    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        stageObjectType = StageObjectType.None;
        return false;
    }

    /// <summary>
    /// ネズミが移動可能なマスか判定して返す
    /// </summary>
    /// <returns></returns>
    public override bool isValidMove()
    {
        return true;
    }

    public Texture2D Texture2D;


    public Texture2D GetTexture2D()
    {
        return Texture2D;
    }

    /// <summary>
    /// ステージ生成時やオブジェクト生成時に再生するアニメーションの関数
    /// </summary>
    public override async UniTask InitAnimation()
    {
        //アニメーションの実行開始
        testIsPlaying = true;
        _testAnimator.SetTrigger(AnimationName.Init);

        await UniTask.WaitUntil(() => !testIsPlaying);
    }


    /// <summary>
    /// オブジェクトの削除時に再生するアニメーションの関数
    /// </summary>
    public override async UniTask EndAnimation()
    {
        //アニメーションの実行開始
        testIsPlaying = true;
        _testAnimator.SetTrigger(AnimationName.End);

        await UniTask.WaitUntil(() => !testIsPlaying);
    }
}