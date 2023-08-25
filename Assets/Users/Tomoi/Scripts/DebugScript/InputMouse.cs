using System;
using UnityEngine;

/// <summary>
/// デバッグ用のスクリプト
/// マウスの入力をKinectの入力のかわりに使用するためのスクリプト
/// </summary>
public class InputMouse : IInputDataHandler
{
    private MagicType _magicType = MagicType.Fire;
    private InputData[] _inputArray = new InputData[] { new InputData() };
    private bool isMagic = false;

    /// <summary>
    /// マウスの座標からInputDataを更新
    /// </summary>
    /// <returns></returns>
    public override InputData[] UpdateInputArrays()
    {
        //配列をクリア
        Array.Clear(_inputArray, 0, _inputArray.Length);
        var v3 = Input.mousePosition;
        var offsetX = (1920 - (925)) / 2;
        var v2 = new Vector2((v3.x - offsetX) * 0.7f, v3.y / 2 * 1.1f);
        
        isMagic = Input.GetMouseButton(0);
        _inputArray = isMagic ? new InputData[] { new InputData(_magicType, v2) } : new InputData[] {};
        return _inputArray;
    }

    public override void Init()
    {
    }
}