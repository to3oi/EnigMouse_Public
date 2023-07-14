using System;
using System.Collections.Generic;
using MessagePack;
using UnityEasyNet;
using UnityEngine;

public class TestReceiveData : MonoBehaviour
{
    private UDPReceiver mUDPReceiver = null;
    private GameObject testGameObject;
    
    public void Awake()
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
    }
    void Start()
    {
        mUDPReceiver = new UDPReceiver(12001, OnReceive);
        testGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testGameObject.name =  "testGameObject";
        testGameObject.AddComponent<TrailRenderer>();
    }

    private List<ReceiveData> deserializedList = new List<ReceiveData>();
    void OnReceive(byte[] bytes)
    {
        Debug.Log("OnReceive");

        if (mUDPReceiver == null)
        {
         return;
        }
        Debug.Log("デシリアライズ");
        deserializedList.Clear();
        // デシリアライズ
        deserializedList  = MessagePackSerializer.Deserialize<List<ReceiveData>>(bytes);
                
        /*foreach (var result in deserializedList)
        {
            Debug.Log($"{result.Label},{result.PosX},{result.PosY},{result.Confidence}");
        }*/
    }

    private void Update()
    {
        foreach (var result in deserializedList)
        {
            //testGameObject.transform.position = new Vector3(result.PosX,0,result.PosY);
            //1920 x 1080のディスプレイに最大値640 x 576のサイズで返される値を中心揃えで再計算する
            //元が画像データなので取得する座標軸はx↓,y→でUnityの座標軸はx↑,y→である
            //var m =(float) 1080 / 576;
            //少し余白をもたせる
            var m =(float) 1080 / 600;
            var offsetX = (1920 - 640 * m) / 2;
            Vector2 pos = new Vector2(result.PosX * m + offsetX, result.PosY * m);            
            GameManager.Instance.TestMagicPoint(pos,MagicType.Fire);
        }
    }
}
