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
    
    /// <summary>
    /// 前のフレームまでに受信したデータが存在するときにture
    /// </summary>
    public bool isReceived { get;private set; }= false;

    public void ResetIsReceived()
    {
        isReceived = false;
    }
    void OnReceive(byte[] bytes)
    {
        //Debug.Log("OnReceive");

        if (mUDPReceiver == null)
        {
            return;
        }
        isReceived = true;
        //Debug.Log("デシリアライズ");
        deserializedList.Clear();
        // デシリアライズ
        deserializedList = MessagePackSerializer.Deserialize<List<ReceiveData>>(bytes);
    }
    List<InputData> oInputData = new List<InputData>();

    public override InputData[] UpdateInputArrays()
    {
        //初期化
        ResetMagicList();
        oInputData.Clear();

        //信頼度でソート
        deserializedList.Sort((a, b) => a.Confidence.CompareTo(b));
        foreach (var receiveData in deserializedList)
        {
            var magicType = convertLabel2MagicType(receiveData.Label);

            //Confidenceの値が現在のreceiveDataが上回っていたら更新
            if (MagicList[(int)magicType].Item2.Confidence <= receiveData.Confidence)
            {
                MagicList[(int)magicType] = (MagicList[(int)magicType].Item1, receiveData);
            }
        }
        
        for (var i = 1; i < MagicList.Count; i++)
        {
            //物体検出の結果の信頼度が50%以上のときInputデータとして扱う
            if (0.5f <= MagicList[i].Item2.Confidence)
            {
                InputData input = new InputData(convertLabel2MagicType(MagicList[i].Item2.Label),
                    new Vector2(MagicList[i].Item2.PosX, MagicList[i].Item2.PosY));
                oInputData.Add(input);
            }
        }

        return oInputData.ToArray();
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
        ResetMagicList();
    }

    /// <summary>
    /// 魔法の種類ごとのReceiveDataの結果
    /// </summary>
    private List<(MagicType, ReceiveData)> MagicList = new List<(MagicType, ReceiveData)>();

    private void ResetMagicList()
    {
        MagicList.Clear();
        MagicList = new List<(MagicType, ReceiveData)>()
        {
            (MagicType.NoneMagic, new ReceiveData("", 0, 0, 0)),
            (MagicType.Fire, new ReceiveData("", 0, 0, 0)),
            (MagicType.Water, new ReceiveData("", 0, 0, 0)),
            (MagicType.Ice, new ReceiveData("", 0, 0, 0)),
            (MagicType.Wind, new ReceiveData("", 0, 0, 0))
        };
    }


    private MagicType convertLabel2MagicType(string label)
    {
        return label switch
        {
            "Cross" => MagicType.Fire,
            "Othello" => MagicType.Water,
            "Ring" => MagicType.Ice,
            "Square" => MagicType.Wind,
            _ => MagicType.NoneMagic
        };
    }
}