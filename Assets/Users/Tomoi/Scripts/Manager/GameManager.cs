using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class GameManager
{
    public int Turn { get; private set; }
    private int _layerMask = 1 << 7;

    /// <summary>
    /// 現在のターンを終了する
    /// </summary>
    public void TurnComplete()
    {
        Turn++;
        isTurnComplete = true;
    }

    
    /// <summary>
    /// 魔法陣の発動を検知したか
    /// </summary>
    private bool isMagicActivation = false;
    /// <summary>
    /// オブジェクトの置き換えを検知したか
    /// </summary>
    private bool isChangedStageObject = false;
    /// <summary>
    /// ターンの終了を検知したか
    /// </summary>
    private bool isTurnComplete = false ;
    
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
    
    void OnDestroy()
    {
        //GameObject破棄時にキャンセル実行
        gameProgressCts?.Cancel();
    }
    
    /// <summary>
    /// ゲームの進行を開始する
    /// </summary>
    private async UniTask GameProgressStart(CancellationToken token)
    {
        //ゲームオーバーになるまで
        while (true)
        {
            //初期化
            isMagicActivation = false;
            isChangedStageObject = false;
            isTurnComplete = false;
            

            try
            {
                //魔法発動を待機
                Debug.Log($"GameProgress2 魔法発動を待機開始");
                //この段階でキャンセル処理が入っていたらゲームの進行を停止する
                await UniTask.WaitUntil(()=>isMagicActivation, cancellationToken: token);
                Debug.Log($"GameProgress2 魔法発動を待機終了");
            }
            catch (OperationCanceledException e)
            {
                //ゲームを停止させた時の処理
                throw;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(5));
            Mouse.Instance.MouseAct();
            
            //TurnCompleteを待機
            Debug.Log($"GameProgress2 TurnCompleteを待機開始");
            await UniTask.WaitUntil(()=>isTurnComplete);
            Debug.Log($"GameProgress2 TurnCompleteを待機終了");


            //魔法の影響を待機
            await UniTask.WaitUntil(()=>isChangedStageObject);

            
            //ターン終了処理
            foreach (var dynamicList in StageManager.Instance.DynamicStageObjectList)
            {
                foreach (var dynamicStageObject in dynamicList)
                {
                    dynamicStageObject.EndTurn().Forget();
                }
            }
            Debug.Log($"GameProgress2 ターン終了処理");
        }
        //TODO:ゲームクリア、ゲームオーバーの処理を未実装
        //TODO:SingleToneなのでどこかでバグる可能性あり
    }

    /// <summary>
    /// タイムオーバーの処理
    /// </summary>
    public async UniTask TimeOver()
    {
        GameProgressCancel();
        //TODO:タイムオーバーの演出を開始
    }
    
    /// <summary>
    /// ゲームオーバーの処理
    /// </summary>
    public async UniTask GameOver()
    {
        GameProgressCancel();
        //TODO:盤面の再生成とゲーム進行の開始
    }
    
    /// <summary>
    /// ゲームクリアの処理
    /// </summary>
    public async UniTask GameClear()
    {
        GameProgressCancel();
        //TODO:ゲームクリアの演出を開始
    }
    
    
    private Camera _camera;

    private async void Start()
    {
        //プレイヤーの魔法陣の発動をステージ生成完了まで止める
        isMagicActivation = true;
        //初期化
        _camera = Camera.main;
        //魔法陣を生成するオブジェクトを作成
        _firePlayerMagic = Instantiate(_firePlayerMagic);
        _waterPlayerMagic = Instantiate(_waterPlayerMagic);
        _icePlayerMagic = Instantiate(_icePlayerMagic);
        _windPlayerMagic = Instantiate(_windPlayerMagic);
        
        //ステージの生成完了を待機
        await StageManager.Instance.CreateStage();
        //プレイヤーの魔法陣の生成を開始
        isMagicActivation = false;

        //キャンセラレーショントークンを発行
        //ゲームの進行を開始
        GameProgressStart(GameProgressCancel()).Forget();
    }

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
    public void Magic_StartCoordinatePause(Vector2 pos, MagicType magicType)
    {
        if(isMagicActivation){return;}

        Debug.Log("Magic_StartCoordinatePause");
        var res = GetMagicPoint(pos);
        if (res.isHitArea)
        {
            res.position.y += 1f;
            GetPlayerMagic(magicType).Init(res.position);
        }
    }
    
    /// <summary>
    /// 魔法の座標の停止中
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="magicType">魔法の種類</param>
    public void Magic_CoordinatePaused(Vector2 pos, MagicType magicType)
    {
        if(isMagicActivation){return;}
        
        Debug.Log("Magic_CoordinatePaused");
        var res = GetMagicPoint(pos);
        if (res.isHitArea)
        {
            res.position.y += 1f;
            GetPlayerMagic(magicType).Move(res.position);
        }
    }

    /// <summary>
    /// 魔法の停止完了
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="vector">魔法発動方向のベクトル</param>
    /// <param name="magicType">魔法の種類</param>
    public async UniTask Magic_PauseCompleted(Vector2 pos,Vector2 vector ,MagicType magicType)
    {
        if(isMagicActivation){return;}
        // ここで魔法の種類と発動方向を引数で取得する
        Debug.Log("Magic_PauseCompleted");
        var res = GetMagicPoint(pos);
        if (res.isHitArea)
        {
            res.position.y += 1f;
            GetPlayerMagic(magicType).Ignite();

            isMagicActivation = true;
            //TODO:魔法の発動
            GetDynamicStageObject(pos)?.HitMagic(magicType,vector);
            
            //TODO:すべての処理でこの時間待機することになるので要検討
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            isChangedStageObject = true;
        }
    }
    
    /// <summary>
    /// 魔法の座標の停止キャンセル
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="magicType">魔法の種類</param>
    public void Magic_CancelCoordinatePause(Vector2 pos, MagicType magicType)
    {
        Debug.Log("Magic_CancelCoordinatePause");
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

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            return (true, hit.point);
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

    #endregion
}