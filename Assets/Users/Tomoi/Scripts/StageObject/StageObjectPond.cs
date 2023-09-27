using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using UniRx;

public class StageObjectPond : BaseStageObject
{
    /// <summary>
    /// 現在このマスが凍っているかどうか
    /// </summary>
    private bool isFrozen = false;
    [SerializeField] private GameObject waterGameObject;
    private Material waterMaterial;
    private float IceRatio = 0;
    [SerializeField] private float frozenTime = 1f;
    
    public StageObjectPond(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
    }

    private void Start()
    {
        waterMaterial = waterGameObject.GetComponent<MeshRenderer>().material;
        
        waterMaterial.SetFloat("_UVTime", Random.Range(0f,10f));
    }

    /// <summary>
    /// 引数のMagicTypeを参照してこのStageObjectに与える影響を定義する関数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="direction"></param>
    /// <param name="newBaseStageObject"></param>
    /// <returns>今のStageObjectを変更するかどうか</returns>
    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        stageObjectType = StageObjectType.None;

        //魔法が氷のときのみ凍る
        if (type == MagicType.Ice)
        {
            isFrozen = true;
            Frozen().Forget();
            SoundManager.Instance.PlaySE(SEType.Frozen);
        }
        else if(type == MagicType.Fire && isFrozen)
        {
            isFrozen = false;
            Melt().Forget();
        }
        //pondは凍ることはあるが別のオブジェクトに変更されることはないのでfalse
        return false;
    }

    /// <summary>
    /// このマスが凍る処理
    /// </summary>
    private async UniTask Frozen()
    {
        var time = 0f;
        while (time <= frozenTime)
        {
            time += Time.deltaTime;
            IceRatio = time / frozenTime;
            await UniTask.Yield();
        }
        IceRatio = 1;
        await UniTask.Yield();
    }
    
    /// <summary>
    /// このマスを溶かす処理
    /// </summary>
    private async UniTask Melt()
    {
        var time = 0f;
        while (time <= frozenTime)
        {
            time += Time.deltaTime;
            IceRatio = 1 - time / frozenTime;
            await UniTask.Yield();
        }
        IceRatio = 0;
        await UniTask.Yield();
    }

    private void Update()
    {
        //IceRatioが1以上ではないときUVスクロールして水の模様を動かす
        if (IceRatio <= 0.99f)
        {
            waterMaterial.SetFloat("_UVTime", waterMaterial.GetFloat("_UVTime") + Time.deltaTime);
        }
        waterMaterial.SetFloat("_IceRatio",IceRatio );
    }

    /// <summary>
    /// ネズミが移動可能なマスか判定して返す
    /// </summary>
    /// <returns></returns>
    public override bool isValidMove()
    {
        //凍っているときのみネズミはこのマスを通れる
        return true;
    }

    public override bool isMovedDeath()
    {
        return !isFrozen;
    }
    /// <summary>
    /// 
    /// </summary>
    public override void Reset()
    {
        isFrozen = false;
        IceRatio = 0;
        waterMaterial.SetFloat("_UVTime", Random.Range(0f,10f));
    }
    public override async UniTask MoveToCell()
    {
        await Mouse.Instance.Death();
    }
}
