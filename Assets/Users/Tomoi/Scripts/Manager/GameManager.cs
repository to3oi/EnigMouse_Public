using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UniRx;

public partial class GameManager
{
    #region ゲームの設定

    /// <summary>
    /// ゲームの制限時間
    /// </summary>
    [SerializeField] private float _timeLimit = 300.0f;

    /// <summary>
    /// 最大ターン数
    /// </summary>
    [SerializeField] private int MaxTurn = 4;

    /// <summary>
    /// ゲーム中にそれぞれのプレイヤーが魔法を使わなければいけない回数
    /// </summary>
    [SerializeField] private int magicUsageCount = 0;

    #endregion

    public int Turn { get; private set; }
    private int _layerMask = 1 << 7;

    private Camera _camera;

    /// <summary>
    /// 魔法陣の発動を検知したか
    /// </summary>
    [SerializeField] private bool isMagicActivation = false;

    /// <summary>
    /// オブジェクトの置き換えを検知したか
    /// </summary>
    private bool isChangedStageObject = false;

    /// <summary>
    /// ターンの終了を検知したか
    /// </summary>
    private bool isTurnComplete = false;

    /// <summary>
    /// ゲームクリアしたときにtrue
    /// </summary>
    private bool isGameClear = false;
    
    /// <summary>
    /// タイムオーバーの演出終了時にtrue
    /// </summary>
    private bool isTimeOver = false;

    [SerializeField] private GameOver _gameOver;
    private GameTimer _gameTimer;
    public GameTimer Timer => _gameTimer;

    public float MaxTime => _timeLimit;
    private GameClear gameClear;

    public GameClear SetGameClear
    {
        set { gameClear = value; }
    }

    private CancellationTokenSource gameProgressCts;
    private CancellationToken gameProgressCt;

    /// <summary>
    /// ゲームの進行を停止する
    /// </summary>
    /// <returns></returns>
    private CancellationToken GameProgressCancel()
    {
        //キャンセル
        gameProgressCts?.Cancel();
        //新しく作成
        gameProgressCts = new CancellationTokenSource();
        gameProgressCt = gameProgressCts.Token;
        return gameProgressCt;
    }

    private CancellationTokenSource countDownProgressCts;
    private CancellationToken countDownProgressCt;

    /// <summary>
    /// カウントダウンを停止する
    /// </summary>
    /// <returns></returns>
    private CancellationToken CountDownProgressCancel()
    {
        //キャンセル
        countDownProgressCts?.Cancel();
        //新しく作成
        countDownProgressCts = new CancellationTokenSource();
        countDownProgressCt = countDownProgressCts.Token;
        return countDownProgressCt;
    }

    #region マスク処理用

    [SerializeField] private CanvasGroup mouseMaskCanvasGroup;
    [SerializeField] private RectTransform unmask;

    #endregion

    #region テキスト表示用

    [SerializeField] private CanvasGroup messageCanvasBackGround;
    [SerializeField] private CanvasGroup textRootCanvasGroup;
    [SerializeField] private TextMeshProUGUI[] messageAreas;

    #endregion


    #region 魔法の発動制限用の変数

    /// <summary>
    /// 魔法が発動済みならtrue
    /// </summary>
    private bool[] checkMagicType;

    #endregion


    #region ゲームの進行

    private SoundHash BGMHash;

    private Subject<float> _timeSubject = new Subject<float>();
    public IObservable<float> TimeObservable => _timeSubject;

    private float initTimeSpan = 1;

