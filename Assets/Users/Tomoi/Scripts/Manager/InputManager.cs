using System;
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

public class InputManager : SingletonMonoBehaviour4Manager<InputManager>
{
    /// <summary>
    /// 静止許容距離
    /// </summary>
    [SerializeField] private float _staticCapacity = 7.5f;

    /// <summary>
    /// 静止している時間
    /// </summary>
    [SerializeField] private float _staticDuration = 2.5f;

    /// <summary>
    /// 魔法陣作成までの待機時間
    /// </summary>
    [SerializeField] private float _delayInitMagicCircle = 0.3f;

    /// <summary>
    /// マウスの入力取得用
    /// </summary>
    private InputMouse _inputMouse = new InputMouse();
    /// <summary>
    /// InputMouseの公開用
    /// </summary>
    public InputMouse InputMouse => _inputMouse;

    private InputReceiveData _inputReceiveData = new InputReceiveData();

    /// <summary>
    /// trueのとき入力をマウスで行う
    /// </summary>
    [SerializeField] private bool isUseMouseInput = true;

    private bool isUpdateInputData = false;


    private InputData[] _inputDatas;

    private Dictionary<MagicType, MagicData>
        MagicDataDictionary = new Dictionary<MagicType, MagicData>();

    /// <summary>
    /// checkMagicType今のフレームにMagicTypeごとのInputが存在するか保持する変数
    /// </summary>
    private bool[] checkMagicType;

    #region Offset

    public float XOffset = 0;
    public float YOffset = 0;
    public float RotationOffset = 0;
    public float ScaleOffsetX = 0;
    public float ScaleOffsetY = 0;

    #endregion

    private SceneList _scene;
    private bool isInputDataUpdate = false;

    #region UniRx

    /// <summary>
    /// MagicTypeに対応した座標を常に返す
    /// </summary>
    public Action<Vector2, MagicType> MagicPositionAlways;

    /// <summary>
    /// 魔法の座標の停止開始
    /// </summary>
    public Action<Vector2, MagicType> Magic_StartCoordinatePause;

    /// <summary>
    /// 魔法の座標の停止中
    /// </summary>
    public Action<Vector2, MagicType> Magic_CoordinatePaused;

    /// <summary>
    /// 魔法の停止完了
    /// </summary>
    public Action<Vector2, Vector2, MagicType> Magic_PauseCompleted;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //初期化
        _inputReceiveData.Init();

