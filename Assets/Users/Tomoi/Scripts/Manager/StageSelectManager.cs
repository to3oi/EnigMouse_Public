using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageSelectManager : SingletonMonoBehaviour<StageSelectManager>
{
    [SerializeField] private GameObject _stageSelectPrefab;

    private Texture tex;

    private List<StageSelectPanel> _stageSelectPanels = new List<StageSelectPanel>();

    private Camera _camera;

    [SerializeField] private Transform[] _stageSelectPoints;

    private CancellationTokenSource _stageSelectProgressCts;
    private CancellationToken _stageSelectProgressCt;

    /// <summary>
    /// StageSelectの進行を停止する
    /// </summary>
    /// <returns></returns>
    private CancellationToken StageSelectProgressCancel()
    {
        //キャンセル
        _stageSelectProgressCts?.Cancel();
        //新しく作成
        _stageSelectProgressCts = new CancellationTokenSource();
        _stageSelectProgressCt = _stageSelectProgressCts.Token;
        return _stageSelectProgressCt;
    }

    async UniTask Start()
    {
        //初期化
        _camera = Camera.main;
        AddInputEvent();

        //魔法陣を生成するオブジェクトを作成
        _firePlayerMagic = Instantiate(_firePlayerMagic);
        _icePlayerMagic = Instantiate(_icePlayerMagic);
        _waterPlayerMagic = Instantiate(_waterPlayerMagic);
        _windPlayerMagic = Instantiate(_windPlayerMagic);


        //ステージの情報をロードする
        var mapList = StageMaps.Instance.StageMapList;
        if (5 < mapList.Count)
        {
            Debug.LogError("マップの数が5を超えています");
        }

        List<UniTask> task = new List<UniTask>();
        //ステージの数だけ_stageSelectPrefabを生成する
        for (int i = 0; i < (mapList.Count < 5 ? mapList.Count : 5); i++)
        {
            var StageSelectPanel = Instantiate(_stageSelectPrefab).GetComponent<StageSelectPanel>();
            //生成した_stageSelectPrefabをリストで保持する
            _stageSelectPanels.Add(StageSelectPanel);

            var g = new GameObject($"StageSelectPanel{i}");
            g.transform.position = _stageSelectPoints[i].position;
            StageSelectPanel.Init(mapList[i].StageSelectTexture,mapList[i].StageSelectMaskTexture, i, g.transform,mapList[i].isHardMode);
            StageSelectPanel.transform.parent = g.transform;
            StageSelectPanel.transform.localPosition = Vector3.zero;
            task.Add(StageSelectPanel.InitAnimation(i / 5f));
        }

        await UniTask.WhenAll(task);

        //生成後に入力の受付を開始
        isMagicActivation = true;

        StageSelectProgressStart(StageSelectProgressCancel()).Forget();
    }

    /// <summary>
    /// ステージ選択の進行を開始する
    /// </summary>
    /// <param name="token"></param>
    private async UniTask StageSelectProgressStart(CancellationToken token)
    {
        //_stageSelectPrefabが選択されるのを待機
        await UniTask.WaitUntil(() => _selectedStageSelectPanel != null, cancellationToken: token);

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        
        List<UniTask> task = new List<UniTask>();
        //選択されていないパネルを燃やす
        for (int i = 0; i < _stageSelectPanels.Count; i++)
        {
            if (_stageSelectPanels[i] != _selectedStageSelectPanel)
            {
                task.Add(_stageSelectPanels[i].StartBurn(Random.Range(0.05f, 0.15f) + (i * 0.15f)));
            }
        }

        //すべてのパネルが燃えるまで待機
        await UniTask.WhenAll(task);

        //待機
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        //選択したStageSelectPanelを拡大
        Sequence sequence = DOTween.Sequence();
        sequence.Join(_selectedStageSelectPanel.RootTransform.DOMove(_stageSelectPoints[4].position, 0.25f));
        sequence.Join(_selectedStageSelectPanel.RootTransform.DOScale(3, 0.25f));

        SoundManager.Instance.AllStopBGM().Forget();
        //待機
        await UniTask.Delay(TimeSpan.FromSeconds(3));

        //暗転
        GetCanvas.Instance.FadeImage.color = Color.black;
        SoundManager.Instance.PlaySE(SEType.SE33);
        await UniTask.Delay(TimeSpan.FromSeconds(2));

        //シーン移動
        SceneManager.Instance.SceneChange(SceneList.MainGame, false, true, isWhite: false, fadeTime: 0);
    }

    /// <summary>
    /// プレイヤーが選択したStageSelectPanel
    /// </summary>
    private StageSelectPanel _selectedStageSelectPanel = null;

    /// <summary>
    /// 選択したStageSelectPanelから続きの演出をする
    /// </summary>
    public void SelectedStageSelectPanel(StageSelectPanel stageSelectPanel)
    {
        _selectedStageSelectPanel = stageSelectPanel;
        ValueRetention.Instance.StageIndex = _selectedStageSelectPanel.MapIndex;
    }
    /// <summary>
    /// Outlineを非同期で表示
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="magicType"></param>
    public void SetOutline(Vector2 pos, MagicType magicType)
    {
        GetStageSelectPanel(pos)?.SetOutline(magicType);
    }

    #region 魔法関係

    [SerializeField] private BasePlayerMagic _firePlayerMagic;
    [SerializeField] private BasePlayerMagic _icePlayerMagic;
    [SerializeField] private BasePlayerMagic _waterPlayerMagic;
    [SerializeField] private BasePlayerMagic _windPlayerMagic;
    private MagicType canUseMagicType = MagicType.NoneMagic;

    /// <summary>
    /// 魔法の発動許可するか
    /// </summary>
    private bool isMagicActivation = false;

    /// <summary>
    /// 魔法の座標の停止開始
    /// </summary>
    /// <param name="pos">画面上の座標</param>
    /// <param name="magicType">魔法の種類</param>
    private void Magic_StartCoordinatePause(Vector2 pos, MagicType magicType)
    {
        if (!isMagicActivation)
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
        if (!isMagicActivation)
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
        if (!isMagicActivation)
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

            //選択は一度のみ許可
            isMagicActivation = false;

            //他の魔法陣のキャンセル
            AllCancelPlayerMagic(magicType);

            //
            GetStageSelectPanel(pos)?.SelectedThisPanel();

            //TODO:すべての処理でこの時間待機することになるので要検討
            await UniTask.Delay(TimeSpan.FromSeconds(3));
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

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("StageSelect")))
        {
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

            //パネルに魔法が埋まるので Vector3.up * 0.1f を追加
            return (true, hit.point + Vector3.up * 0.1f);
        }

        return (false, Vector3.zero);
    }

    /// <summary>
    /// レイキャストで画面上の座標からStageSelectPanelを取得する
    /// なければnullを返す
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private StageSelectPanel GetStageSelectPanel(Vector2 pos)
    {
        Ray ray = _camera.ScreenPointToRay(pos);

        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("StageSelect")))
        {
            return hit.transform.GetComponent<StageSelectPanel>();
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
        //TODO:削除する?
        //gameProgressCts?.Cancel();

        RemoveInputEvent();
    }

    #endregion
}