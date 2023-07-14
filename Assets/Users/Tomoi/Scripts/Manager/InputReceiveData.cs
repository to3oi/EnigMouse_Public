using System;
using System.Collections.Generic;
using MessagePack;
using UnityEasyNet;
using UnityEngine;

/// <summary>
/// 外部からの入力を変換してInputDataにして渡すクラス
/// </summary>
public class InputReceiveData : IInputDataHandler
{
    private UDPReceiver mUDPReceiver = null;
    
    private List<ReceiveData> deserializedList = new List<ReceiveData>();
    void OnReceive(byte[] bytes)
    {
        //Debug.Log("OnReceive");

        if (mUDPReceiver == null)
        {
            return;
        }
        //Debug.Log("デシリアライズ");
        deserializedList.Clear();
        // デシリアライズ
        deserializedList  = MessagePackSerializer.Deserialize<List<ReceiveData>>(bytes);
    }

    private InputData[] _inputArray = new InputData[]{new InputData()};

    public override InputData[] UpdateInputArrays()
    {
        //配列をクリア
        Array.Clear(_inputArray, 0, _inputArray.Length);

        _inputArray = new InputData[deserializedList.Count];

        for (int i = 0; i < deserializedList.Count; i++)
        {
            _inputArray[i] = new InputData(convertLabel2MagicType(deserializedList[i].Label),
                new Vector2(deserializedList[i].PosX, deserializedList[i].PosY));
        }
        return _inputArray;
    }

    public override void Init()
    {
        // シリアライザの初期設定
        MessagePack.Resolvers.StaticCompositeResolver.Instance.Register(
            MessagePack.Resolvers.GeneratedResolver.Instance, // コード生成した型解決クラス
            MessagePack.Unity.UnityResolver.Instance,
            MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
            MessagePack.Resolvers.StandardResolver.Instance
        );
        var option = MessagePack.MessagePackSerializerOptions.Standard
            .WithCompression(MessagePack.MessagePackCompression.Lz4BlockArray) // LZ4 圧縮利用
            .WithResolver(MessagePack.Resolvers.StaticCompositeResolver.Instance);
        MessagePack.MessagePackSerializer.DefaultOptions = option;
        
        mUDPReceiver = new UDPReceiver(12001, OnReceive);
    }

    private MagicType convertLabel2MagicType(string label)
    {
        //TODO:ラベルの変換
        return MagicType.Fire;
    }
}
