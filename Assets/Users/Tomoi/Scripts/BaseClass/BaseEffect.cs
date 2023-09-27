using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BaseEffect : MonoBehaviour
{
    public ParticleSystem _effect; 
    public ParticleSystem Effect => 
        _effect ? _effect : _effect = GetComponent<ParticleSystem>();
    public EffectType EffectType { get; set; }

    public void Play(Vector3 position,Quaternion rotation)
    {
        transform.position = position;
        transform.localRotation = rotation;
        Effect.Play();
    }  
    public void Play(Vector3 position,Quaternion rotation,Transform parent)
    {
        transform.parent = parent;
        transform.localPosition = position;
        transform.localRotation = rotation;
        Effect.Play();
    }

    public void Stop()
    {
        Effect?.Stop();
    }
    
    /// <summary>
    /// エフェクト停止時の処理
    /// </summary>
    public virtual void OnParticleSystemStopped()
    {
        transform.parent = null;
        EffectManager.Instance.ReturnPool(EffectType,this);
    }
}