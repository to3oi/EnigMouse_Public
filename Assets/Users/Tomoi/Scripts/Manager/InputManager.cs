using System.Collections.Generic;
using UnityEngine;

public class MagicData
{
    /// <summary>
    /// 前のフレームの座標
    /// </summary>
    public Vector2 LastFramePosition = Vector2.zero;

    /// <summary>
    /// 停止を開始した位置
    /// </summary>
    public Vector2 StopPosition = Vector2.zero;

    /// <summary>
    /// 現在停止しているか
    /// </summary>
    public bool isStop = false;

    /// <summary>
    /// 停止を開始した時間
    /// </summary>
    public float StopStartTime = 0;

    /// <summary>
    /// Initを実行しているか
    /// </summary>
    public bool isInited = false;

    /// <summary>
    /// 魔法の発動状態
    /// </summary>
    public bool isMagicActive = false;

    /// <summary>
    /// 魔法発動直前の座標
    /// </summary>
    public Vector2 LastPosMagicActive = Vector2.zero;

    /// <summary>
    /// 魔法発動済みか
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
    /// 魔法陣作成までの待機時間
    /// </summary>
    [SerializeField] private float _delayInitMagicCircle = 0.3f;

    /// <summary>
    /// 魔法発動時に移動させる距離
    /// </summary>
    [SerializeField] private float _magicActivationRange = 5f;

    /// <summary>
    /// デバッグ用
    /// </summary>
    private IInputDataHandler _inputMouse = new InputMouse();

    private InputReceiveData _inputReceiveData = new InputReceiveData();

    /// <summary>
    /// trueのとき入力をマウスで行う
    /// </summary>
    [SerializeField] private bool isDebugInput =
# if UNITY_EDITOR
        true;
# else
    false;
#endif
    private bool isUpdateInputData = false;


    private InputData[] _inputDatas;

    private Dictionary<MagicType, MagicData>
        MagicDataDictionary = new Dictionary<MagicType, MagicData>();

    /// <summary>
    /// checkMagicType今のフレームにMagicTypeごとのInputが存在するか保持する変数
    /// </summary>
    private bool[] checkMagicType;

    private void ResetMagicDataDictionary()
    {
        MagicDataDictionary.Clear();
        MagicDataDictionary =
            new Dictionary<MagicType, MagicData>()
            {
                { MagicType.NoneMagic, new MagicData() },
                { MagicType.Fire, new MagicData() },
                { MagicType.Water, new MagicData() },
                { MagicType.Ice, new MagicData() },
                { MagicType.Wind, new MagicData() }
            };
    }

    protected override void Awake()
    {
        base.Awake();
        //初期化
        checkMagicType = new[] { false, false, false, false, false };
        _inputReceiveData.Init();
        ResetMagicDataDictionary();
    }


    private void Update()
    {
        if (isDebugInput)
        {
            //入力データの取得
            //デバッグ用にUpdateでマウスの座標を処理
            if (isUpdateInputData)
            {
                UpdateInputDataArrays(_inputMouse);
                isUpdateInputData = false;
            }
            else
            {
                isUpdateInputData = true;
            }
        }
        else
        {
            //データを受信した次のフレームでUpdateInputDataArraysを実行する
            if (_inputReceiveData.isReceived)
            {
                _inputReceiveData.ResetIsReceived();
                UpdateInputDataArrays(_inputReceiveData);
            }
        }
    }

