using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class Mouse : SingletonMonoBehaviour<Mouse>
{
    private bool _keyFlag = false;                  //falseで鍵未所持状態
    private bool _destinationarea = false;          //移動先の判定trueで移動可能
    public bool _damage = false;                   //trueの場合死亡処理を行う
    private bool isinput = false;                   //入力されたら一連の行動が終わるまでは処理しない(デバッグ用)
    [SerializeField] private float runSpead = 0;    //移動速度
    [SerializeField] private float rotateSpead = 0; //回転速度
    private Quaternion startQuaternion;             //自身の角度
    private Vector3 startPos;                       //初期座標
    private Vector3 mousePos;                       //現在座標
    private Vector3 destinationPos;                 //移動先の座標
    public Vector2 MousePosition;                   //受け渡し用のマウスの座標
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
        MousePosition = new Vector2(mousePos.x,Mathf.Abs(mousePos.z - 5));
        destinationPos = mousePos;//移動先の座標を現在の座標に設定

        transform.rotation = Quaternion.EulerRotation(0, transform.rotation.y, 0);
        startQuaternion = transform.rotation;

    }
    void Update()
    {

    }
    /// <summary>
    /// ネズミの移動
    /// </summary>
    public void MouseAct()
    {
        if(_damage)
        {
            return;
        }
        SoundManager.Instance.PlaySE(SEType.MouseMove);
        isinput = true;
        _destinationarea = false;
        anim.SetTrigger("Run");
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
            if (_destinationarea)
            {
                var dif = destinationPos - mousePos; //現在座標と移動先の差
                for (int i = 0; i < 5; i++)//直線状の移動できるマスを検索する
                {
                    dif = destinationPos - mousePos;
                    if (!_destinationarea)//移動先が行き止まりの場合
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
                    else if (_damage && _destinationarea)//移動先が死亡するエリアの場合
                    {
                        turncomp = true;
                        //移動先がMonsterの場合のみ移動先を少し手前にする
                        if (stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z) == StageObjectType.Monster)
                        {
                            if (dif.x != 0)//x方向に移動するとき
                            {
                                dif.x -= Mathf.Sign(dif.x) * 0.7f;
                            }
                            else if (dif.z != 0)//z方向に移動するとき
                            {
                                dif.z -= Mathf.Sign(dif.z) * 0.7f;
                            }
                        }
                        break;
                    }
                    else if (_keyFlag && stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z) == StageObjectType.MagicCircle)//鍵を持っている状態でゴールに触ったらその時点で移動処理を終了
                    {
                        break;
                    }
                    else if (stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z) == StageObjectType.Key && !_keyFlag)//初めて鍵のマスに止まった時の処理
                    {
                        turncomp = true;
                        break;
                    }
                    else if(stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z) == StageObjectType.ResetBottle)
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
                    if (stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z) == StageObjectType.Monster)//Monster用にずらした座標の修正
                    {
                        if (dif.x != 0)//x方向に移動するとき
                        {
                            dif.x += Mathf.Sign(dif.x) * 0.7f;
                        }
                        else if (dif.z != 0)//z方向に移動するとき
                        {
                            dif.z += Mathf.Sign(dif.z) * 0.7f;
                        }
                    }
                    mousePos += dif;//座標を更新
                    destinationPos = mousePos;
                    MousePosition.x = mousePos.x;
                    MousePosition.y = Mathf.Abs(mousePos.z - 5);
                    MouseAreaEvent(stageManager.GetStageObjectType((int)mousePos.x, (int)mousePos.z));
                    _destinationarea = false;
                    isinput = false;
                    if(_keyFlag
                       && stageManager.GetStageObjectType((int)destinationPos.x, (int)destinationPos.z) == StageObjectType.MagicCircle
                       && GameManager.Instance.IsLimitCheck())
                    {
                        Debug.Log("Turn:"+GameManager.Instance.Turn);
                    }
                    else if (!turncomp)
                    {
                        transform.DORotate(Vector3.up * 90f, rotateSpead, mode: RotateMode.WorldAxisAdd);
                        //ターン終了用の関数を呼び出す
                        GameManager.Instance.TurnComplete();
                    }
                });
            }
            else
            {
                transform.DORotate(Vector3.up * 90f, rotateSpead,RotateMode.WorldAxisAdd);
                GameManager.Instance.TurnComplete();

            }
        }
        else
        {
            transform.DORotate(Vector3.up * 90f, rotateSpead, RotateMode.WorldAxisAdd);
            GameManager.Instance.TurnComplete();
        }
        anim.SetTrigger("Idle");

    }
    public void Reset(Vector2 pos)//座標や角度及び鍵の所持状態をリセットさせる
    {
        _damage = false;
        _keyFlag = false;

        mousePos = new Vector3(startPos.x, 0, startPos.z - 1);//座標調整でZ座標を-1している
        MousePosition = new Vector2(mousePos.x, Mathf.Abs(mousePos.z - 5));
        destinationPos = mousePos;//移動先の座標を現在の座標に設定

        anim.SetTrigger("Reset");

        SetMousePosition(pos);
    }

    public void HideMouse()
    {
        transform.DOScale(Vector3.zero, 0.5f);
    }
    /// <summary>
    /// 鍵の取得
    /// </summary>
    public IEnumerator KeyGet()
    {
        if (!_keyFlag)
        {
            RouteSearch();
            anim.SetTrigger("KeyGet");
            yield return new WaitForSeconds(1f);
            anim.SetTrigger("Idle");
            SoundManager.Instance.PlaySE(SEType.KeyGet);
            yield return new WaitForSeconds(0.5f);
            MouseMove();
            //ここに鍵取得の演出をいれる
            _keyFlag = true;
        }
        else
        {
            Debug.Log("鍵取得済み");
        }
        yield return null;
    }
    public IEnumerator ResetBottle()
    {
        RouteSearch();
        anim.SetTrigger("KeyGet");
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ResetUseMagicLimit();//魔法の使用状況のリセット
        anim.SetTrigger("Idle");
        SoundManager.Instance.PlaySE(SEType.KeyGet);
        yield return new WaitForSeconds(0.5f);
        MouseMove();
        yield return null;
    }
    /// <summary>
    /// ゲームクリアの判定
    /// </summary>
    public void ClearCheck()
    {
        if (_keyFlag)
        {
            Debug.Log("ゲームクリア");
            anim.SetTrigger("KeyGet");
            
            GameManager.Instance.GameClear().Forget();
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
        _damage = true;
        anim.SetTrigger("Death");
        //SE追加予定
        SoundManager.Instance.PlaySE(SEType.SE17);
        _keyFlag = false;
        yield return new WaitForSeconds(1.5f);
        //非表示の処理をスケールで対応
        yield return transform.DOScale(Vector3.zero, 0.5f);
        
        GameManager.Instance.GameOver().Forget();
        
        //GameManager.Instance.TurnComplete();
        //gameObject.SetActive(false);
    }
    /// <summary>
    /// 指定の座標に変更する
    /// </summary>
    public void SetMousePosition(Vector2 pos)
    {
        //MousePosition = pos;
        //mousePos.x = MousePosition.x;
        //mousePos.z = Mathf.Abs(MousePosition.y - 6);
        //transform.position = mousePos;
        //mousePos.z = Mathf.Abs(MousePosition.y - 5);
        //destinationPos = mousePos;

        mousePos = StageManager.GetWorldPosition(pos) + Vector3.up * 0.5f;
        transform.position = mousePos;
        mousePos.z = Mathf.Abs(pos.y - 5);
        
        //角度を初期角度、サイズを0に変更し、その後1秒かけてもとのサイズに戻す
        transform.rotation = startQuaternion;
        transform.localScale = Vector3.zero;
        transform.DOScale(new Vector3(1, 1, 1),0.5f);    
    }
    /// <summary>
    /// マウスの向きを指定の角度に変更する
    /// </summary>
    /// <param name="rotate"></param>
    public void SetMouseRotation(int rotate)
    {
        if(rotate % 90 != 0)
        {
            Debug.LogError("不正な値です");
            return;
        }
        transform.rotation = Quaternion.Euler(0,rotate, 0);

    }
    public async UniTask MouseStartAnim()
    {
        anim.SetTrigger("Start");
        SoundManager.Instance.PlaySE(SEType.SE17);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        anim.SetTrigger("Idle");
    }
    /// <summary>
    /// マウスが移動した先での処理を決定する
    /// </summary>
    /// <param name="stagetype"></param>
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
                case StageObjectType.ResetBottle:
                    dynamicStageObject.MoveToCell();
                    break;
                case StageObjectType.Magma:
                case StageObjectType.Abyss:
                case StageObjectType.Pond:
                case StageObjectType.Monster:
                case StageObjectType.Flame:
                    if(dynamicStageObject.isMovedDeath())
                    {
                        dynamicStageObject.MoveToCell();
                    }
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
    /// <summary>
    /// マウスが移動する際に移動可能かどうかを判定する
    /// </summary>
    /// <param name="stagetype"></param>
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
                case StageObjectType.ResetBottle:
                    break;
                case StageObjectType.Magma:
                case StageObjectType.Abyss:
                case StageObjectType.Pond:
                case StageObjectType.Monster:
                case StageObjectType.Flame:
                    if(dynamicStageObject.isMovedDeath())
                    {
                        _damage = true;
                    }
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

    /// <summary>
    /// EX演出でゴール時のネズミ退出準備
    /// </summary>
    public async UniTask InitExtraPerformance()
    {
        anim.SetTrigger("Reset");
        //await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        await UniTask.Yield();
        //パーティクルで光らせる
        EffectManager.Instance.PlayEffect(EffectType.EXExitMouseEffect,Vector3.zero,Quaternion.identity,transform);
        anim.SetTrigger("EX");
        SoundManager.Instance.PlaySE(SEType.SE17);
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        anim.SetTrigger("Reset");
    }

    /// <summary>
    /// EX演出でゴール時にネズミが画面外に退出する演出の開始
    /// </summary>
    public async UniTask ExitMouse4ExtraPerformance()
    {
        SoundManager.Instance.PlaySE(SEType.SE37);
        await transform.DOMoveY(10, 1.5f);
    }
}