    private async void Start()
    {
        //プレイヤーの魔法陣の発動をステージ生成完了まで止める
        isMagicActivation = true;

        #region 変数などの初期化

        AddInputEvent();

        _camera = Camera.main;
        mouseMaskCanvasGroup.alpha = 0;
        messageCanvasBackGround.alpha = 0;
        textRootCanvasGroup.alpha = 0;

        //魔法陣を生成するオブジェクトを作成
        _firePlayerMagic = Instantiate(_firePlayerMagic);
        _waterPlayerMagic = Instantiate(_waterPlayerMagic);
        _icePlayerMagic = Instantiate(_icePlayerMagic);
        _windPlayerMagic = Instantiate(_windPlayerMagic);
        _gameTimer = gameObject.AddComponent<GameTimer>();
        _gameTimer.SetTime(_timeLimit);

        #endregion

        #region 盤面の初期アニメーションを再生

        //ステージの生成完了を待機
        await StageManager.Instance.CreateStage();

        //BGM開始
        BGMHash = SoundManager.Instance.PlayBGM(BGMType.BGM5);

        //ネズミの周りを暗くしてネズミにアクションを取らせる
        await Mouse.Instance.MouseStartAnim();
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        
        //await UniTask.Delay(TimeSpan.FromSeconds(5));

        //ネズミのマスクを解除
        await UpdateCanvasGroupAlpha(1, 0, 2, mouseMaskCanvasGroup);

        //ネズミのマスク用のオブジェクトを削除
        Destroy(mouseMaskCanvasGroup.gameObject);

        await UniTask.Delay(TimeSpan.FromSeconds(initTimeSpan));
        
        //TODO:制限時間のゲージを満たす
        //TODO:ゲージ以外を暗くしてテキスト表示
        //制限時間は5分
        //messageCanvasGroupを表示する
        await UpdateCanvasGroupAlpha(0, 1, 2, messageCanvasBackGround);
        
        List<UniTask> task = new List<UniTask>();
        task.Add( DOVirtual.Float(0, _timeLimit, 4, x => { _timeSubject.OnNext(x); }).ToUniTask());
        task.Add( MessageUpdate(3, "制限時間は5分"));
        await UniTask.WhenAll(task);

        await UniTask.Delay(TimeSpan.FromSeconds(initTimeSpan));

        //背景を拡大
        var mcbgRectTransform = messageCanvasBackGround.transform.GetComponent<RectTransform>();
        await mcbgRectTransform.DOScale(2, 1);

        //全体を暗くしてテキスト表示
        //魔法を使いこなし
        //3s
        await MessageUpdate(3, "魔法を使いこなし");

        await UniTask.Delay(TimeSpan.FromSeconds(initTimeSpan));

        //続き テキスト表示
        //ネズミに鍵を取らせ、魔法陣へ誘導しろ
        //5s
        await MessageUpdate(5, "ネズミに鍵を取らせ\n魔法陣へ誘導しろ");

        await UpdateCanvasGroupAlpha(1, 0, 2, messageCanvasBackGround);

        //BGM停止
        await SoundManager.Instance.StopBGM(BGMHash,0.5f);
        
        await UniTask.Delay(TimeSpan.FromSeconds(initTimeSpan));
        
        //カウントダウン
        //3,2,1,Start
        //フォントサイズを変更
        foreach (var textMeshProUGUI in messageAreas)
        {
            textMeshProUGUI.fontSize = 100;
        }


        SoundManager.Instance.PlaySE(SEType.CountDown);
        await MessageUpdate(0.5f, "3",0.25f);
        SoundManager.Instance.PlaySE(SEType.CountDown);
        await MessageUpdate(0.5f, "2",0.25f);
        SoundManager.Instance.PlaySE(SEType.CountDown);
        await MessageUpdate(0.5f, "1",0.25f);
        SoundManager.Instance.PlaySE(SEType.TimeOver);
        await MessageUpdate(1.5f, "Start",0.25f);

        //最後にメッセージキャンバスを削除
        Destroy(messageCanvasBackGround.transform.parent.gameObject);

        #endregion

        //プレイヤーの魔法陣の生成を開始
        isMagicActivation = false;

        //時間が更新されたら通知
        _gameTimer.ObserveEveryValueChanged(x => x.TimeLimit).Subscribe(x =>
        {
            _timeSubject.OnNext(x);
        }).AddTo(this);
        
        //キャンセラレーショントークンを発行
        //ゲームの進行を開始
        BGMHash = SoundManager.Instance.PlayBGM(BGMType.BGM2);
        GameProgressStart(GameProgressCancel()).Forget();
    }

