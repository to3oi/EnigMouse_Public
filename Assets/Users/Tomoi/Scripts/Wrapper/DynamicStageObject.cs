using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// ステージオブジェクトの実体
/// StageManagerやGameManagerはこのクラスの関数を呼びオブジェクトの固有処理を実行する
/// </summary>
public class DynamicStageObject : MonoBehaviour
{
    private BaseStageObject _defaultBaseStageObject;
    private StageObjectType _defaultStageObjectType;
    private BaseStageObject _nowStageObject;

    public StageObjectType _nowStageObjectType { get; private set; } = StageObjectType.None;

    public int _stageCreateAnimationIndex { get; private set; }

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
    private const float _height = 8.0f;
    
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
        //高さを調整
        var position = transform.position;
        position = new Vector3(position.x, _height,position.z);
        transform.position = position;

        //_stageCreateAnimationIndexの秒数待機
        await UniTask.Delay(TimeSpan.FromSeconds(_stageCreateAnimationIndex / _startSpeed));

        //移動
        //移動距離,移動時間
        transform.DOMoveY(0, _moveSpeed).SetEase(Ease.InQuint);

    }

    public void Setup(Vector2 position, int stageCreateAnimationIndex, StageObjectType stageObjectType)
    {
        Position = position;
        _stageCreateAnimationIndex = stageCreateAnimationIndex;
        _defaultStageObjectType = stageObjectType;
        _nowStageObjectType = stageObjectType;

        //生成したStageObjectを変数に保持
        var baseStageObject = GenerateStageObject(stageObjectType);
        _nowStageObject = baseStageObject;
    }


    /// <summary>
    /// 引数のMagicTypeを参照してこのStageObjectに与える影響を定義する関数
    /// </summary>
    public void HitMagic(MagicType type, Vector2 direction)
    {
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

        //通常の変更
        await UniTask.WhenAll(_nowStageObject.EndAnimation(),
            newBaseStageObject.InitAnimation());
        //アニメーションが終わったら

        //古いオブジェクトの削除と新しいオブジェクトの更新
        //TODO:オブジェクトプールに置き換え
        Destroy(_nowStageObject.gameObject);
        _nowStageObject = newBaseStageObject;
        isReplaceBaseStageObject = false;
    }


    /// <summary>
    /// ターン終了時に呼ぶ処理
    /// </summary>
    public async UniTask EndTurn()
    {
        await _nowStageObject.EndTurn();
    }
}