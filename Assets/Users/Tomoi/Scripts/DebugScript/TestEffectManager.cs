using Unity.Mathematics;
using UnityEngine;

public class TestEffectManager : MonoBehaviour
{
    [SerializeField] private MagicType _magicType = MagicType.NoneMagic;
    [SerializeField] private EffectType _effectType = EffectType.Magic_None;

    private void OnGUI()
    {
        if (GUILayout.Button("エフェクトテスト"))
        {
            PlayEffect();
        }
        
        if (GUILayout.Button("エフェクトテスト2"))
        {
            PlayEffect2();
        }
    }
    private void PlayEffect()
    {
        EffectManager.Instance.PlayEffect((EffectType)_magicType,Vector3.zero,quaternion.identity);
    } 
    private void PlayEffect2()
    {
        EffectManager.Instance.PlayEffect(_effectType,Vector3.zero,quaternion.identity);
    }
}