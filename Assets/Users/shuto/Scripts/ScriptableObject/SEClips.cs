using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SEClips",menuName = "Data/SEClips")]
public class SEClips : ScriptableObject
{
    public List<SEClip> seClip = new List<SEClip>();
}

[System.Serializable]
public class SEClip
{
    public SEType SEType;
    public AudioClip clip;
}