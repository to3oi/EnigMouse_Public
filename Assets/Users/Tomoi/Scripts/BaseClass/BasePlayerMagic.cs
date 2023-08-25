using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BasePlayerMagic : MonoBehaviour
{
    private CancellationTokenSource particleCts;
    protected CancellationToken particleCt;

    private CancellationTokenSource moveCts;
    protected CancellationToken moveCt;

    [SerializeField] private ParticleSystem initCharge;
    [SerializeField] private MagicType _magicType;
    private EffectType eff_initCircle;
    private EffectType eff_defaultCircle;
    private EffectType eff_igniteParticle;
    private EffectType eff_releaseCircle;

    protected Transform particlePosition;

    /// <summary>
    /// チャージ時間
    /// InputManagerの_staticDurationと合わせる
    /// </summary>
    private float storageTime = 2f;

    /// <summary>
    /// 魔法を発火するかどうか
    /// </summary>
    private bool isIgnite = false;

    protected void Awake()
    {
        // CancellationTokenSourceを生成
        particleCts = new CancellationTokenSource();
        particleCt = particleCts.Token;
        moveCts = new CancellationTokenSource();
        moveCt = moveCts.Token;

        particlePosition = new GameObject("particlePosition").transform;

        //_magicTypeからエフェクトを一括設定
        eff_initCircle = (EffectType)((int)_magicType * 10 + 1);
        eff_defaultCircle = (EffectType)((int)_magicType * 10 + 3);
        eff_igniteParticle = (EffectType)((int)_magicType * 10 + 4);
        eff_releaseCircle = (EffectType)((int)_magicType * 10 + 5);
    }

    protected CancellationToken MoveCancel()
    {
        //キャンセル
        moveCts.Cancel();
        //新しく作成
        moveCts = new CancellationTokenSource();
        moveCt = moveCts.Token;
        return moveCt;
    }

    protected CancellationToken ParticleCancel()
    {
        //キャンセル
        particleCts.Cancel();
        //新しく作成
        particleCts = new CancellationTokenSource();
        particleCt = particleCts.Token;
        return particleCt;
    }

    void OnDestroy()
    {
        //GameObject破棄時にキャンセル実行
        moveCts?.Cancel();
        particleCts?.Cancel();
    }
    
    /// <summary>
    /// 魔法陣の展開
    /// </summary>
    public virtual void Init(Vector3 newPosition)
    {
        isIgnite = false;
        newPosition.y += 0.1f;
        particlePosition.position = newPosition;
        StartParticle(ParticleCancel()).Forget();
    }

    /// <summary>
    /// 魔法陣の解除
    /// </summary>
    public virtual void Release()
    {
        ParticleCancel();
    }

    /// <summary>
    /// パーティクルの生成を開始
    /// </summary>
    /// <param name="token"></param>
    protected virtual async UniTask StartParticle(CancellationToken token)
    {
        //Init
        var _initCircle =
            EffectManager.Instance.PlayEffect(eff_initCircle, Vector3.zero, Quaternion.identity, particlePosition);
        var _initCharge = Instantiate(initCharge, particlePosition, false);

        float _time = 0;
        var isNotDefaultCircle = false;
        BaseEffect _defaultCircle = null;
        while (_time <= storageTime)
        {
            //適当な秒数経ってからdefaultCircleを生成する
            if (1.5f <= _time && !isNotDefaultCircle)
            {
                isNotDefaultCircle = true;
                _defaultCircle = EffectManager.Instance.PlayEffect(eff_defaultCircle, Vector3.zero, Quaternion.identity,
                    particlePosition);
            }

            if (token.IsCancellationRequested)
            {
                _initCircle.OnParticleSystemStopped();

                foreach (var particle in _initCharge.GetComponentsInChildren<ParticleSystem>())
                {
                    ParticleFadeColorRelease(particle).Forget();
                }


                if (isNotDefaultCircle)
                {
                    _defaultCircle.OnParticleSystemStopped();
                }

                ParticleRelease().Forget();
            }


            _time += Time.deltaTime;
            token.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }

        //Init終了
        _initCircle.OnParticleSystemStopped();

        //待機
        while (!isIgnite)
        {
            if (token.IsCancellationRequested)
            {
                _defaultCircle.OnParticleSystemStopped();
                ParticleRelease().Forget();
            }

            token.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }

        //Ignite
        //キャンセル処理不可
        var Ignite =
            EffectManager.Instance.PlayEffect(eff_igniteParticle, Vector3.zero, Quaternion.identity, particlePosition);

        _time = 0;
        while (_time <= 1f)
        {
            _time += Time.deltaTime;
            await UniTask.Yield();
        }

        _defaultCircle.OnParticleSystemStopped();
        ParticleRelease().Forget();
    }

    /// <summary>
    /// 引数のパーティクルをFadeOutさせる関数
    /// 削除はしないのでパーティクル自身が時間で実行するDestroyで削除する
    /// </summary>
    /// <param name="releaseParticle"></param>
    protected virtual async UniTask ParticleFadeColorRelease(ParticleSystem releaseParticle)
    {
        float _time = 0;
        var mainParticle = releaseParticle.main;
        var gradientColor = new ParticleSystem.MinMaxGradient
        {
            mode = ParticleSystemGradientMode.Color,
            color = mainParticle.startColor.color
        };
        while (_time <= 0.3f)
        {
            gradientColor.color = new Color(gradientColor.color.r, gradientColor.color.g, gradientColor.color.b,
                1 - _time / 0.3f);
            mainParticle.startColor = gradientColor;
            _time += Time.deltaTime;
            await UniTask.Yield();
        }

        gradientColor.color = new Color(gradientColor.color.r, gradientColor.color.g, gradientColor.color.b, 0);
        mainParticle.startColor = gradientColor;
        await UniTask.Yield();
    }

    /// <summary>
    /// パーティクルの削除を開始
    /// </summary>
    protected virtual async UniTask ParticleRelease()
    {
        float _time = 0;
        //Release
        //Release中はキャンセルできない
        EffectManager.Instance.PlayEffect(eff_releaseCircle, Vector3.zero, Quaternion.identity, particlePosition);
        //Instantiate(releaseCircle, particlePosition, false);
        _time = 0;
        while (_time <= 1f)
        {
            _time += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    /// <summary>
    /// 座標を0.1秒ほどかけて移動させる
    /// 複数回呼ばれた際は以前のMoveをキャンセルして新たに移動を開始する
    /// </summary>
    /// <param name="newPosition"></param>
    public virtual void Move(Vector3 newPosition)
    {
        newPosition.y += 0.1f;
        asyncMove(newPosition, MoveCancel()).Forget();
    }

    public virtual void Ignite()
    {
        isIgnite = true;
    }

    private async UniTask asyncMove(Vector3 newPosition, CancellationToken token)
    {
        float _time = 0;
        Vector3 _oldPosition = particlePosition.position;
        while (_time <= 0.1f)
        {
            Vector3 setPosition = Vector3.Slerp(_oldPosition, newPosition, _time / 0.1f);
            particlePosition.position = setPosition;
            _time += Time.deltaTime;
            token.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }
    }
}