        //offsetの読み込み
        XOffset = PlayerPrefs.GetFloat("XOffset", 0);
        YOffset = PlayerPrefs.GetFloat("YOffset", 0);
        RotationOffset = PlayerPrefs.GetFloat("RotationOffset", 0);
        ScaleOffsetX = PlayerPrefs.GetFloat("ScaleOffsetX", 1);
        ScaleOffsetY = PlayerPrefs.GetFloat("ScaleOffsetY", 1);
    }

    /// <summary>
    /// 現在のシーンを内部的に変更する
    /// これによりInputの処理が変わる
    /// </summary>
    /// <param name="sceneList"></param>
    public void ChangeScene(SceneList sceneList)
    {
        isInputDataUpdate = true;
        _scene = sceneList;
        //初期化
        checkMagicType = new[] { false, false, false, false, false };
        ResetMagicDataDictionary();
    }

    /// <summary>
    /// 現在入力されている情報をリセットする
    /// </summary>
    public void ResetMagicDataDictionary()
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

    /// <summary>
    /// 現在入力されている情報をリセットする
    /// </summary>
    public void ResetMagicDataDictionary(MagicType magicType)
    {
        MagicDataDictionary[magicType] = new MagicData();
    }


    private void Update()
    {
        //ChangeSceneが呼び出されていなければ入力処理を受け付けない
        if (!isInputDataUpdate)
        {
            return;
        }

        if (isUseMouseInput)
        {
            //入力データの取得
            //Updateでマウスの座標を処理
            if (isUpdateInputData)
            {
                UpdateInputDataArrays(_inputMouse);
                isUpdateInputData = false;
            }
            else
            {
                isUpdateInputData = true;
            }

            //マウスで使用する魔法を変更する
            if (Input.GetMouseButtonDown(1))
            {
                _inputMouse.ChangeMagicType();
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
        switch (_scene)
        {
            case SceneList.Setting:
            case SceneList.Title:
            case SceneList.MainGame:
                InputNotify(inputDataHandler);
                break;

            case SceneList.GameClear:
            case SceneList.GameOver:
                break;
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
            switch (_scene)
            {
                case SceneList.Title:
                    TitleManager.Instance.Magic_CancelCoordinatePause(MagicDataDictionary[magicType].LastFramePosition,
                        magicType);
                    break;
                case SceneList.MainGame:
                    GameManager.Instance.Magic_CancelCoordinatePause(MagicDataDictionary[magicType].LastFramePosition,
                        magicType);
                    break;
            }
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

    /// <summary>
    /// IInputDataHandlerから座標と魔法の種類を通知するロジック
    /// </summary>
    /// <param name="inputDataHandler"></param>
    private void InputNotify(IInputDataHandler inputDataHandler)
    {
        //入力される座標を取得
        //入力されるMagicTypeの種類は一意である
        //ただし、入力される順番とすべての魔法が入力されているかは保証されない
        _inputDatas = inputDataHandler.UpdateInputArrays();

        for (int inputIndex = 0; inputIndex < _inputDatas.Length; inputIndex++)
        {
            //コードが長くなるので変数で保持
            var inputData = _inputDatas[inputIndex];
            var magicData = MagicDataDictionary[inputData.MagicType];

            checkMagicType[(int)inputData.MagicType] = true;

            //追加されているアクションを実行
            MagicPositionAlways?.Invoke(GetCalibratedVector2(inputData.Pos), inputData.MagicType);

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

                    Magic_StartCoordinatePause?.Invoke(GetCalibratedVector2(inputData.Pos), inputData.MagicType);
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
                    //魔法陣の発動
                    //ベクトルの概念が消えたので元々ベクトルを入れていたところには Vector2.zero を入れる
                    Magic_PauseCompleted?.Invoke(GetCalibratedVector2(inputData.Pos), Vector2.zero,
                        inputData.MagicType);

                    //このboolがtrueのときは魔法を再度発動できない
                    //魔法のパーティクルが削除されるときにリセットする処理が走る
                    magicData.isUsedMagic = true;
                }


                //魔法陣発動済みの場合魔法陣の移動をする
                if (magicData.isInited)
                {
                    Magic_CoordinatePaused?.Invoke(GetCalibratedVector2(inputData.Pos), inputData.MagicType);
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
    /// Offsetの値を適応したVector2の値を返す
    /// </summary>
    /// <returns></returns>
    private Vector2 GetCalibratedVector2(Vector2 pos)
    {
        //入力される軸が反転しているので修正
        if (!isUseMouseInput)
        {
            pos.y = 512 - pos.y;
        }

        //Kinectの方向に応じてposを回転させる
        //TODO:ここでカメラの角度調整をする
        //float sin = (float)Math.Sin(RotationOffset * (Math.PI / 180));
        //float cos = (float)Math.Cos(RotationOffset * (Math.PI / 180));
        float sin = (float)Math.Sin(0 * (Math.PI / 180));
        float cos = (float)Math.Cos(0 * (Math.PI / 180));
        var res = new Vector2(pos.x * cos + pos.y * sin, pos.x * sin + pos.y * cos);
        
        return new Vector2((res.x + XOffset) * ScaleOffsetX, (res.y + YOffset) * ScaleOffsetY);
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("XOffset", XOffset);
        PlayerPrefs.SetFloat("YOffset", YOffset);
        PlayerPrefs.SetFloat("RotationOffset", RotationOffset);
        PlayerPrefs.SetFloat("ScaleOffsetX", ScaleOffsetX);
        PlayerPrefs.SetFloat("ScaleOffsetY", ScaleOffsetY);
    }

    /// <summary>
    /// マウスを使用してゲームをプレイするか
    /// </summary>
    /// <param name="_isDebugInput"></param>
    public void SetUseMouse(bool _isDebugInput)
    {
        isUseMouseInput = _isDebugInput;
    }

    /// <summary>
    /// 補正値をセット
    /// </summary>
    public void SetCorrection(Vector2 position, Vector2 scale)
    {
        XOffset = position.x;
        YOffset = position.y;
        ScaleOffsetX = scale.x;
        ScaleOffsetY = scale.y;
    }

    /// <summary>
    /// 補正値をセット
    /// </summary>
    public void SetCorrection(Vector2 leftDownPosition, Vector2 rightDownPosition, Vector2 leftUpPosition,
        Vector2 rightUpPosition, Vector2 positionOffset)
    {
        //TODO:角4箇所で補正をかける計算をする
        XOffset = positionOffset.x;
        YOffset = positionOffset.y;
    }
}