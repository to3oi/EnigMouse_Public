using UniRx.Toolkit;
using UnityEngine;
/// <summary>
/// エフェクトのオブジェクトプールを生成するクラス
/// </summary>
public class EffectPool : ObjectPool<BaseEffect>
{
    private readonly BaseEffect _baseEffect;
    private readonly Transform _parenTransform;
    private readonly EffectType _effectType;
    
    public EffectPool(BaseEffect baseEffect,Transform parenTransform,EffectType effectType)
    {
        _baseEffect = baseEffect;
        _parenTransform = parenTransform;
        _effectType = effectType;
        //_baseEffect.EffectType = effectType;
    }
    /// <summary>
    /// エフェクトのプレファブの追加生成時に実行される
    /// </summary>
    /// <returns></returns>
    protected override BaseEffect CreateInstance()
    {
        var be = Object.Instantiate(_baseEffect,_parenTransform, true);
        be.EffectType = _effectType;
        return be;
    }
}
