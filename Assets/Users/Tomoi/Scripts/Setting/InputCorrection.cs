using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class InputCorrection : MonoBehaviour
{
    #region UI周りの変数

    [SerializeField] private GameObject pf_InputScale;
    private GameObject inputScale;
    private UIDocument _uiDocument;
    private Label TextArea;
    private Button next_Button;
    private Button complete_Button;
    private bool isNextButtonClicked = false;
    private bool isSettingCompleted = false;

    /// <summary>
    /// 最初に入力がだったときの分岐を入れる
    /// </summary>
    private bool isFirstClicked = false;

    #endregion


    #region 入力補正の計算用変数

    private Vector2 positoinOffset = Vector2.zero;
    private Vector2 scaleOffset = Vector2.zero;

    
    /// <summary>
    /// 入力されて座標を一時的に保持する変数
    /// </summary>
    private Vector2 tempInputPosition = Vector2.zero;

    /// <summary>
    /// 入力された座標を保持
    /// </summary>
    private Vector2[] inputPositions = {Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero};

    /*
     * 2            3
     *
     *
     *
     * 0            1
     */
    /// <summary>
    /// 
    /// </summary>
    [SerializeField] 
    private Transform[] targetPositionTransforms;


    private float x_Distance;
    private float y_Distance;

    #endregion

    private Camera _camera;


    #region キャンセルトークン

    private CancellationTokenSource settingProgressCts;
    private CancellationToken settingProgressCt;

    /// <summary>
    /// 入力座標の補正設定の進行を停止する
    /// </summary>
    /// <returns></returns>
    private CancellationToken SettingProgressCancel()
    {
        //キャンセル
        settingProgressCts?.Cancel();
        //新しく作成
        settingProgressCts = new CancellationTokenSource();
        settingProgressCt = settingProgressCts.Token;
        return settingProgressCt;
    }

    #endregion

    async void Start()
    {
        AddInputEvent();
        _camera = Camera.main;
        InputManager.Instance.SetCorrection(Vector2.zero, Vector2.one);
        inputScale = Instantiate(pf_InputScale);
        _uiDocument = inputScale.GetComponent<UIDocument>();

        TextArea = _uiDocument.rootVisualElement.Q<Label>("TextArea");


        //最初に入力がEnigMouseSendMasterからの入力かマウスかを選択
        //NextではEnigMouseSendMaster
        next_Button = _uiDocument.rootVisualElement.Q<Button>("Next");
        next_Button.clickable.clicked += ClickedNext;

        //Completeではマウス
        complete_Button = _uiDocument.rootVisualElement.Q<Button>("Complete");

        //設定完了時の処理
        complete_Button.clickable.clicked += SettingCompleted;
        InputCorrectionSettingProcess(SettingProgressCancel()).Forget();
    }

    private void ClickedNext()
    {
        isNextButtonClicked = true;
    }

    private void SettingCompleted()
    {
        if (!isSettingCompleted)
        {
            return;
        }

        InputManager.Instance.SetUseMouse(false);

        /*
         * 2            3
         *
         *
         *
         * 0            1
        */
        Vector3[] ScreenPos = new []
        {
            _camera.WorldToScreenPoint(targetPositionTransforms[0].position),
            _camera.WorldToScreenPoint(targetPositionTransforms[1].position),
            _camera.WorldToScreenPoint(targetPositionTransforms[2].position),
            _camera.WorldToScreenPoint(targetPositionTransforms[3].position)
        };

        //設定完了時に呼び出される処理

        //距離計算用に保持
        x_Distance = Mathf.Abs(Mathf.Abs(ScreenPos[3].x) - Mathf.Abs(ScreenPos[0].x));
        y_Distance = Mathf.Abs(Mathf.Abs(ScreenPos[3].y) - Mathf.Abs(ScreenPos[0].y));

        var x_scale = x_Distance / Mathf.Abs(Mathf.Abs(inputPositions[3].x) - Mathf.Abs(inputPositions[0].x));
        var y_scale = y_Distance / Mathf.Abs(Mathf.Abs(inputPositions[3].y) - Mathf.Abs(inputPositions[0].y));

        var resScale = new Vector2(x_scale, y_scale);


        //補正値をセット
        //オフセットはfirstPositionTransformからfirstPositionの値までの距離
        //正負符号を取得
        var resPosition = (new Vector2(ScreenPos[0].x, ScreenPos[0].y) - inputPositions[0] * resScale).normalized
            * Vector2.Distance(new Vector2(ScreenPos[0].x, ScreenPos[0].y), inputPositions[0] * resScale) / resScale;


        //TODO:角4箇所で補正をかける計算をする
        /*InputManager.Instance.SetCorrection(ScreenPos[0], ScreenPos[1],
            ScreenPos[2], ScreenPos[3], resScale);*/
        InputManager.Instance.SetCorrection(resPosition, resScale);

        //設定の停止
        SettingProgressCancel();
        Destroy(inputScale.gameObject);
        Destroy(gameObject);
    }


    /// <summary>
    /// 入力の補正を開始
    /// </summary>
    /// <param name="token"></param>
    private async UniTask InputCorrectionSettingProcess(CancellationToken token)
    {
        //////////////////////////////////////////////////////////////////////////////
        
        next_Button.text = "次へ";
        complete_Button.text = "";
        complete_Button.style.visibility = Visibility.Hidden;
        isSettingCompleted = false;
        TextArea.text = "1 盤面上からすべての魔導具をどけてください";

        await UniTask.WaitUntil(NextButtonClicked, cancellationToken: token);

        //四箇所の角分for文を回す
        for (int i = 0; i < 4; i++)
        {
            //////////////////////////////////////////////////////////////////////////////
            TextArea.text = $"{i + 2} 魔法の中心に魔導具を一つ置いて「次へ」を押してください";
            var pos = targetPositionTransforms[i].position + Vector3.up * 0.01f;
            var eff = EffectManager.Instance.PlayEffect(EffectType.Fire_UI, pos, quaternion.identity);
            await UniTask.WaitUntil(NextButtonClicked, cancellationToken: token);
            eff.Stop();
            inputPositions[i] = tempInputPosition;
            //////////////////////////////////////////////////////////////////////////////
        }

        //////////////////////////////////////////////////////////////////////////////
        TextArea.text = "";
        next_Button.text = "もう一度設定する";
        complete_Button.text = "設定完了";
        complete_Button.style.visibility = Visibility.Visible;
        isSettingCompleted = true;
        await UniTask.WaitUntil(NextButtonClicked, cancellationToken: token);
        //////////////////////////////////////////////////////////////////////////////


        InputCorrectionSettingProcess(SettingProgressCancel()).Forget();
    }

    /// <summary>
    /// NextボタンをクリックしたらTrueを返す
    /// </summary>
    /// <returns></returns>
    private bool NextButtonClicked()
    {
        if (!isNextButtonClicked)
        {
            return false;
        }
        else
        {
            isNextButtonClicked = false;
            return true;
        }
    }


    #region イベント

    private void UpdatePosition(Vector2 pos, MagicType magicType)
    {
        tempInputPosition = pos;
        GetMagicPoint(pos);
    }

    /// <summary>
    /// レイキャストで盤面上の座標を取得する
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private (bool isHitArea, Vector3 position) GetMagicPoint(Vector2 pos)
    {
        Ray ray = _camera.ScreenPointToRay(pos);


        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            return (true, hit.point);
        }

        return (false, Vector3.zero);
    }

    private void AddInputEvent()
    {
        InputManager.Instance.MagicPositionAlways += UpdatePosition;
    }

    private void RemoveInputEvent()
    {
        InputManager.Instance.MagicPositionAlways -= UpdatePosition;
    }


    void OnDestroy()
    {
        //GameObject破棄時にキャンセル実行
        settingProgressCts?.Cancel();

        RemoveInputEvent();
    }

    #endregion
}