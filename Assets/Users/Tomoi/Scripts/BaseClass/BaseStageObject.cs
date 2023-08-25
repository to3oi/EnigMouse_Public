using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BaseStageObject : MonoBehaviour
{
    /// <summary>
    /// Unity上のワールド座標ではなくゲームの盤面上の座標
    /// </summary>
    public Vector2 Position;

    [SerializeField] private StageObjectType _stageObjectType = StageObjectType.None;
    public StageObjectType StageObjectType { get => _stageObjectType;}
    
    public int _stageCreateAnimationIndex { get; private set; }

    /// <summary>
    /// アニメーションが再生中かどうか
    /// </summary>
    protected bool isPlaying = false;

    protected Animator _animator;

    public BaseStageObject(Vector2 position,int stageCreateAnimationIndex)
    {
        Position = position;
        _stageCreateAnimationIndex = stageCreateAnimationIndex;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        ObservableStateMachineTrigger[] triggers =
            _animator.GetBehaviours<ObservableStateMachineTrigger>();
        
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
                        isPlaying = false;
                    }
                }).AddTo(this);
        }
    }

    #region Animations

    /// <summary>
    /// ステージ生成時やオブジェクト生成時に再生するアニメーションの関数
    /// </summary>
    public virtual async UniTask InitAnimation()
    {
        //アニメーションの実行開始
        isPlaying = true;
        _animator.SetTrigger(AnimationName.Init);

        await UniTask.WaitUntil(() => !isPlaying);    
    }
    
    
    /// <summary>
    /// オブジェクトの削除時に再生するアニメーションの関数
    /// </summary>
    public virtual async UniTask EndAnimation()
    {
        //アニメーションの実行開始
        isPlaying = true;
        _animator.SetTrigger(AnimationName.End);

        await UniTask.WaitUntil(() => !isPlaying);
    }
    #endregion

    /// <summary>
    /// 引数のMagicTypeを参照してこのStageObjectに与える影響を定義する関数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="direction"></param>
    /// <param name="newBaseStageObject"></param>
    /// <returns>今のStageObjectを変更するかどうか</returns>
    public virtual bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        stageObjectType = StageObjectType.None;
        return false;
    }

    /// <summary>
    /// ネズミが移動可能なマスか判定して返す
    /// </summary>
    /// <returns></returns>
    public virtual bool isValidMove()
    {
        return false;
    }
    /// <summary>
    /// ネズミが今移動したら死亡するか判定して返す
    /// </summary>
    /// <returns></returns>
    public virtual bool isMovedDeath()
    {
        return false;
    }

    /// <summary>
    /// このマスに移動したときの処理を実行
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask MoveToCell()
    {
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void Reset()
    {
        
    }

    /// <summary>
    /// ターンの終了処理
    /// ネズミが動いたあとに必ず呼ばれる
    /// </summary>
    public virtual async UniTask EndTurn()
    {
        await UniTask.Yield();
    }
}