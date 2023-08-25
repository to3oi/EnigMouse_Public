using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    private EffectsData _effectsData;
    private List<EffectPool> _effectPoolList = new List<EffectPool>();

    private Dictionary<EffectType, (Transform parent, EffectPool pool)> _effectDictionary =
        new Dictionary<EffectType, (Transform parent, EffectPool pool)>();

    private void Start()
    {
        _effectsData = EffectsData.Instance;
        //ObjectPoolの生成
        foreach (var effectTypeInfo in _effectsData.EffectTypeInfoList)
        {
            var parentObj = new GameObject($"Effect_{effectTypeInfo.MagicType.ToString()}").transform;
            var pool = new EffectPool(effectTypeInfo.Effect, parentObj, effectTypeInfo.MagicType);
            _effectDictionary.Add(effectTypeInfo.MagicType, (parentObj, pool));
        }

        
        
        //シーンをアロード時にpoolを削除する処理
        this.OnDestroyAsObservable().Subscribe(_ =>
        {
            foreach (var key in _effectDictionary)
            {
                if (key.Value.pool == null) throw new ArgumentNullException(nameof(key.Value.pool));
                key.Value.pool.Dispose();
            }
        }).AddTo(this);
    }

    public BaseEffect PlayEffect(EffectType effectType,Vector3 position,Quaternion rotation)
    {
        var baseEffect = _effectDictionary[effectType].pool.Rent();
        
        baseEffect.Play(position,rotation);
        return baseEffect;
    }  
    public BaseEffect PlayEffect(EffectType effectType,Vector3 position,Quaternion rotation,Transform parent)
    {
        var baseEffect = _effectDictionary[effectType].pool.Rent();
        
        baseEffect.Play(position,rotation,parent);
        return baseEffect;
    }
    
    /// <summary>
    /// 再生が終了したエフェクトをプールに戻す関数
    /// </summary>
    /// <param name="effectType"></param>
    /// <param name="particlePlayer"></param>
    public void ReturnPool(EffectType effectType,BaseEffect baseEffect)
    {
        //パーティクルの親オブジェクトの変更
        baseEffect.transform.parent = _effectDictionary[effectType].parent;
        _effectDictionary[effectType].pool.Return(baseEffect);
    }
}