using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// ステージの情報を管理、更新するマネージャー
/// </summary>
public class StageManager : SingletonMonoBehaviour<StageManager>
{
    /// <summary>
    /// 現在のマップの情報
    /// 配列の構造は以下
    ///  [y,x]
    ///     → x
    ///  ↓ [0,0] [0,1]...
    ///  y [1,0] ...
    ///    
    ///    
    ///                           [4,5]
    ///                     [5,4],[5,5]
    /// </summary>
    [SerializeField]
    public Map.Map currentMap { get; private set; }

    public Map.Map defoultMap { get; private set; }

    /// <summary>
    /// 取得時はy,x
    /// </summary>
    public List<List<DynamicStageObject>> DynamicStageObjectList = new List<List<DynamicStageObject>>();

    /// <summary>
    /// ステージのオブジェクトをまとめるためのルートオブジェクト
    /// </summary>
    private GameObject stageRoot;

    [SerializeField] public static float offset = 1;

    /// <summary>
    /// ステージを生成するときのアニメーションに使用する固定の順番を表した番号
    /// </summary>
    private readonly int[][] stageCreateAnimationIndex = new[]
    {
        new[] { 15, 16, 17, 18, 19, 0 },
        new[] { 14, 29, 30, 31, 20, 1 },
        new[] { 13, 28, 35, 32, 21, 2 },
        new[] { 12, 27, 34, 33, 22, 3 },
        new[] { 11, 26, 25, 24, 23, 4 },
        new[] { 10, 9, 8, 7, 6, 5 }
    };

    public BaseFrameOutline FrameOutlinePrefab
    {
        get { return _frameOutline; }
    }
    [SerializeField] private BaseFrameOutline _frameOutline;
    protected override void Awake()
    {
        base.Awake();

        stageRoot = new GameObject("StageRoot");

        loadStageMap();
    }

    /// <summary>
    /// StageMapsに設定されているマップのインデックスをランダムで返す
    /// </summary>
    /// <returns></returns>
    int StageRandomSelect()
    {
        return Random.Range(0, StageMaps.Instance.StageMapList.Count - 1);
    }

    /// <summary>
    /// マップの初期化
    /// </summary>
    [ContextMenu("ResetMap")]
    private void ResetMap()
    {
        foreach (Transform n in stageRoot.transform)
        {
            Destroy(n.gameObject);
        }

        //初期化
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                currentMap.y[y].x[x] = defoultMap.y[y].x[x];
            }
        }
    }

    /// <summary>
    /// ステージの生成
    /// </summary>
    [ContextMenu("CreateStage")]
    public async UniTask CreateStage()
    {
        //DynamicStageObjectListの初期化
        DynamicStageObjectList = new List<List<DynamicStageObject>>(6);
        for (int y = 0; y < 6; y++)
        {
            DynamicStageObjectList.Add(new List<DynamicStageObject>());
            for (int x = 0; x < 6; x++)
            {
                DynamicStageObjectList[y].Add(null);
            }
        }

        /* 生成時の座標系
         *  [y,x]
         *     → x
         *  ↓ [0,0] [0,1]...
         *  y [1,0] ...
         *    
         *    
         *                           [4,5]
         *                     [5,4],[5,5]
         */

        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                //Mathf.Abs(y - 6)で座標を逆転させている
                //var gameObject = new GameObject();
                //CubeのColliderが必要なのでCubeを生成してMeshRendererを削除
                var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gameObject.transform.position = new Vector3(x * offset, 0, Mathf.Abs(y - 6) * offset);
                gameObject.layer = 7;
                Destroy(gameObject.GetComponent<MeshRenderer>());
                //デバッグ用
                gameObject.name = $"[{y},{x}]";
                gameObject.transform.parent = stageRoot.transform;

                var dynamicStageObject = gameObject.AddComponent<DynamicStageObject>();
                dynamicStageObject.Setup(new Vector2(x, y), stageCreateAnimationIndex[y][x], currentMap.y[y].x[x]);
                DynamicStageObjectList[y][x] = dynamicStageObject;
            }
        }
        //InitAnimation
        List<UniTask> task = new List<UniTask>();
        foreach (var listY in DynamicStageObjectList)
        {
            foreach (var dynamicStageObject in listY)
            {
                task.Add(dynamicStageObject.InitStageAnimation());
            }
        }

        //すべてのオブジェクトが落ちるのを待つ
        await UniTask.WhenAll(task);

        foreach (var listY in DynamicStageObjectList)
        {
            foreach (var dynamicStageObject in listY)
            {
                dynamicStageObject.InitAnimation().Forget();
            }
        }
    }


    /// <summary>
    /// ステージを再生成する
    /// </summary>
    [ContextMenu("RegenerationStage")]
    public async UniTask RegenerationStage()
    {
        var destroyStageObjectList = DynamicStageObjectList;

        foreach (var listY in destroyStageObjectList)
        {
            foreach (var dynamicStageObject in listY)
            {
                dynamicStageObject.ExitStageAnimation().Forget();
            }
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        await CreateStage();
    } 

    /// <summary>
    /// 盤面の座標x,yからUnityのワールド空間上の座標に変換する
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * offset, 0, Mathf.Abs(y - 6) * offset);
    }
    /// <summary>
    /// 盤面の座標x,yからUnityのワールド空間上の座標に変換する
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector3 GetWorldPosition(Vector2 pos)
    {
        return new Vector3(pos.x * offset, 0, Mathf.Abs(pos.y - 6) * offset);
    }

    /// <summary>
    /// ステージ上に存在するDynamicStageObjectを引数x,yで取得する
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public DynamicStageObject GetDynamicStageObject(int x, int y)
    {
        return DynamicStageObjectList[y][x];
    }

    [ContextMenu("reset")]
    public void Reset()
    {
        foreach (var list in DynamicStageObjectList)
        {
            foreach (var dynamicStageObject in list)
            {
                dynamicStageObject.Reset();
            }
        }
    }

    /// <summary>
    /// マップデータの読み込み
    /// </summary>
    private void loadStageMap()
    {
        var mapIndex = StageRandomSelect();
        defoultMap = new Map.Map(StageMaps.Instance.StageMapList[mapIndex].y).DeepCopy();
        currentMap = new Map.Map(StageMaps.Instance.StageMapList[mapIndex].y).DeepCopy();
    }

    /// <summary>
    /// 引数x,yでStageObjectTypeをUnityの座標空間上と同じ軸で取得できる
    ///
    ///                     [4,5],[5,5]
    ///                           [5,4]
    ///    
    ///    
    ///  ↑ [0,1] ...
    ///  y [0,0] [1,0]...
    ///     → x
    ///  [x,y]
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public StageObjectType? GetStageObjectType(int x, int y)
    {
        //値がマップの範囲内か判定
        if (!(0 <= x && x <= 5 && 0 <= y && y <= 5))
        {
            //範囲外ならnullを返す
            return null;
        }

        return GetDynamicStageObject(x, Mathf.Abs(y - 5)).NowStageObjectType;
    }
}