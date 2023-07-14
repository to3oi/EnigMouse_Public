using Unity.Mathematics;
using UnityEngine;

public class TestEffectManager : MonoBehaviour
{
    [SerializeField] private MagicType _magicType = MagicType.NonMagic;

    private void OnGUI()
    {
        if (GUILayout.Button("エフェクトテスト"))
        {
            PlayEffect();
        }
    }
    [ContextMenu("PlayEffect")]
    private void PlayEffect()
    {
        EffectManager.Instance.PlayEffect((EffectType)_magicType,Vector3.zero,quaternion.identity);
    }
}