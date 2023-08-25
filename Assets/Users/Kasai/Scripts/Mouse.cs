using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Mouse : SingletonMonoBehaviour<Mouse>
{
    private bool _keyFlag = false;                  //falseで鍵未所持状態
    private bool _destinationarea = false;          //移動先の判定trueで移動可能
    private bool _damage = false;                   //trueの場合死亡処理を行う
    private bool isinput = false;                   //入力されたら一連の行動が終わるまでは処理しない(デバッグ用)
    private int _rotateCount = 0;                   //回転した回数
    [SerializeField] private float runSpead = 0;    //移動速度
    [SerializeField] private float rotateSpead = 0; //回転速度
    private Quaternion startQuaternion;             //自身の角度
    private Vector3 startPos;                       //初期座標
    private Vector3 mousePos;                       //現在座標
    private Vector3 destinationPos;                 //移動先の座標
    public Vector3 MousePosition;                   //受け渡し用のマウスの座標
    private StageManager stageManager;
    private DynamicStageObject dynamicStageObject;
    private Animator anim;
    public Mouse(Vector3 pos, float rotation)
    {
        startPos = pos;//スタート時の自身の座標を保持する
        mousePos = new Vector3(startPos.x, 0, startPos.z - 1);//座標調整でZ座標を-1している
        destinationPos = mousePos;//移動先の座標を現在の座標に設定

        transform.rotation = Quaternion.EulerRotation(0, rotation, 0);
        startQuaternion = transform.rotation;
    }
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        stageManager = StageManager.Instance;
        anim = GetComponent<Animator>();
        //デバッグ用
        startPos = transform.position;
        mousePos = new Vector3(startPos.x, 0, startPos.z - 1);//座標調整でZ座標を-1している
        MousePosition = mousePos;
        destinationPos = mousePos;//移動先の座標を現在の座標に設定

        transform.rotation = Quaternion.EulerRotation(0, transform.rotation.y, 0);
        startQuaternion = transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        //デバッグ用
        if (Input.GetKeyDown(KeyCode.RightShift) && !isinput)
        {
            MouseAct();
        }
    }
    /// <summary>
    /// ネズミの移動
    /// </summary>
    public void MouseAct()
    {
        isinput = true;
        _destinationarea = false;
        anim.SetTrigger("Run");
        //自分のいるマスのイベントを処理
        MouseAreaEvent(stageManager.GetStageObjectType((int)mousePos.x, (int)mousePos.z));
        //移動先の探索と移動
        MouseMove();
    }
    private void RouteSearch()//移動先の決定
    {
        switch (transform.localEulerAngles.y)
        {
            case 0:
                destinationPos.z += 1;
                break;
            case 90:
                destinationPos.x += 1;
                break;
            case 180:
                destinationPos.z -= 1;
                break;
            case 270:
                destinationPos.x -= 1;
                break;
            default:
                Debug.LogWarning("角度が不正な値です。0度に修正します");
                transform.Rotate(0, 0, 0);
                break;
        }
    }
    private void MouseMove()
    {
        destinationPos = mousePos;
        var turncomp = false;
        RouteSearch();
        if ((destinationPos.x >= 0 && destinationPos.x <= 5) && (destinationPos.z <= 5 && destinationPos.z >= 0))
        {
            DestinationAreaEvent(stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z));
            if (_destinationarea && _rotateCount < 4)
            {
                var dif = destinationPos - mousePos; //現在座標と移動先の差
                for (int i = 0; i < 5; i++)//直線状の移動できるマスを検索する
                {
                    dif = destinationPos - mousePos;
                    if (!_destinationarea || stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z) == null)
                    {
                        if (dif.x != 0)//x方向に移動するとき
                        {
                            dif.x -= Mathf.Sign(dif.x);
                        }
                        else if (dif.z != 0)//z方向に移動するとき
                        {
                            dif.z -= Mathf.Sign(dif.z);
                        }
                        break;
                    }
                    else if (_damage && _destinationarea)
                    {
                        break;
                    }
                    else if (_keyFlag && stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z) == StageObjectType.MagicCircle)//鍵を持っている状態でゴールに触ったら其の時点で移動処理を終了
                    {
                        break;
                    }
                    else if (stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z) == StageObjectType.Key && !_keyFlag)
                    {
                        turncomp = true;
                        break;
                    }
                    RouteSearch();
                    if (destinationPos.z < 0 || destinationPos.z > 5 || destinationPos.x < 0 || destinationPos.x > 5)
                    {
                        if (dif.x != 0)//x方向に移動するとき
                        {
                            dif.x = Mathf.Sign(dif.x) * (Mathf.Abs(dif.x));
                        }
                        else if (dif.z != 0)//z方向に移動するとき
                        {
                            dif.z = Mathf.Sign(dif.z) * (Mathf.Abs(dif.z));
                        }
                        break;
                    }
                    DestinationAreaEvent(stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z));
                }
                transform.DOMove(dif, runSpead).SetEase(Ease.InOutCubic).SetRelative(true).OnComplete(() =>//現在の座標と目的の座標を比較し、差を代入して移動させる
                {
                    mousePos += dif;//座標を更新
                    destinationPos = mousePos;
                    MousePosition = mousePos;
                    MouseAreaEvent(stageManager.GetStageObjectType((int)mousePos.x, (int)mousePos.z));
                    _destinationarea = false;
                    isinput = false;
                    if (!turncomp && !_damage)
                    {
                        //ターン終了用の関数を呼び出す
                        GameManager.Instance.TurnComplete();
                    }
                });
                _rotateCount = 0;//回転の回数リセット
            }
            else if (_rotateCount >= 4)
            {
                Debug.LogError("移動先がありません");
                anim.SetTrigger("Idle");
            }
            else
            {
                transform.DORotate(Vector3.up * 90f, rotateSpead, mode: RotateMode.WorldAxisAdd).OnComplete(() =>//正面のマスのtypeが移動不可なら90度回転させてから直進
                {
                    destinationPos = mousePos;//移動先の座標をリセット
                    _rotateCount++;
                    MouseMove();
                });
            }
        }
        else
        {
            transform.DORotate(Vector3.up * 90f, rotateSpead, mode: RotateMode.WorldAxisAdd).OnComplete(() =>//移動先が配列外なら90度回転させてから直進
            {
                destinationPos = mousePos;//移動先の座標をリセット
                _rotateCount++;
                MouseMove();
            });
        }
        anim.SetTrigger("Idle");
    }
    public void Reset()//座標や角度及び鍵の所持状態をリセットさせる
    {
        gameObject.SetActive(true);
        transform.position = startPos;
        transform.rotation = startQuaternion;
    }
    /// <summary>
    /// 鍵の取得
    /// </summary>
    public IEnumerator KeyGet()
    {
        if (!_keyFlag)
        {
            RouteSearch();
            if ((destinationPos.x >= 0 && destinationPos.x <= 5) && (destinationPos.z <= 5 && destinationPos.z >= 0))
            {
                DestinationAreaEvent(stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z));
                if (_destinationarea)
                {
                    anim.SetTrigger("KeyGet");
                    yield return new WaitForSeconds(1f);
                    anim.SetTrigger("Idle");
                    destinationPos = mousePos;//一度検索結果を破棄
                    yield return new WaitForSeconds(2f);
                    MouseMove();
                }
            }
            //ここに鍵取得の演出をいれる

            _keyFlag = true;
        }
        else
        {
            Debug.Log("鍵取得済み");
        }
        yield return null;
    }
    /// <summary>
    /// ゲームクリアの判定
    /// </summary>

    public void ClearChack()
    {
        if (_keyFlag)
        {
            Debug.Log("ゲームクリア");
        }
        else
        {
            Debug.Log("鍵を持っていません");
        }
    }
    /// <summary>
    /// 死亡時の処理
    /// </summary>
    public IEnumerator Death()
    {
        anim.SetTrigger("Death");
        _keyFlag = false;
        _damage = false;
        yield return new WaitForSeconds(2f);
        GameManager.Instance.TurnComplete();
        gameObject.SetActive(false);
    }
    private async void MouseAreaEvent(StageObjectType? stagetype)
    {
        var pos = mousePos;
        dynamicStageObject = stageManager.GetDynamicStageObject((int)pos.x, (int)Mathf.Abs(pos.z - 5));
        if (stagetype == null)
        {
            Debug.LogWarning("AreaEvent is null");
            return;
        }
        if (dynamicStageObject.isValidMove())
        {
            switch (stagetype)
            {
                case StageObjectType.Key:
                case StageObjectType.MagicCircle:
                case StageObjectType.Magma:
                case StageObjectType.Abyss:
                case StageObjectType.Pond:
                case StageObjectType.Monster:
                case StageObjectType.Flame:
                    dynamicStageObject.MoveToCell();
                    break;
                case StageObjectType.None:
                case StageObjectType.Grassland:
                case StageObjectType.Mouse:
                    break;
                default:
                    Debug.LogError("このエリアには侵入できません");
                    break;
            }
        }
        else
        {
            Debug.LogError("このエリアには侵入できません");
        }
    }
    private void DestinationAreaEvent(StageObjectType? stagetype)
    {
        var pos = destinationPos;
        dynamicStageObject = stageManager.GetDynamicStageObject((int)pos.x, (int)Mathf.Abs(pos.z - 5));
        if (stagetype == null)
        {
            Debug.LogWarning("AreaEvent is null");
            return;
        }
        if (dynamicStageObject.isValidMove())
        {
            _destinationarea = true;
            switch (stagetype)
            {
                case StageObjectType.None:
                case StageObjectType.Grassland:
                case StageObjectType.Mouse:
                case StageObjectType.MagicCircle:
                case StageObjectType.Key:
                    break;
                case StageObjectType.Magma:
                case StageObjectType.Abyss:
                case StageObjectType.Pond:
                case StageObjectType.Monster:
                case StageObjectType.Flame:
                    _damage = true;
                    break;
                default:
                    Debug.LogError("このエリアには侵入できません");
                    break;
            }
        }
        else
        {
            _destinationarea = false;
        }
    }
}