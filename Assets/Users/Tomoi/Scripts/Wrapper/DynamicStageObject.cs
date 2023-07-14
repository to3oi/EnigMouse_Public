using Cysharp.Threading.Tasks;
using Unity.Mathematics;
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
    /// 地面のテクスチャを変更するバージョン
    /// </summary>
    /// <param name="position"></param>
    /// <param name="stageCreateAnimationIndex"></param>
    /// <param name="stageObjectType"></param>
    /// <param name="texture2D"></param>
    public void Setup(Vector2 position, int stageCreateAnimationIndex, StageObjectType stageObjectType,
        Texture2D texture2D)
    {
        Position = position;
        _stageCreateAnimationIndex = stageCreateAnimationIndex;
        isUpdateGroundMaterial = true;

        //生成したStageObjectを変数に保持
        var baseStageObject = GenerateStageObject(stageObjectType);
        _nowStageObject = baseStageObject;
        
        //TODO:地面のテクスチャの変更
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
    /// ネズミが移動可能なマスか判定して返す
    /// </summary>
    /// <returns></returns>
    public bool isValidMove()
    {
        return _nowStageObject.isValidMove();
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
        //var g = Instantiate(StageObjectList.Instance.GetGameObject(type), Vector3.zero, quaternion.identity, transform);
        var g = Instantiate(StageObjectList.Instance.GetGameObject(type), transform,false);
        return g.GetComponent<BaseStageObject>();
    }
    
    private bool isReplaceBaseStageObject = false;

    /// <summary>
    /// BaseStageObjectを置き換える
    /// </summary>
    public async void ReplaceBaseStageObject(StageObjectType type)
    {
        if (isReplaceBaseStageObject)
        {
            return;
        }

        isReplaceBaseStageObject = true;
        var newBaseStageObject = GenerateStageObject(type);

        //IPondを継承している
        if (newBaseStageObject is IPond pond)
        {
            await UniTask.WhenAll(_nowStageObject.EndAnimation(),
                newBaseStageObject.InitAnimation(),
                GroundMaterialUpdate(pond.GetTexture2D()));
        }
        else
        {
            if (newBaseStageObject is IAbyss)
            {
                //IAbyssを継承している
            }

            //通常の変更
            await UniTask.WhenAll(_nowStageObject.EndAnimation(),
                newBaseStageObject.InitAnimation());
        }
        //アニメーションが終わったら

        //古いオブジェクトの削除と新しいオブジェクトの更新
        Destroy(_nowStageObject.gameObject);
        _nowStageObject = newBaseStageObject;
        isReplaceBaseStageObject = false;
    }

    private async UniTask GroundMaterialUpdate(Texture2D texture2D)
    {
        //TODO:マテリアルの更新をする
        /*Debug.Log("wait");
        await UniTask.Delay(3000);
        Debug.Log("complete material update");*/
    }
}