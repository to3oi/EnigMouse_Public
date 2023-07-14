using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageMaps", menuName = "Data/StageMaps")]
public class StageMaps : ScriptableObject
{
    public List<Map.Map> StageMapList
    {
        get { return _stageMapList; }
        private set { }
    }

    /// <summary>
    /// StageMapEditorで参照するList
    /// </summary>
    public List<Map.Map> _stageMapList = new List<Map.Map>();

    public const string PATH = "StageMaps";
    private static StageMaps _instance;

    public static StageMaps Instance
    {
        get
        {
            //初アクセス時にロードする
            if (_instance == null)
            {
                _instance = Resources.Load<StageMaps>(PATH);

                //ロード出来なかった場合はエラーログを表示
                if (_instance == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _instance;
        }
    }
}

namespace Map
{
    [System.Serializable]
    public class Map
    {
        public List<X> y = new List<X>(6);

        public Map(List<X> list)
        {
            y = list;
        }
    }

    [System.Serializable]
    public class X
    {
        public List<StageObjectType> x = new List<StageObjectType>(6);
    }
}