    /// <summary>
    /// ゲームの進行を開始する
    /// </summary>
    private async UniTask GameProgressStart(CancellationToken token)
    {
        ResetCheckMagicType();
        _gameTimer.TimerStart();
        LimitReset();
        Turn = 0;

        //ゲームオーバーになるまで
        while (true)
        {
            //直近の入力を初期化
            AllCancelPlayerMagic();
            InputManager.Instance.ResetMagicData(MagicType.Fire);
            InputManager.Instance.ResetMagicData(MagicType.Ice);
            InputManager.Instance.ResetMagicData(MagicType.Water);
            InputManager.Instance.ResetMagicData(MagicType.Wind);
            InputManager.Instance.ResetMagicDataDictionary();
            isMagicActivation = false;
            isChangedStageObject = false;
            isTurnComplete = false;


            //魔法発動を待機
            //この段階でキャンセル処理が入っていたらゲームの進行を停止する
            await UniTask.WaitUntil(() => isMagicActivation, cancellationToken: token);


            //魔法使用後にネズミが動くまでの秒数を調整
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            Mouse.Instance.MouseAct();

            //TurnCompleteを待機
            await UniTask.WaitUntil(() => isTurnComplete);

            //ターン数チェック
            if (MaxTurn <= Turn)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                Mouse.Instance.HideMouse();
                GameOver().Forget();
            }

            //魔法の影響を待機
            await UniTask.WaitUntil(() => isChangedStageObject);


            //ターン終了処理
            foreach (var dynamicList in StageManager.Instance.DynamicStageObjectList)
            {
                foreach (var dynamicStageObject in dynamicList)
                {
                    dynamicStageObject.EndTurn().Forget();
                }
            }

            token.ThrowIfCancellationRequested();
        }
    }

    /// <summary>
    /// 現在のターンを終了する
    /// </summary>
    public void TurnComplete()
    {
        Turn++;
        isTurnComplete = true;
    }

    #endregion

    #region ゲーム中の演出

    public async UniTask CountDown()
    {
        await _gameOver.StartCountDown(CountDownProgressCancel());
    }

    /// <summary>
    /// ゲームオーバーの処理
    /// </summary>
    public async UniTask GameOver()
    {
        var token = GameProgressCancel();
        InitEffectUI();

        //盤面の再生成とゲーム進行の開始
        await StageManager.Instance.RegenerationStage();
        //ネズミのリセット処理はStageObjectMouseのInitAnimation内で実行されます

        //生成が終わったらスタート
        GameProgressStart(token).Forget();
    }

    /// <summary>
    /// タイムオーバーの処理
    /// </summary>
    [ContextMenu("TimeOver")]
    public async UniTask TimeOver()
    {
        SoundManager.Instance.StopBGM(BGMHash).Forget();
        GameProgressCancel();
        CountDownProgressCancel();
        _gameTimer.TimerStop();
        //SEの鳴り止みを待機
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        SoundManager.Instance.PlaySE(SEType.TimeOver);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        SoundManager.Instance.PlaySE(SEType.TimeOver);
        await _gameOver.TimeOverStart();
        //ゲームクリアとタイムオーバーが同時に実行されているときはゲームクリアを優先的に実行
        if (!isGameClear)
        {
            isTimeOver = true;
            SceneManager.Instance.SceneChange(SceneList.GameOver, true, true);
        }
    }

    /// <summary>
    /// ゲームクリアの処理
    /// </summary>
    [ContextMenu("GameClear")]
    public async UniTask GameClear()
    {
        if(isTimeOver){return;}
        
        isGameClear = true;
        SoundManager.Instance.StopBGM(BGMHash).Forget();
        GameProgressCancel();
        CountDownProgressCancel();
        _gameTimer.TimerStop();

        SoundManager.Instance.PlaySE(SEType.GameClear);

        await gameClear.GameClearStart();
        SceneManager.Instance.SceneChange(SceneList.GameClear, true, true);
    }

    /// <summary>
    /// ネズミの座標でマスク処理をする
    /// </summary>
    /// <param name="stageObjectMousePosition"></param>
    public void SetMouseMask(Vector3 stageObjectMousePosition)
    {
        unmask.position = _camera.WorldToScreenPoint(stageObjectMousePosition);
        UpdateCanvasGroupAlpha(0, 1, 2, mouseMaskCanvasGroup).Forget();
    }


    private async UniTask MessageUpdate(float displayTime, string text,float fadeTime = 0.5f)
    {
        foreach (var textMeshProUGUI in messageAreas)
        {
            textMeshProUGUI.text = text;
        }

        await UpdateCanvasGroupAlpha(0, 1, fadeTime, textRootCanvasGroup);
        await UniTask.Delay(TimeSpan.FromSeconds(displayTime));
        await UpdateCanvasGroupAlpha(1, 0, fadeTime, textRootCanvasGroup);
    }

    private async UniTask UpdateCanvasGroupAlpha(float from, float to, float duration, CanvasGroup canvasGroup)
    {
        await DOVirtual.Float(from, to, duration, f => { canvasGroup.alpha = f; });
    }

    private void ResetCheckMagicType()
    {
        checkMagicType = new[] { false, false, false, false, false };
    }

    /// <summary>
    /// Outlineを非同期で表示
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="magicType"></param>
    public void SetOutline(Vector2 pos, MagicType magicType)
    {
        GetDynamicStageObject(pos)?.SetOutline(magicType);
    }

    #endregion

    #region 魔法関係

    [SerializeField] private BasePlayerMagic _firePlayerMagic;
    [SerializeField] private BasePlayerMagic _waterPlayerMagic;
    [SerializeField] private BasePlayerMagic _icePlayerMagic;
    [SerializeField] private BasePlayerMagic _windPlayerMagic;


    /// <summary>
    /// 魔法の座標の停止開始
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="magicType">魔法の種類</param>
    private void Magic_StartCoordinatePause(Vector2 pos, MagicType magicType)
    {
        if (isMagicActivation)
        {
            return;
        }

        if (checkMagicType[(int)magicType])
        {
            return;
        }

        var res = GetMagicPoint(pos);
        if (res.isHitArea)
        {
            GetPlayerMagic(magicType).Init(res.position);
        }
    }

    /// <summary>
    /// 魔法の座標の停止中
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="magicType">魔法の種類</param>
    private void Magic_CoordinatePaused(Vector2 pos, MagicType magicType)
    {
        if (isMagicActivation)
        {
            return;
        }

        if (checkMagicType[(int)magicType])
        {
            return;
        }

        var res = GetMagicPoint(pos);
        if (res.isHitArea)
        {
            GetPlayerMagic(magicType).Move(res.position);
        }
    }

    private void Magic_PauseCompleted_Start(Vector2 pos, Vector2 vector, MagicType magicType)
    {
        if (isMagicActivation)
        {
            return;
        }

        if (checkMagicType[(int)magicType])
        {
            return;
        }

        Magic_PauseCompleted(pos, vector, magicType).Forget();
    }

    /// <summary>
    /// 魔法の停止完了
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="vector">魔法発動方向のベクトル</param>
    /// <param name="magicType">魔法の種類</param>
    private async UniTask Magic_PauseCompleted(Vector2 pos, Vector2 vector, MagicType magicType)
    {
        // ここで魔法の種類と発動方向を引数で取得する
        var res = GetMagicPoint(pos);
        if (res.isHitArea)
        {
            GetPlayerMagic(magicType).Ignite();

            isMagicActivation = true;

            //他の魔法陣のキャンセル
            AllCancelPlayerMagic(magicType);

            //魔法の発動
            GetDynamicStageObject(pos)?.HitMagic(magicType, vector);
            MagicUsageCountUp(magicType);
            UpdateEffectUI();
            checkMagicType[(int)magicType] = true;

            await UniTask.Delay(TimeSpan.FromSeconds(3));
            isChangedStageObject = true;
        }
        else
        {
            //DynamicStageObject以外の上で魔法の発動時間が経過したときにもう一度魔法が打てるようにリセットする
            InputManager.Instance.ResetMagicDataDictionary(magicType);
        }
    }

    /// <summary>
    /// 魔法の座標の停止キャンセル
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="magicType">魔法の種類</param>
    public void Magic_CancelCoordinatePause(Vector2 pos, MagicType magicType)
    {
        GetPlayerMagic(magicType).Release();
    }

    /// <summary>
    /// レイキャストで盤面上の座標を取得する
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private (bool isHitArea, Vector3 position) GetMagicPoint(Vector2 pos)
    {
        Ray ray = _camera.ScreenPointToRay(pos);

        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("DynamicStageObject")))
        {
            Vector3 reverseDirection = -ray.direction;
            return (true, hit.point + reverseDirection * 1);
        }

        return (false, Vector3.zero);
    }

    /// <summary>
    /// レイキャストで画面上の座標からDynamicStageObjectを取得する
    /// なければnullを返す
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private DynamicStageObject GetDynamicStageObject(Vector2 pos)
    {
        Ray ray = _camera.ScreenPointToRay(pos);

        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("DynamicStageObject")))
        {
            return hit.transform.GetComponent<DynamicStageObject>();
        }

        return null;
    }

    /// <summary>
    /// MagicTypeに対応したBasePlayerMagicを取得する
    /// </summary>
    /// <param name="magicType"></param>
    /// <returns></returns>
    private BasePlayerMagic GetPlayerMagic(MagicType magicType)
    {
        switch (magicType)
        {
            case MagicType.Fire:
                return _firePlayerMagic;
                break;
            case MagicType.Water:
                return _waterPlayerMagic;
                break;
            case MagicType.Ice:
                return _icePlayerMagic;
                break;
            case MagicType.Wind:
                return _windPlayerMagic;
                break;
        }

        return null;
    }

    /// <summary>
    /// すべての魔法陣のキャンセル処理をする
    /// </summary>
    /// <param name="magicType">この魔法陣のキャンセル処理は実行しない</param>
    private void AllCancelPlayerMagic(MagicType magicType)
    {
        if (magicType != MagicType.Fire)
        {
            _firePlayerMagic.Release();
        }

        if (magicType != MagicType.Water)
        {
            _waterPlayerMagic.Release();
        }

        if (magicType != MagicType.Ice)
        {
            _icePlayerMagic.Release();
        }

        if (magicType != MagicType.Wind)
        {
            _windPlayerMagic.Release();
        }
    }

    /// <summary>
    /// すべての魔法陣のキャンセル処理をする
    /// </summary>
    private void AllCancelPlayerMagic()
    {
        _firePlayerMagic.Release();
        _waterPlayerMagic.Release();
        _icePlayerMagic.Release();
        _windPlayerMagic.Release();
    }

    #endregion

    #region ターンなどの制限処理

    private List<int> magicUsageCountList = new List<int>();

    private void LimitReset()
    {
        magicUsageCountList = new List<int>(4);
        for (int i = 0; i < 4; i++)
        {
            magicUsageCountList.Add(0);
        }
    }

    private void MagicUsageCountUp(MagicType magicType)
    {
        if (magicType == MagicType.NoneMagic)
        {
            return;
        }

        magicUsageCountList[(int)magicType - 1]++;
    }

    /// <summary>
    /// ゲームのクリア条件を満たしているか
    /// true:クリア条件を満たしている
    /// </summary>
    /// <returns></returns>
    public bool IsLimitCheck()
    {
        //クリアマスの制限チェック
        return Turn <= MaxTurn
               && magicUsageCount <= magicUsageCountList[0]
               && magicUsageCount <= magicUsageCountList[1]
               && magicUsageCount <= magicUsageCountList[2]
               && magicUsageCount <= magicUsageCountList[3];
    }

    private BaseEffect EffectUI_Fire = null;
    private BaseEffect EffectUI_Ice = null;
    private BaseEffect EffectUI_Water = null;
    private BaseEffect EffectUI_Wind = null;

    [SerializeField] private Transform[] effectUIPosition;

    private void InitEffectUI()
    {
        //UIが生成済みなら停止
        EffectUI_Fire?.Stop();
        EffectUI_Ice?.Stop();
        EffectUI_Water?.Stop();
        EffectUI_Wind?.Stop();

        EffectUI_Fire = null;
        EffectUI_Ice = null;
        EffectUI_Water = null;
        EffectUI_Wind = null;
    }

    private void UpdateEffectUI()
    {
        if (magicUsageCount <= magicUsageCountList[0] && EffectUI_Fire == null)
        {
            EffectUI_Fire = EffectManager.Instance.PlayEffect(EffectType.Fire_UI, effectUIPosition[0].position,
                Quaternion.identity);
        }

        if (magicUsageCount <= magicUsageCountList[1] && EffectUI_Ice == null)
        {
            EffectUI_Ice =
                EffectManager.Instance.PlayEffect(EffectType.Ice_UI, effectUIPosition[1].position, Quaternion.identity);
        }

        if (magicUsageCount <= magicUsageCountList[2] && EffectUI_Water == null)
        {
            EffectUI_Water = EffectManager.Instance.PlayEffect(EffectType.Water_UI, effectUIPosition[2].position,
                Quaternion.identity);
        }

        if (magicUsageCount <= magicUsageCountList[3] && EffectUI_Wind == null)
        {
            EffectUI_Wind = EffectManager.Instance.PlayEffect(EffectType.Wind_UI, effectUIPosition[3].position,
                Quaternion.identity);
        }
    }

    #endregion

    #region イベント

    private void AddInputEvent()
    {
        InputManager.Instance.MagicPositionAlways += SetOutline;
        InputManager.Instance.Magic_StartCoordinatePause += Magic_StartCoordinatePause;
        InputManager.Instance.Magic_CoordinatePaused += Magic_CoordinatePaused;
        InputManager.Instance.Magic_PauseCompleted += Magic_PauseCompleted_Start;
    }

    private void RemoveInputEvent()
    {
        InputManager.Instance.MagicPositionAlways -= SetOutline;
        InputManager.Instance.Magic_StartCoordinatePause -= Magic_StartCoordinatePause;
        InputManager.Instance.Magic_CoordinatePaused -= Magic_CoordinatePaused;
        InputManager.Instance.Magic_PauseCompleted -= Magic_PauseCompleted_Start;
    }


    void OnDestroy()
    {
        //GameObject破棄時にキャンセル実行
        gameProgressCts?.Cancel();

        RemoveInputEvent();
    }

    #endregion
}