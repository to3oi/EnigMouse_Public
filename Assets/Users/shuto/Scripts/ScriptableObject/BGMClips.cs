using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BGMClips", menuName = "Data/BGMClips")]
public class BGMClips : ScriptableObject
{
    public List<BGMClip> bgmClip = new List<BGMClip>();
}

[System.Serializable]
public class BGMClip
{
    public BGMType BGMType;
    public AudioClip clip;
}