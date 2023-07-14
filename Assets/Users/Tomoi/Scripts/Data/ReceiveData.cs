using System;
using MessagePack;

[MessagePackObject]
[Serializable]
public struct ReceiveData
{
    [Key(0)] public string Label { get; set; }
    [Key(1)] public float PosX { get; set; }
    [Key(2)] public float PosY { get; set; }
    [Key(3)] public float Confidence { get; set; }


    public ReceiveData(string _Label, float _PosX, float _PosY, float _Confidence)
    {
        Label = _Label;
        PosX = _PosX;
        PosY = _PosY;
        Confidence = _Confidence;
    }
}