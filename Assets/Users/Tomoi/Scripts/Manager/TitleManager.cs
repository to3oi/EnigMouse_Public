using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    [SerializeField] private List<Transform> MagicPosition = new List<Transform>();
    private CancellationTokenSource gameProgressCts;
    private CancellationToken gameProgressCt;


    /// <summary>
    /// 魔法陣の発動を検知したか
    /// </summary>
    private bool isMagicActivation = false;

    /// <summary>
    /// オブジェクトの置き換えを検知したか
    /// </summary>
    private bool isChangedStageObject = false;
    
    private SoundHash BgmHash;
    
    private MagicType canUseMagicType = MagicType.NoneMagic;

    /// <summary>
    /// ゲームの進行を停止する
    /// </summary>
    /// <returns></returns>
    private CancellationToken TitleProgressCancel()
    {
        //キャンセル
        gameProgressCts?.Cancel();
        //新しく作成
        gameProgressCts = new CancellationTokenSource();
        gameProgressCt = gameProgressCts.Token;
        return gameProgressCt;
    }

    /// <summary>
    /// ゲームの進行を開始する
    /// </summary>
    private async UniTask TitleProgressStart(CancellationToken token)
    {
        MagicPosition.ForEach(x => x.gameObject.SetActive(false));
        await UniTask.DelayFrame(1);

        //Fire
        MagicPosition[0].gameObject.SetActive(true);
        var pos = MagicPosition[0].position + Vector3.up * 0.01f;
        EffectManager.Instance.PlayEffect(EffectType.Magic_Fire_initCircle, pos, Quaternion.identity);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        var fireCircle =
            EffectManager.Instance.PlayEffect(EffectType.Magic_Fire_defaultCircle, pos, Quaternion.identity);
        canUseMagicType = MagicType.Fire;
        InputManager.Instance.ResetMagicData(MagicType.Fire);
        token.ThrowIfCancellationRequested();
        //魔法発動を待機
        await UniTask.WaitUntil(() => isMagicActivation);
        EffectManager.Instance.PlayEffect(EffectType.Fire_UI, pos, Quaternion.identity);
        canUseMagicType = MagicType.NoneMagic; 
        //処理終了まで待機
        await UniTask.WaitUntil(() => !isMagicActivation);
        fireCircle.OnParticleSystemStopped();
        EffectManager.Instance.PlayEffect(EffectType.Magic_Fire_releaseCircle, pos, Quaternion.identity);
        MagicPosition[0].gameObject.SetActive(false);
        InputManager.Instance.ResetMagicData(MagicType.Fire);

        //魔法発動後に少し待機
        await UniTask.Delay(TimeSpan.FromSeconds(2));


        //Ice
        MagicPosition[1].gameObject.SetActive(true);
        pos = MagicPosition[1].position + Vector3.up * 0.01f;
        EffectManager.Instance.PlayEffect(EffectType.Magic_Ice_initCircle, pos, Quaternion.identity);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        var iceCircle = EffectManager.Instance.PlayEffect(EffectType.Magic_Ice_defaultCircle, pos, Quaternion.identity);
        canUseMagicType = MagicType.Ice;
        InputManager.Instance.ResetMagicData(MagicType.Ice);
        token.ThrowIfCancellationRequested();
        //魔法発動を待機
        await UniTask.WaitUntil(() => isMagicActivation);
        EffectManager.Instance.PlayEffect(EffectType.Ice_UI, pos, Quaternion.identity);
        canUseMagicType = MagicType.NoneMagic;
        //処理終了まで待機
        await UniTask.WaitUntil(() => !isMagicActivation);
        iceCircle.OnParticleSystemStopped();
        EffectManager.Instance.PlayEffect(EffectType.Magic_Ice_releaseCircle, pos, Quaternion.identity);
        MagicPosition[1].gameObject.SetActive(false);
        InputManager.Instance.ResetMagicData(MagicType.Ice);

        //魔法発動後に少し待機
        await UniTask.Delay(TimeSpan.FromSeconds(2));


        //Water
        MagicPosition[2].gameObject.SetActive(true);
        pos = MagicPosition[2].position + Vector3.up * 0.01f;
        EffectManager.Instance.PlayEffect(EffectType.Magic_Water_initCircle, pos, Quaternion.identity);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        var waterCircle =
            EffectManager.Instance.PlayEffect(EffectType.Magic_Water_defaultCircle, pos, Quaternion.identity);
        canUseMagicType = MagicType.Water;
        InputManager.Instance.ResetMagicData(MagicType.Water);
        token.ThrowIfCancellationRequested();
        //魔法発動を待機
        await UniTask.WaitUntil(() => isMagicActivation);
        EffectManager.Instance.PlayEffect(EffectType.Water_UI, pos, Quaternion.identity);
        canUseMagicType = MagicType.NoneMagic;
        //処理終了まで待機
        await UniTask.WaitUntil(() => !isMagicActivation);
        waterCircle.OnParticleSystemStopped();
        EffectManager.Instance.PlayEffect(EffectType.Magic_Water_releaseCircle, pos, Quaternion.identity);
        MagicPosition[2].gameObject.SetActive(false);
        InputManager.Instance.ResetMagicData(MagicType.Water);

        //魔法発動後に少し待機
        await UniTask.Delay(TimeSpan.FromSeconds(2));


        //Wind
        MagicPosition[3].gameObject.SetActive(true);
        pos = MagicPosition[3].position + Vector3.up * 0.01f;
        EffectManager.Instance.PlayEffect(EffectType.Magic_Wind_initCircle, pos, Quaternion.identity);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        var windCircle =
            EffectManager.Instance.PlayEffect(EffectType.Magic_Wind_defaultCircle, pos, Quaternion.identity);
        canUseMagicType = MagicType.Wind;
        InputManager.Instance.ResetMagicData(MagicType.Wind);
        token.ThrowIfCancellationRequested();
        //魔法発動を待機
        await UniTask.WaitUntil(() => isMagicActivation);
        EffectManager.Instance.PlayEffect(EffectType.Wind_UI, pos, Quaternion.identity);
        canUseMagicType = MagicType.NoneMagic;
        //処理終了まで待機
        await UniTask.WaitUntil(() => !isMagicActivation);
        windCircle.OnParticleSystemStopped();
        EffectManager.Instance.PlayEffect(EffectType.Magic_Wind_releaseCircle, pos, Quaternion.identity);
        MagicPosition[3].gameObject.SetActive(false);
        InputManager.Instance.ResetMagicData(MagicType.Wind);

        //TODO:シーン移動
        SoundManager.Instance.StopBGM(BgmHash);
        SceneManager.Instance.SceneChange(SceneList.MainGame, true, true,false);
    }

    private void Start()
    {
        //初期化
        _camera = Camera.main;

        //魔法陣を生成するオブジェクトを作成
        _firePlayerMagic = Instantiate(_firePlayerMagic);
        _icePlayerMagic = Instantiate(_icePlayerMagic);
        _waterPlayerMagic = Instantiate(_waterPlayerMagic);
        _windPlayerMagic = Instantiate(_windPlayerMagic);

        BgmHash = SoundManager.Instance.PlayBGM(BGMType.BGM1);
        TitleProgressStart(TitleProgressCancel()).Forget();
    }

    void OnDestroy()
    {
        //GameObject破棄時にキャンセル実行
        gameProgressCts?.Cancel();
    }

    private Camera _camera;


    #region 魔法関係

    [SerializeField] private BasePlayerMagic _firePlayerMagic;
    [SerializeField] private BasePlayerMagic _icePlayerMagic;
    [SerializeField] private BasePlayerMagic _waterPlayerMagic;
    [SerializeField] private BasePlayerMagic _windPlayerMagic;


    /// <summary>
    /// 魔法の座標の停止開始
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="magicType">魔法の種類</param>
    public void Magic_StartCoordinatePause(Vector2 pos, MagicType magicType)
    {Debug.Log($"MagicType :{magicType})");
        if (canUseMagicType != magicType)
        {
            return;
        }

        if (isMagicActivation)
        {
            return;
        }

        Debug.Log("Magic_StartCoordinatePause");
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
    public void Magic_CoordinatePaused(Vector2 pos, MagicType magicType)
    {
        if (canUseMagicType != magicType)
        {
            return;
        }

        if (isMagicActivation)
        {
            return;
        }

        Debug.Log("Magic_CoordinatePaused");
        var res = GetMagicPoint(pos);
        if (res.isHitArea)
        {
            GetPlayerMagic(magicType).Move(res.position);
        }
    }

    /// <summary>
    /// 魔法の停止完了
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="vector">魔法発動方向のベクトル</param>
    /// <param name="magicType">魔法の種類</param>
    public async UniTask Magic_PauseCompleted(Vector2 pos, Vector2 vector, MagicType magicType)
    {
        if (canUseMagicType != magicType)
        {
            return;
        }

        if (isMagicActivation)
        {
            return;
        }

        // ここで魔法の種類と発動方向を引数で取得する
        Debug.Log("Magic_PauseCompleted");
        var res = GetMagicPoint(pos);
        if (res.isHitArea)
        {
            GetPlayerMagic(magicType).Ignite();

            isMagicActivation = true;

            //他の魔法陣のキャンセル
            AllCancelPlayerMagic(magicType);

            //魔法の発動
            GetDynamicStageObject(pos)?.HitMagic(magicType, vector);

            //TODO:すべての処理でこの時間待機することになるので要検討
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            isMagicActivation = false;
        }
        else
        {
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


        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
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
            case MagicType.Ice:
                return _icePlayerMagic;
                break;
            case MagicType.Water:
                return _waterPlayerMagic;
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
        if (magicType != MagicType.Ice)
        {
            _icePlayerMagic.Release();
        }
        
        if (magicType != MagicType.Water)
        {
            _waterPlayerMagic.Release();
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
        _icePlayerMagic.Release();
        _waterPlayerMagic.Release();
        _windPlayerMagic.Release();
    }

    #endregion
}