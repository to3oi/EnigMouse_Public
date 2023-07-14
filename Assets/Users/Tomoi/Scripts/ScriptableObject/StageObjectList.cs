using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageObjects", menuName = "Data/StageObjects")]
public class StageObjectList : ScriptableObject
{
    public List<StageObject> List = new List<StageObject>();

    public const string PATH = "StageObjects";
    private static StageObjectList _instance;

    public static StageObjectList Instance
    {
        get
        {
            //初アクセス時にロードする
            if (_instance == null)
            {
                _instance = Resources.Load<StageObjectList>(PATH);


                //ロード出来なかった場合はエラーログを表示
                if (_instance == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// 引数で指定したStageObjectTypeのオブジェクトを返す
    /// 設定されてない場合nullを返す
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public GameObject GetGameObject(StageObjectType _type)
    {
        foreach (var stageObject in List)
        {
            if (stageObject.ObjectType == _type)
            {
                return stageObject.GameObject;
            }
        }
        return null;
    }
}

[System.Serializable]
public class StageObject
{
    public StageObjectType ObjectType = StageObjectType.None;
    public GameObject GameObject = null;
}