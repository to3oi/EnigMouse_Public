using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

class InputMagicData
{
    /// <summary>
    /// 前回座標を更新したときの座標
    /// </summary>
    public Vector2 lastPos = Vector2.zero;
    /// <summary>
    /// 現在静止しているか
    /// </summary>
    public bool isStop = false;
    /// <summary>
    /// 静止を開始した時間
    /// </summary>
    public float StopTime = 0f;
    /// <summary>
    /// 魔法発動中か
    /// </summary>
    public bool isMagicActive = false;
    /// <summary>
    /// 魔法発動前の座標
    /// </summary>
    public Vector2 lastPosMagicActive = Vector2.zero;
    /// <summary>
    /// 魔法を使用済みか
    /// </summary>
    public bool isUsedMagic = false;
}

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    /// <summary>
    /// 静止許容距離
    /// </summary>
    [SerializeField] private float _staticCapacity = 7.5f;
    
    /// <summary>
    /// 魔法発動時に魔法の発動方向を指定する距離
    /// </summary>
    [SerializeField] private float _magicActiveCapacity = 5.0f;

    /// <summary>
    /// 静止している時間
    /// </summary>
    [SerializeField] private float _staticDuration = 2.0f;

    /// <summary>
    /// デバッグ用
    /// </summary>
    private IInputDataHandler _inputMouse = new InputMouse();
    
    
    private IInputDataHandler _inputReceiveData = new InputReceiveData();

    private InputData[] _inputData;

    //発動する魔法の移動方向を取得するための変数
    private Dictionary<MagicType, InputMagicData> _inputMagicDataDictionary =
        new Dictionary<MagicType, InputMagicData>()
        {
            { MagicType.NonMagic, new InputMagicData() },
            { MagicType.Fire, new InputMagicData() },
            { MagicType.Water, new InputMagicData() },
            { MagicType.Ice, new InputMagicData() },
            { MagicType.Wind, new InputMagicData() }
        };

    private void ResetInputMagicDataDictionary()
    {
        _inputMagicDataDictionary.Clear();
        _inputMagicDataDictionary = new Dictionary<MagicType, InputMagicData>()
        {
            { MagicType.NonMagic, new InputMagicData() },
            { MagicType.Fire, new InputMagicData() },
            { MagicType.Water, new InputMagicData() },
            { MagicType.Ice, new InputMagicData() },
            { MagicType.Wind, new InputMagicData() }
        };
    }

    protected override void Awake()
    {
        base.Awake();
        _inputReceiveData.Init();
        ResetInputMagicDataDictionary();
    }

    private bool a = false;
    private void Update()
    {
        
        //入力データの取得
        //デバッグ用にUpdateでマウスの座標を処理
        if (a)
        {
            //UpdateInputDataArrays(_inputMouse);
            UpdateInputDataArrays(_inputReceiveData);
            a = false;
        }
        else
        {
            a = true;
        }
        //Debug.Log($"data MagicType{_inputData[0].MagicType.ToString()}, Position{_inputData[0].Pos} , Static {_inputMagicDataDictionary[_inputData[0].MagicType].isStop}");
    }

    /// <summary>
    /// InputDataArraysを更新する
    /// 別のPCからの通信で座標を受信した際にこの関数を呼び出せるようにpublic
    /// </summary>
    public void UpdateInputDataArrays(IInputDataHandler inputDataHandler)
    {
        //入力される座標を取得
        _inputData = inputDataHandler.UpdateInputArrays();

        for (int i = 0; i < _inputData.Length; i++)
        {
            //停止とみなす距離の範囲内
            if (Vector2.Distance(_inputMagicDataDictionary[_inputData[i].MagicType].lastPos, _inputData[i].Pos) <=
                _staticCapacity)
            {
                //停止してから一定時間がたち、尚且つ魔法が発動済みではない場合
                if (_inputMagicDataDictionary[_inputData[i].MagicType].StopTime + _staticDuration <=
                    Time.realtimeSinceStartup && !_inputMagicDataDictionary[_inputData[i].MagicType].isMagicActive && !_inputMagicDataDictionary[_inputData[i].MagicType].isUsedMagic)
                {
                    //魔法の発動中を設定
                    _inputMagicDataDictionary[_inputData[i].MagicType].isMagicActive = true;
                    _inputMagicDataDictionary[_inputData[i].MagicType].lastPosMagicActive = _inputData[i].Pos;
                }


                //前フレームでも座標が停止しているなら
                if (_inputMagicDataDictionary[_inputData[i].MagicType].isStop)
                {
                    // TODO:GameManagerに「座標の停止中」の処理を流す 
                }
                //前フレームで座標が停止していない場合
                else
                {
                    // TODO:GameManagerに「座標の停止開始」の処理を流す 
                    //停止開始の時間を変数に保持
                    _inputMagicDataDictionary[_inputData[i].MagicType].StopTime = Time.realtimeSinceStartup;
                }

                _inputMagicDataDictionary[_inputData[i].MagicType].isStop = true;

                //Debug.Log(_inputData[i].Pos);
                
                //1920 x 1080のディスプレイに最大値640 x 576のサイズで返される値を中心揃えで再計算する
                //元が画像データなので取得する座標軸はx↓,y→でUnityの座標軸はx↑,y→である
                //var m =(float) 1080 / 576;
                //少し余白をもたせる
                var m =(float) 1080 / (576 * 1.04f);
                var offsetX = (1920 - (640 * 1.04f)* m) / 2;
                //TODO:y軸が逆
                Vector2 pos = new Vector2(_inputData[i].Pos.x * m + offsetX, 1080 - _inputData[i].Pos.y * m);            
                GameManager.Instance.TestMagicPoint(pos, _inputData[i].MagicType);
                //GameManager.Instance.TestMagicPoint(_inputData[i].Pos, _inputData[i].MagicType);
            }
            //停止とみなす距離の範囲外
            else
            {
                //前フレームで停止していたら
                if (_inputMagicDataDictionary[_inputData[i].MagicType].isStop)
                {
                    // TODO:GameManagerに「座標の停止キャンセル」の処理を流す 
                }

                _inputMagicDataDictionary[_inputData[i].MagicType].isStop = false;
            }
            
            //魔法発動中か
            if (_inputMagicDataDictionary[_inputData[i].MagicType].isMagicActive && !_inputMagicDataDictionary[_inputData[i].MagicType].isUsedMagic)
            {
                //魔法発動中になってから一定の距離移動したら
                if (_magicActiveCapacity <= Vector2.Distance(_inputMagicDataDictionary[_inputData[i].MagicType].lastPosMagicActive,
                        _inputData[i].Pos) )
                {
                    
                    /*
                     *            1 y
                     *            |
                     *            |
                     * -1 --------|------- 1 x
                     *            |
                     *            |  
                     *            -1     
                     */
                    // TODO:GameManagerに「停止完了」の処理を流す 
                    //魔法発動中になってから座標が移動した方向を取得
                    var v = _inputData[i].Pos - _inputMagicDataDictionary[_inputData[i].MagicType].lastPosMagicActive ;
                    Debug.Log($"停止完了 {v.normalized}");
                    _inputMagicDataDictionary[_inputData[i].MagicType].isMagicActive = false;
                    _inputMagicDataDictionary[_inputData[i].MagicType].isUsedMagic = true;
                }
            }
            //1フレーム前の座標を更新
            _inputMagicDataDictionary[_inputData[i].MagicType].lastPos = _inputData[i].Pos;
        }
    }
}