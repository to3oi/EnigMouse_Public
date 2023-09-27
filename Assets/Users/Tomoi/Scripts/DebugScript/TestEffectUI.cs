using UnityEngine;

public class TestEffectUI : MonoBehaviour
{
    private BaseEffect _baseEffect;
    
    [ContextMenu("SpawnFireUI")]
    private void SpawnFireUI()
    {
        _baseEffect = EffectManager.Instance.PlayEffect(EffectType.Fire_UI, Vector3.zero, Quaternion.identity);
    }

    [ContextMenu("HideFireUI")]
    private void HideFireUI()
    {
        _baseEffect.Stop();
    } 
}