using System.Collections;
using UnityEngine;
using DG.Tweening;
public class Mouse : MonoBehaviour
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
    [HideInInspector]public Vector3 MousePosition;  //受け渡し用のマウスの座標

    private StageManager stageManager;
    private Animator anim;

    public bool debugmode = true;
    public Mouse(Vector3 pos, float rotation)
    {
        startPos = pos;//スタート時の自身の座標を保持する
        mousePos = new Vector3(startPos.x, 0, startPos.z - 1);//座標調整でZ座標を-1している
        destinationPos = mousePos;//移動先の座標を現在の座標に設定

        transform.rotation = Quaternion.EulerRotation(0, rotation, 0);
        startQuaternion = transform.rotation;
    }
    void Start()
    {
        stageManager = StageManager.Instance;
        anim = GetComponent<Animator>();
        //デバッグ用
        //startPos = transform.position;//スタート時の自身の座標を保持する
        //startPos = Vector3.zero;
        startPos = transform.position;
        mousePos = new Vector3(startPos.x, 0, startPos.z-1);//座標調整でZ座標を-1している
        MousePosition = mousePos;
        destinationPos = mousePos;//移動先の座標を現在の座標に設定

        transform.rotation = Quaternion.EulerRotation(0, transform.rotation.y, 0);
        startQuaternion = transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isinput&&debugmode)
        {
            //デバッグ用
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                MouseAct();
            }
        }
    }
    public void MouseAct()//マウスの移動と回転
    {
        isinput = true;
        _destinationarea = false;
        anim.SetTrigger("Run");
        //自分のいるマスのイベントを処理
        AreaEvent(stageManager.GetStageObjectType((int)mousePos.x, (int)mousePos.z), true);
        //移動先の探索と移動
        MouseMove();

        //ターン終了用の関数を呼び出す
        
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
        RouteSearch();
        if ((destinationPos.x >= 0 && destinationPos.x <= 5) && (destinationPos.z <= 5 && destinationPos.z >= 0))
        {
            AreaEvent(stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z), false);
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
                    else if(_damage&&_destinationarea)
                    {
                        Debug.Log("死亡");
                        break;
                    }
                    RouteSearch();
                    if (destinationPos.z < 0 || destinationPos.z > 5 || destinationPos.x < 0 || destinationPos.x > 5)
                    {
                        if (dif.x != 0)//x方向に移動するとき
                        {
                            dif.x = Mathf.Sign(dif.x) *(Mathf.Abs(dif.x));
                        }
                        else if (dif.z != 0)//z方向に移動するとき
                        {
                            dif.z = Mathf.Sign(dif.z) *(Mathf.Abs(dif.z));
                        }
                        break;
                    }
                    AreaEvent(stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z), false);//移動先のマスの種類を確認
                }                
                transform.DOMove(dif, runSpead).SetEase(Ease.InOutCubic).SetRelative(true).OnComplete(() =>//現在の座標と目的の座標を比較し、差を代入して移動させる
                {
                    mousePos += dif;//座標を更新
                    MousePosition = mousePos;
                    AreaEvent(stageManager.GetStageObjectType((int)mousePos.x, (int)mousePos.z), true);
                    _destinationarea = false;
                    isinput = false;
                });               
                _rotateCount = 0;//回転の回数リセット
            }
            else if (_rotateCount >= 4)
            {
                Debug.Log("RotateCount" + _rotateCount);
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
    private void KeyGet()
    {
        if(_keyFlag)
        {
            Debug.Log("鍵取得済み");
        }
        _keyFlag = true;

    }
    //死亡時の処理
    public IEnumerator Death()
    {
        anim.SetTrigger("Death");
        _keyFlag = false;
        _damage = false;
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
    //それぞれのマスごとの処理を行う。マウスの座標ならtrue、移動先の座標として使用するならfalse
    private void AreaEvent(StageObjectType? stagetype, bool mouse)
    {
        var pos = transform.position;
        var obj = transform.transform;
        if (mouse)
        {
            pos = mousePos;
        }
        else
        {
            pos = destinationPos;
        }
        if (!debugmode)
        {
            obj = GameObject.Find($"[{Mathf.Abs(pos.z - 6)},{pos.x}]").transform.GetChild(0);
        }
        if (stagetype == null) {
            Debug.LogWarning("AreaEvent is null");
            return; }
        if(mouse)
        {
            Debug.Log("mouse:"+stagetype);
        }
        else
        {
            Debug.Log("destination:"+stagetype);
        }
        switch (stagetype)
        {
            case StageObjectType.Magma:
                if (mouse)
                {
                    StartCoroutine(Death());
                }
                else
                {
                    _destinationarea = true;
                    _damage = true;
                }

                break;
            case StageObjectType.Grassland:
                if (!mouse)
                {
                    _destinationarea = true;
                }
                break;
            case StageObjectType.Wood:
                if (mouse)
                {
                    Debug.LogError("このエリアは進入できません");
                    Reset();
                }
                else
                {
                    _destinationarea = false;
                }
                break;
            case StageObjectType.Monster://火があるなら移動可能にする
                if (mouse)
                {
                    StartCoroutine(Death());

                    //各オブジェクトの内容が更新されたら削除する
                    //if (obj.GetComponent<BaseStageObject>().isValidMove())
                    //{
                    //    StartCoroutine(Death());
                    //}
                    //else
                    //{
                    //    Debug.Log("Monsterは倒れた");
                    //}
                }
                else
                {
                    _destinationarea = true;
                    _damage = true;
                    //各オブジェクトの内容が更新されたら削除する
                    //if (!obj.GetComponent<BaseStageObject>().isValidMove())
                    //{
                    //    _destinationarea = true;
                    //    _damage = true;
                    //}
                    //else
                    //{
                    //    _destinationarea = true;
                    //    _damage = false;
                    //}
                }
                break;
            case StageObjectType.Ice://火があるなら移動可能
                if (mouse)
                {
                    Debug.LogError("このエリアは進入できません");
                    Reset();
                }
                else
                {
                    _destinationarea = false;
                }

                break;
            case StageObjectType.Pond://氷があるなら移動可能
                if (mouse)
                {
                    StartCoroutine(Death());
                    //各オブジェクトの内容が更新されたら削除する
                    //if (obj.GetComponent<BaseStageObject>().isValidMove())
                    //{
                    //    StartCoroutine(Death());
                    //}
                    //else
                    //{
                    //    Debug.Log("池で溺れた");
                    //}
                }
                else
                {
                    _destinationarea = true;
                    _damage = true;
                    //各オブジェクトの内容が更新されたら削除する
                    //if (!obj.GetComponent<BaseStageObject>().isValidMove())
                    //{
                    //    _destinationarea = true;
                    //    _damage = true;
                    //}
                    //else
                    //{
                    //    _destinationarea = true;
                    //    _damage = false;
                    //}
                }
                break;
            case StageObjectType.Abyss://風があるなら移動可能
                if (mouse)
                {
                    StartCoroutine(Death());
                    //各オブジェクトの内容が更新されたら削除する
                    //if (obj.GetComponent<BaseStageObject>().isValidMove())
                    //{
                    //    StartCoroutine(Death());
                    //}
                    //else
                    //{
                    //    Debug.Log("奈落を通り抜けた");
                    //}
                }
                else
                {
                    _destinationarea = true;
                    _damage = true;
                    //各オブジェクトの内容が更新されたら削除する
                    //if (!obj.GetComponent<BaseStageObject>().isValidMove())
                    //{
                    //    _destinationarea = true;
                    //    _damage = true;
                    //}
                    //else
                    //{
                    //    _destinationarea = true;
                    //    _damage = false;
                    //}
                }
                break;
            case StageObjectType.Flame://水があるなら移動可能
                if (mouse)
                {
                    StartCoroutine(Death());
                }
                else
                {
                    _destinationarea = true;
                    _damage = true;
                }
                break;
            case StageObjectType.Rock:
                if (mouse)
                {
                    Debug.LogError("このエリアは進入できません");
                    Reset();
                }
                else
                {
                    _destinationarea = false;
                }

                break;
            case StageObjectType.Key:
                if (mouse)
                {
                    KeyGet();
                    Debug.Log("KeyFlag:"+_keyFlag);
                }
                else
                {
                    KeyGet();
                    Debug.Log("KeyFlag:" + _keyFlag);
                    _destinationarea = true;
                }
                break;
            case StageObjectType.MagicCircle:
                if (_keyFlag && mouse)
                {
                    Debug.Log("ゲームクリア");
                }
                else if (!_keyFlag && mouse)
                {
                    Debug.Log("鍵がありません");
                }
                else if (!mouse)
                {
                    _destinationarea = true;
                }

                break;
            case StageObjectType.Mouse:
                if (!mouse)
                {
                    _destinationarea = true;
                }
                break;

            case StageObjectType.None:
                if (!mouse)
                {
                    _destinationarea = true;
                }
                break;
            default:
                Debug.Log("このstagetypeは存在しません");
                if (!mouse)
                {    
                    _destinationarea = false; 
                }
                break;
        }
    }
}
