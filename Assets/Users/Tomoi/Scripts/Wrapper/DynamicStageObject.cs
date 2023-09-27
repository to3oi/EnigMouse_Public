using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// ステージオブジェクトの実体
/// StageManagerやGameManagerはこのクラスの関数を呼びオブジェクトの固有処理を実行する
/// </summary>
public class DynamicStageObject : MonoBehaviour
{
    private BaseStageObject _defaultBaseStageObject;
    private StageObjectType _defaultStageObjectType;
    private BaseStageObject _nowStageObject;
    private BaseFrameOutline _frameOutline;

    public StageObjectType NowStageObjectType { get; private set; } = StageObjectType.None;

    public int StageCreateAnimationIndex { get; private set; }

    /// <summary>
    /// Unity上のワールド座標ではなくゲームの盤面上の座標
    /// </summary>
    public Vector2 Position { get; private set; }

    private bool isUpdateGroundMaterial = false;

    //InitStageAnimation周りの変数
    /// <summary>
    /// StageObjectを落下させる高さ
    /// </summary>
    /// <returns></returns>
    private const float _height = 10.0f;

    /// <summary>
    /// StageObjectを落下させる速さ
    /// </summary>
    /// <returns></returns>
    //private const float _moveSpeed = 5.0f;
    private const float _moveSpeed = 0.5f;

    /// <summary>
    /// StageObjectの落下開始の速さ
    /// </summary>
    /// <returns></returns>
    private const float _startSpeed = 7.5f;


    /// <summary>
    /// ステージ生成時に再生するアニメーションの関数
    /// これは上から落ちてくるアニメーションを含む
    /// </summary>
    public async UniTask InitStageAnimation()
    {
        //子要素の影を一時的に消す
        foreach (var meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        }
        
        //高さを調整
        var position = transform.position;
        position = new Vector3(position.x, _height, position.z);
        transform.position = position;

        //_stageCreateAnimationIndexの秒数待機
        await UniTask.Delay(TimeSpan.FromSeconds((StageCreateAnimationIndex + 3) / _startSpeed));

        //移動
        //移動距離,移動時間
        await transform.DOMoveY(0, _moveSpeed).SetEase(Ease.InQuint);
        SoundManager.Instance.PlaySE(SEType.SE2);
        //子要素の影を表示する
        //上の段階で配列保持すると参照が切れるオブジェクトがあるので毎回取得する
        foreach (var meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.shadowCastingMode = ShadowCastingMode.On;
        }
    }

    /// <summary>
    /// ステージ生成時に再生するアニメーションの関数
    /// これは上から落ちてくるアニメーションを含む
    /// </summary>
    public async UniTask ExitStageAnimation()
    {
        gameObject.layer = LayerMask.NameToLayer("None");

        //_stageCreateAnimationIndexの秒数待機
        await UniTask.Delay(TimeSpan.FromSeconds((StageCreateAnimationIndex + 3) / _startSpeed));

        //移動
        //移動距離,移動時間
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(-_height, _moveSpeed).SetEase(Ease.InQuint))
            .Join(transform.DOScale(Vector3.zero, _moveSpeed).SetEase(Ease.InQuint));
        await sequence;
        Destroy(gameObject);
    }

    public void Setup(Vector2 position, int stageCreateAnimationIndex, StageObjectType stageObjectType)
    {
        Position = position;
        StageCreateAnimationIndex = stageCreateAnimationIndex;
        _defaultStageObjectType = stageObjectType;
        NowStageObjectType = stageObjectType;

        //生成したStageObjectを変数に保持
        var baseStageObject = GenerateStageObject(stageObjectType);
        baseStageObject.Position = Position;
        _nowStageObject = baseStageObject;

        _frameOutline = Instantiate(StageManager.Instance.FrameOutlinePrefab, transform);
        _frameOutline.transform.localScale = new Vector3(0.975f, 0.975f, 0.975f);
    }


    /// <summary>
    /// 引数のMagicTypeを参照してこのStageObjectに与える影響を定義する関数
    /// </summary>
    public async void HitMagic(MagicType type, Vector2 direction)
    {
        var pos = Position;
        if (pos == Mouse.Instance.MousePosition)
        {
            //魔法の処理のあとに実行したい
            Mouse.Instance._damage = true;
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            StartCoroutine(Mouse.Instance.Death());
        }

        if (_nowStageObject.HitMagic(type, direction, out StageObjectType stageObjectType))
        {
            ReplaceBaseStageObject(stageObjectType);
        }
    }

    /// <summary>
    /// InitAnimationを再生する
    /// </summary>
    public async UniTask InitAnimation()
    {
        await _nowStageObject.InitAnimation();
    }

    /// <summary>
    /// ネズミが移動可能なマスか判定して返す
    /// </summary>
    /// <returns></returns>
    public bool isValidMove()
    {
        return _nowStageObject.isValidMove();
    }

    /// <summary>
    /// ネズミが今移動したら死亡するか判定して返す
    /// </summary>
    /// <returns></returns>
    public bool isMovedDeath()
    {
        return _nowStageObject.isMovedDeath();
    }

    /// <summary>
    /// このマスに移動したときの処理を実行
    /// </summary>
    public async UniTask MoveToCell()
    {
        await _nowStageObject.MoveToCell();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Reset()
    {
        ReplaceBaseStageObject(_defaultStageObjectType);
    }

    /// <summary>
    /// 新しいStageObjectを生成する
    /// </summary>
    private BaseStageObject GenerateStageObject(StageObjectType type)
    {
        var g = Instantiate(StageObjectList.Instance.GetGameObject(type), transform, false);
        return g.GetComponent<BaseStageObject>();
    }

    /// <summary>
    /// 現在オブジェクトの置き換えをしているか
    /// </summary>
    private bool isReplaceBaseStageObject = false;

    /// <summary>
    /// BaseStageObjectを置き換える
    /// </summary>
    public async void ReplaceBaseStageObject(StageObjectType type)
    {
        //オブジェクト置換え中にこの関数を呼び出しても置き換えの処理をしないように
        if (isReplaceBaseStageObject)
        {
            return;
        }

        isReplaceBaseStageObject = true;
        var newBaseStageObject = GenerateStageObject(type);
        newBaseStageObject.Position = Position;

        //通常の変更
        await UniTask.WhenAll(_nowStageObject.EndAnimation(),
            newBaseStageObject.InitAnimation());
        //アニメーションが終わったら

        //古いオブジェクトの削除と新しいオブジェクトの更新
        //TODO:オブジェクトプールに置き換え
        Destroy(_nowStageObject.gameObject);
        _nowStageObject = newBaseStageObject;
        NowStageObjectType = type;
        isReplaceBaseStageObject = false;
    }

    /// <summary>
    /// Outlineを非同期で表示する
    /// </summary>
    public void SetOutline(MagicType magicType)
    {
        _frameOutline.SetOutline(magicType);
    }

    /// <summary>
    /// ターン終了時に呼ぶ処理
    /// </summary>
    public async UniTask EndTurn()
    {
        await _nowStageObject.EndTurn();
    }
}