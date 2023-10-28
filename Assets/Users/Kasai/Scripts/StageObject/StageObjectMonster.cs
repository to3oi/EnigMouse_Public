using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using UniRx;
using System.Threading;

public class StageObjectMonster : BaseStageObject
{
    #region アニメーション

    private Animator _monsterAnimator;

    private static readonly int IsIdleMotionChange = Animator.StringToHash("isIdleMotionChange");
    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private static readonly int IsJump = Animator.StringToHash("isJump");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int DieCancel = Animator.StringToHash("DieCancel");
    private static readonly int AttackType = Animator.StringToHash("AttackType");
    private static readonly int DieType = Animator.StringToHash("DieType");

    #endregion

    private bool isDie = false;
    [SerializeField] private GameObject _monsterRoot;
    [SerializeField] private Transform _offsetObject;
    [SerializeField] private Transform RotationTarget;

    private CancellationTokenSource monsterAnimationCts;
    private CancellationToken monsterAnimationCt;
    private CancellationToken MonsterAnimationCancel()
    {
        //キャンセル
        monsterAnimationCts?.Cancel();
        //新しく作成
        monsterAnimationCts = new CancellationTokenSource();
        monsterAnimationCt = monsterAnimationCts.Token;
        return monsterAnimationCt;
    }
    private void Start()
    {
        _monsterAnimator = _monsterRoot.GetComponent<Animator>();
        _offsetObject = _monsterRoot.transform.parent;
        _offsetObject.transform.localEulerAngles = new Vector3(0, Random.Range(0,360), 0);
        MonsterAnimationCancel();
        ChangeAnimationLoop(MonsterAnimationCancel()).Forget();
    }
    /// <summary>
    /// isIdleMotionChangeをランダムな時間でSetTriggerする
    /// </summary>
    private async UniTask ChangeAnimationLoop(CancellationToken token)
    {
        while (true)
        {
            //ランダムな時間待機
            token.ThrowIfCancellationRequested();
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(15, 35)),cancellationToken:token);
            await _offsetObject.DORotate(new Vector3(0, Random.Range(0, 360), 0), 1);
            _monsterAnimator.SetTrigger(IsIdleMotionChange);

            //50%で連続再生
            //Triggerを使用してAnimationを実行するのに時間がかかるので適当な時間待機
            token.ThrowIfCancellationRequested();
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
            if (Random.Range(0, 2) == 0)
            {
                await _offsetObject.DORotate(new Vector3(0, Random.Range(0, 360), 0), 1);
                _monsterAnimator.SetTrigger(IsIdleMotionChange);
                
            }
        }
    }

    public override async UniTask InitAnimation()
    {
        Reset();
        await base.InitAnimation();
    }

    public StageObjectMonster(Vector2 position, int stageCreateAnimationIndex) : base(position,
        stageCreateAnimationIndex)
    {
    }

    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        //モンスターが火の魔法を受けたとき
        //モンスターが死んでいないとき
        if (MagicType.Fire == type && !isDie)
        {
            //死亡するアニメーションを再生する
            _monsterAnimator.SetTrigger(Die);
            _monsterAnimator.SetFloat(DieType, (float)Random.Range(0, 2));

            SoundManager.Instance.PlaySE(SEType.SE22);
            isDie = true;
        }
        if(MagicType.Ice == type && isDie)
        {
            stageObjectType = StageObjectType.Ice;
            return true;
        }

        stageObjectType = StageObjectType.Monster;
        return false;
    }

    /// <summary>
    /// このマスに移動したときの処理を実行
    /// </summary>
    /// <returns></returns>
    public override async UniTask MoveToCell()
    {
        //自分が死んでいる場合処理を終了
        if (isDie)
        {
            return;
        }

        RotationTarget = Mouse.Instance.transform;
        //ネズミを攻撃する
        _monsterAnimator.SetTrigger(IsJump);
        _monsterAnimator.SetFloat(AttackType, Random.Range(0, 2));
        //モンスターとネズミの方向の差のQuaternion
        var aim = RotationTarget.position - _offsetObject.position;
        aim.y = 0;
        var q = Quaternion.LookRotation(aim, Vector3.up);
        //少し待機してジャンプのアニメーションで空中にいるときに回転を開始する
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        //回転の終了を待つ
        await _offsetObject.DORotateQuaternion(q, 0.1f).AsyncWaitForCompletion();
        _monsterAnimator.SetTrigger(IsAttack);
        //攻撃モーションごとに遅延の時間を変えたい
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        SoundManager.Instance.PlaySE(SEType.SE18);
        await Mouse.Instance.Death();
        //アタックアニメーションの時間待機
        //アニメーションの終了時間でawaitできるけど使用箇所ここだけになるので未使用
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void Reset()
    {
        if (isDie)
        {
            _monsterAnimator.SetTrigger(DieCancel);
        }
        else
        {
            _monsterAnimator.ResetTrigger(DieCancel);
        }

        isDie = false;
        _monsterAnimator.ResetTrigger(IsIdleMotionChange);
        _monsterAnimator.ResetTrigger(IsAttack);
        _monsterAnimator.ResetTrigger(IsJump);
        _monsterAnimator.ResetTrigger(Die);
        _offsetObject.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
    }

    public override bool isValidMove()
    {
        return true;
    }

    public override bool isMovedDeath()
    {
        return !isDie;
    }
    private void OnDestroy()
    {
        MonsterAnimationCancel();
    }
}