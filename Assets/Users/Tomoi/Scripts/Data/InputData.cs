using UnityEngine;
public struct InputData
{
    public MagicType MagicType { get; }
    public Vector2 Pos { get;}

    public InputData(MagicType magicType, Vector2 pos)
    {
        MagicType = magicType;
        Pos = pos;
    }
}