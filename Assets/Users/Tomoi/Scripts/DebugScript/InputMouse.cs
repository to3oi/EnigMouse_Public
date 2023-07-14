using System;
using UnityEngine;
/// <summary>
/// デバッグ用のスクリプト
/// マウスの入力をKinectの入力のかわりに使用するためのスクリプト
/// </summary>
public class InputMouse : IInputDataHandler
{
    private MagicType _magicType = MagicType.Fire;
    private InputData[] _inputArray = new InputData[]{new InputData()};

    /// <summary>
    /// マウスの座標からInputDataを更新
    /// </summary>
    /// <returns></returns>
    public override InputData[] UpdateInputArrays()
    {
        //配列をクリア
        Array.Clear(_inputArray, 0, _inputArray.Length);
        var v3 = Input.mousePosition;
        _inputArray = new InputData[]{new InputData(_magicType, v3)};
        return _inputArray;
    }

    public override void Init()
    {
        
    }
}
