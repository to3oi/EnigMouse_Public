using UnityEngine;

/// <summary>
/// パーティクルの終了時にInputManagerのResetInputMagicDataDictionaryを呼び出す
/// </summary>
public class PlayerMagicCallback : MonoBehaviour
{
    [SerializeField] private MagicType magicType;
    
    private void OnParticleSystemStopped()
    {
        InputManager.Instance.ResetMagicData(magicType);
    }
}