    /// <summary>
    /// InputDataArraysを更新する
    /// 別のPCからの通信で座標を受信した際にこの関数を呼び出せるようにpublic
    /// </summary>
    public void UpdateInputDataArrays(IInputDataHandler inputDataHandler)
    {
        //入力される座標を取得
        //入力されるMagicTypeの種類は一意である
        //ただし、入力される順番とすべての魔法が入力されているかは保証されない
        _inputDatas = inputDataHandler.UpdateInputArrays();

        /*//初期化
        for (int index = 0; index < checkMagicType.Length; index++)
        {
            checkMagicType[index] = false;
        }*/

        for (int inputIndex = 0; inputIndex < _inputDatas.Length; inputIndex++)
        {
            //コードが長くなるので変数で保持
            var inputData = _inputDatas[inputIndex];
            var magicData = MagicDataDictionary[inputData.MagicType];

            checkMagicType[(int)inputData.MagicType] = true;

            //静止判定
            //isInitedがTrueのときは距離の判定をLastFramePositionではなくStopPositionで行う

            if (Vector2.Distance(magicData.isInited ? magicData.StopPosition : magicData.LastFramePosition,
                    _inputDatas[inputIndex].Pos) <= _staticCapacity)
            {
                //停止から少し時間を待って魔法陣を展開開始する
                //StopStartTimeのデフォルトの値0は無視する
                //一度だけこのネストの中を実行する
                if (magicData.StopStartTime != 0
                    && magicData.StopStartTime + _delayInitMagicCircle <= Time.time
                    && !magicData.isInited)
                {
                    magicData.StopPosition = inputData.Pos;
                    magicData.isInited = true;
                    GameManager.Instance.Magic_StartCoordinatePause(inputData.Pos, inputData.MagicType);
                }

                //魔法陣を作成してから_staticDurationの秒数立ったら魔法を発動可能にする
                if (magicData.StopStartTime != 0
                    && magicData.StopStartTime + _delayInitMagicCircle + _staticDuration <= Time.time
                    && magicData.isInited
                    && !magicData.isMagicActive)
                {
                    magicData.isMagicActive = true;
                    magicData.LastPosMagicActive = inputData.Pos;
                }

                //魔法の発動
                if (magicData.isMagicActive)
                {
                    //魔法発動中になってから一定の距離移動したら
                    if (_magicActiveCapacity <=
                        Vector2.Distance(magicData.LastPosMagicActive, inputData.Pos))
                    {
                        //魔法発動方向のベクトルを計算
                        var v = inputData.Pos - magicData.LastPosMagicActive;
                        //このあと魔法陣を動かすことができないので多少動かす
                        GameManager.Instance.Magic_PauseCompleted(inputData.Pos,v, inputData.MagicType);

                        //このboolがtrueのときは魔法を再度発動できない
                        //魔法のパーティクルが削除されるときにリセットする処理が走る
                        magicData.isUsedMagic = true;
                    }
                }


                //魔法陣発動済みの場合魔法陣の移動をする
                if (magicData.isInited)
                {
                    GameManager.Instance.Magic_CoordinatePaused(inputData.Pos, inputData.MagicType);
                }

                //停止開始時の時間を保持
                if (!magicData.isStop)
                {
                    magicData.StopStartTime = Time.time;
                    magicData.isStop = true;
                }
            }
            //静止していない or 静止画解除されたら
            else
            {
                //魔法が発動済みでなければ初期化
                if (!magicData.isUsedMagic)
                {
                    ResetMagicData(inputData.MagicType);
                }
            }

            //停止位置を更新
            //次のフレームの判定に使うのでLastFramePositionのみ更新
            magicData.LastFramePosition = inputData.Pos;
        }

        //最終チェック
        for (int index = 1; index < 5; index++)
        {
            //入力されていな尚且つ魔法が発動済みでなければ初期化
            if (!checkMagicType[index] && !MagicDataDictionary[(MagicType)index].isUsedMagic)
            {
                ResetMagicData((MagicType)index);
            }
            //trueの場合次のフレームで使用するので初期化
            else
            {
                checkMagicType[index] = false;
            }
        }
    }

    /// <summary>
    /// 魔法が発動可能かを計算している変数を初期化する
    /// </summary>
    /// <param name="magicType"></param>
    public void ResetMagicData(MagicType magicType)
    {
        //魔法を発動中ならリリースする ただし、魔法発動済みなら無視する
        if (MagicDataDictionary[magicType].isInited && !MagicDataDictionary[magicType].isUsedMagic)
        {
            GameManager.Instance.Magic_CancelCoordinatePause(MagicDataDictionary[magicType].LastFramePosition,
                magicType);
        }

        MagicDataDictionary[magicType].LastFramePosition = Vector2.zero;
        MagicDataDictionary[magicType].StopPosition = Vector2.zero;
        MagicDataDictionary[magicType].isStop = false;
        MagicDataDictionary[magicType].StopStartTime = 0;
        MagicDataDictionary[magicType].isInited = false;
        MagicDataDictionary[magicType].isMagicActive = false;
        MagicDataDictionary[magicType].LastPosMagicActive = Vector2.zero;
        MagicDataDictionary[magicType].isUsedMagic = false;
    }
}