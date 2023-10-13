using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageObjectFlame : BaseStageObject
{
    private Vector2 vec = Vector2.zero;
    private Vector2 pos = Vector2.zero;

    [SerializeField] private Transform _objectRoot;
    private Vector3 _effectFirePosition = new Vector3(0f, 0.9f, 0f);

    private DynamicStageObject dynamicStageObject;
    /// <summary>
    /// 炎が出ているか
    /// </summary>
    private bool isFire = true;

    public StageObjectFlame(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {

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
        if (MagicType.Wind == type && isFire) //風の魔法を受けた時の処理
        {

            StopFireEffect();

            SoundManager.Instance.PlaySE(SEType.SE27);
        }

        if (MagicType.Water == type) //水の魔法を受けた時の処理
        {
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
            SoundManager.Instance.PlaySE(SEType.SE26);
        }

        stageObjectType = StageObjectType.Flame;
        return false;
    }
    private void vecSign(Vector2 winddir)
    {
        if (winddir == Vector2.zero)
        {
            Debug.LogError("Windが使用できません");
            return;
        }
        else if (winddir.x == winddir.y)
        {
            Debug.LogError("ベクトル量が同じなため移動先を決定できません");
        }
        if (Mathf.Abs(winddir.x) > Mathf.Abs(winddir.y))
        {
            winddir.x = Mathf.Sign(winddir.x);
            winddir.y = 0;
        }
        else
        {
            winddir.y = Mathf.Sign(winddir.y);
            winddir.x = 0;
        }
        vec = winddir;
    }
    private void ignition(Vector2 vector)
    {
        var destinationPos = pos - vector;
        if ((destinationPos.x <= 5 && destinationPos.x >= 0) && (destinationPos.y <= 5 && destinationPos.y >= 0))
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.Log("destinationtype =  "+StageManager.Instance.GetStageObjectType((int)destinationPos.x, (int)destinationPos.y));
                Debug.Log("postype = "+StageManager.Instance.GetStageObjectType((int)pos.x, (int)pos.y));
                Debug.Log("Destinationpos:" + destinationPos);
                Debug.Log("vec =" + vector);

                if (!stageTypeCheck(StageManager.Instance.GetStageObjectType((int)destinationPos.x, (int)destinationPos.y)))
                {
                    
                    Debug.Log("TypeCheck:"+stageTypeCheck(StageManager.Instance.GetStageObjectType((int)destinationPos.x, (int)destinationPos.y)));
                    Debug.Log("destination =  " + StageManager.Instance.GetStageObjectType((int)destinationPos.x, (int)destinationPos.y));
                    break;
                }
                else if(Mouse.Instance.MousePosition == pos)
                {
                    Debug.Log("MousePos == pos");
                    break;
                }
                else if(StageManager.Instance.GetStageObjectType((int)destinationPos.x, (int)destinationPos.y) == null)
                {
                    if (vector.x != 0)//x方向に移動するとき
                    {
                        destinationPos.x -= Mathf.Sign(vector.x);
                    }
                    else if (vector.y != 0)//y方向に移動するとき
                    {
                        destinationPos.y -= Mathf.Sign(vector.y);
                    }
                    Debug.Log("ステージ外");
                    break;
                }
                else if(stageTypeCheck(StageManager.Instance.GetStageObjectType((int)destinationPos.x, (int)destinationPos.y)))
                {
                    Debug.Log("Roop");
                }
                destinationPos -= vector;
            }
        }
        pos = destinationPos;
        dynamicStageObject = StageManager.Instance.GetDynamicStageObject((int)pos.x, (int)pos.y);
        dynamicStageObject.HitMagic(MagicType.Fire, Vector2.zero);
    }
    private bool stageTypeCheck(StageObjectType? stagetype)
    {
        if (stagetype == null)
        {
            Debug.Log("StageType = null");
            return false;
        }
        //Debug.Log(stagetype);
        switch (stagetype)
        {
            case StageObjectType.None:
            case StageObjectType.Mouse:
                return true;

            default:
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
            await Mouse.Instance.Death();
        }
    }

    public override bool isMovedDeath()
    {
        return isFire;
    }
}