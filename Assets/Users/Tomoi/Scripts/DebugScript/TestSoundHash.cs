using UnityEngine;

public class TestSoundHash : MonoBehaviour
{
    void Start()
    {
        var soundHash = new SoundHash();
        var soundHash2 = new SoundHash();
     
        Debug.Log($"soundHash == soundHash is {soundHash.SoundHashID == soundHash.SoundHashID}");
        Debug.Log($"soundHash == soundHash2 is {soundHash.SoundHashID == soundHash2.SoundHashID}");
    }

}
