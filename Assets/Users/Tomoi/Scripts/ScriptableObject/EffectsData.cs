using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// エフェクトのタイプとデータを紐付けるScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "EffectsData", menuName = "Data/EffectsData")]
public class EffectsData : ScriptableObject
{
    public const string PATH = "EffectsData";

    private static EffectsData _instance;

    public static EffectsData Instance
    {
        get
        {
            //初アクセス時にロードする
            if (_instance == null)
            {
                _instance = Resources.Load<EffectsData>(PATH);


                //ロード出来なかった場合はエラーログを表示
                if (_instance == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _instance;
        }
    }

    public List<EffectTypeInfo> EffectTypeInfoList = new List<EffectTypeInfo>();
}


[System.Serializable]
public class EffectTypeInfo
{
    public EffectType MagicType;
    public BaseEffect Effect;
}
