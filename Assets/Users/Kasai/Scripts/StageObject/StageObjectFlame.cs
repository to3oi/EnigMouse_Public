using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageObjectFlame : BaseStageObject
{
    private Vector3 _flamePos;
    private Vector3 _destinationPos;
    private bool _detinationArea = false;

    [SerializeField] private Transform _objectRoot;
    private Vector3 _effectFirePosition = new Vector3(0f, 0.9f, 0f);

    private StageManager stageManager;
    /// <summary>
    /// 炎が出ているか
    /// </summary>
    private bool isFire = false;

    public StageObjectFlame(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {
        _flamePos = new Vector3(transform.position.x, 0, transform.position.z);
        _destinationPos = _flamePos;
    }

    public override async UniTask InitAnimation()
    {
        PlayFireEffect();
        await base.InitAnimation();
    }

    public override async UniTask EndAnimation()
    {
        StopFireEffect();
        await base.EndAnimation();
    }

    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        _flamePos = transform.position;
        _destinationPos = _flamePos;
        _flamePos.y += 1f;
        if (MagicType.Wind == type) //風の魔法を受けた時の処理
        {
            EffectManager.Instance.PlayEffect(EffectType.Magic_Fire, _flamePos, Quaternion.identity);

            RouteSearch(direction);

            FireMove(direction);

            //自身をNoneに変更
        }

        if (MagicType.Water == type) //水の魔法を受けた時の処理
        {
            //自身をNoneに変更


            //エフェクトを再生
            //ここでは適当な値StageObject_Noneを使用
            //第二引数、第三引数に座標と回転を入れる
            //回転には基本的にPrefab自身の回転情報を使うQuaternion.identityを入れる
            //もし指定の方向に回転したいときのみQuaternion.Euler()などで計算して使用する
            //EffectManager.Instance.PlayEffect(EffectType.Magic_Water, _flamePos, Quaternion.identity);

            //Fireのエフェクトを停止
            StopFireEffect();

            //TODO:変更処理がありそうだけどわからないから保留
            //stageObjectType = StageObjectType.Flame;
            //return true;
        }

        stageObjectType = StageObjectType.Flame;
        return false;
    }

    private void RouteSearch(Vector2 direction) //移動方向の選択
    {
        var winddir = (Mathf.Sign(direction.x), Mathf.Sign(direction.y));
        switch (winddir)
        {
            case (1, 0):
                _destinationPos.x += 1;
                break;
            case (-1, 0):
                _destinationPos.x -= 1;
                break;
            case (0, 1):
                _destinationPos.y += 1;
                break;
            case (0, -1):
                _destinationPos.y -= 1;
                break;

            default:
                Debug.LogError("不正な値です");
                break;
        }
    }

    private void FireMove(Vector2 direction) //炎の移動先の検索
    {
        //盤面の外に向かって風が吹いた場合の挙動どうする
        var mousePos = GameObject.Find("Mouse").GetComponent<Mouse>().MousePosition;
        stageManager = StageManager.Instance;
        RouteSearch(direction);
        if (AreaEvent(stageManager.GetStageObjectType((int)_destinationPos.x, (int)_destinationPos.z)) ||
            stageManager.GetStageObjectType((int)_destinationPos.x, (int)_destinationPos.z) == null)
        {
            var dif = _destinationPos - _flamePos;
            for (int i = 0; i < 5; i++)
            {
                dif = _destinationPos - _flamePos;
                if (!AreaEvent(stageManager.GetStageObjectType((int)_destinationPos.x,
                        (int)_destinationPos.z))) //マウスと座標がかぶったときの処理も追加予定
                {
                    //巻き戻し
                    if (dif.x != 0) //x方向に移動するとき
                    {
                        dif.x -= Mathf.Sign(dif.x);
                    }
                    else if (dif.z != 0) //z方向に移動するとき
                    {
                        dif.z -= Mathf.Sign(dif.z);
                    }

                    break;
                }
                else if (AreaEvent(stageManager.GetStageObjectType((int)_destinationPos.x, (int)_destinationPos.z)) ||
                         _destinationPos == mousePos) //マウスの座標とかぶったときとAreaEventがtrueのとき
                {
                    break;
                }

                RouteSearch(direction);
                if (_destinationPos.z < 0 || _destinationPos.z > 5 || _destinationPos.x < 0 || _destinationPos.x > 5)
                {
                    //巻き戻し
                    if (dif.x != 0) //x方向に移動するとき
                    {
                        dif.x -= Mathf.Sign(dif.x);
                    }
                    else if (dif.z != 0) //z方向に移動するとき
                    {
                        dif.z -= Mathf.Sign(dif.z);
                    }

                    break;
                }
            }
        }
        //火の魔法の処理を_destinationPosに発生させる
    }

    private bool AreaEvent(StageObjectType? stagetype)
    {
        if (stagetype == null)
        {
            Debug.LogWarning("AreaEvent is null");
            return false;
        }

        switch (stagetype)
        {
            case StageObjectType.Magma:
                return false;
            case StageObjectType.Grassland:
                return true;
            case StageObjectType.Wood:
                return true;
            case StageObjectType.Monster:
                return true;
            case StageObjectType.Ice:
                return true;
            case StageObjectType.Pond:
                return true;
            case StageObjectType.Abyss:
                return false;
            case StageObjectType.Flame:
                return false;
            case StageObjectType.Rock:
                return false;
            case StageObjectType.Key:
                return false;
            case StageObjectType.MagicCircle:
                return false;
            case StageObjectType.Mouse:
                return false;
            case StageObjectType.None:
                return true;
            default:
                Debug.Log("このstagetypeは存在しません");
                return false;
        }
    }


    private BaseEffect _baseFireEffect;

    private void PlayFireEffect()
    {
        _baseFireEffect = EffectManager.Instance.PlayEffect(EffectType.StageObject_Flame, _effectFirePosition,
            Quaternion.identity, _objectRoot);
        isFire = true;
    }

    private void StopFireEffect()
    {
        if (_baseFireEffect._effect.isPlaying)
        {
            _baseFireEffect._effect.Stop();
            isFire = false;
        }
    }


    /// <summary>
    /// ネズミが移動可能なマスか判定して返す
    /// </summary>
    /// <returns></returns>
    public override bool isValidMove()
    {
        return true;
    }

    public override async UniTask MoveToCell()
    {
        if (isFire)
        {
            //await Mouse.Instance.Death();
        }
    }
    
    public override bool isMovedDeath()
    {
        return isFire;
    }
}