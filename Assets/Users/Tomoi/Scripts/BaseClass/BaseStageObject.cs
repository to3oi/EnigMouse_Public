using Cysharp.Threading.Tasks;
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
    /// 最初に設定する地面のテクスチャー
    /// </summary>
    public Texture2D DefaultGroundTexture;

    public BaseStageObject(Vector2 position,int stageCreateAnimationIndex)
    {
        Position = position;
        _stageCreateAnimationIndex = stageCreateAnimationIndex;
    }

    #region Animations
    /// <summary>
    /// ステージ生成時に再生するアニメーションの関数
    /// これは上から落ちてくるアニメーションを含む
    /// </summary>
    public void InitStageAnimation()
    {
        //TODO:ステージ生成時に上から落ちてくるアニメーションを作る
        InitAnimation();
    }

    /// <summary>
    /// ステージ生成時やオブジェクト生成時に再生するアニメーションの関数
    /// </summary>
    public virtual async UniTask InitAnimation()
    {
        
    }
    
    
    /// <summary>
    /// オブジェクトの削除時に再生するアニメーションの関数
    /// </summary>
    public virtual async UniTask EndAnimation()
    {
        
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
    /// 
    /// </summary>
    public virtual void Reset()
    {
        
    }
}