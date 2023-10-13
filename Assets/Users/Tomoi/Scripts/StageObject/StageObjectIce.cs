using System;
using UnityEngine;

public class StageObjectIce : BaseStageObject
{
    private Vector2 vec , pos = Vector2.zero;
    private DynamicStageObject dynamicStageObject;
    private BaseEffect _effect;
    public StageObjectIce(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {

    }
    public override bool HitMagic(MagicType type, Vector2 direction,out StageObjectType stageObjectType)
    {
        //if (MagicType.Wind == type)
        //{
        //    var position = transform.position;
        //    position.y += 1f;
        //    EffectManager.Instance.PlayEffect(EffectType.StageObject_None, position, Quaternion.identity);

        //    vec = direction;
        //    vecSign(vec);

        //    pos = new Vector2(transform.position.x, Mathf.Abs(transform.position.z - 6));
        //    var destinationPos = new Vector2(transform.position.x + vec.x, Mathf.Abs(transform.position.z - 6) + vec.y);
        //    if ((destinationPos.x <= 5 && destinationPos.x >= 0) && (destinationPos.y <= 5 && destinationPos.y >= 0))
        //    {
        //        dynamicStageObject = StageManager.Instance.GetDynamicStageObject((int)(destinationPos.x), (int)(destinationPos.y));
        //        if (stageTypeCheck(StageManager.Instance.GetStageObjectType((int)destinationPos.x, (int)Mathf.Abs(destinationPos.y - 5))) || destinationPos == Mouse.Instance.MousePosition)
        //        {
        //            dynamicStageObject.ReplaceBaseStageObject(StageObjectType.Ice);
        //            stageObjectType = StageObjectType.None;
        //            return true;
        //        }
        //        else
        //        {
        //            stageObjectType = StageObjectType.Ice;
        //            return false;
        //        }
        //    }
        //}
        if (MagicType.Fire == type)
        {
            var position = transform.position;
            position.y += 0.5f;
            _effect = EffectManager.Instance.PlayEffect(EffectType.Magic_Fire, position, Quaternion.identity);

            stageObjectType = StageObjectType.None;
            SoundManager.Instance.PlaySE(SEType.SE23);
            return true;
        }

        stageObjectType = StageObjectType.Ice;
        return false;
    }

    private void OnDestroy()
    {
        _effect?.Stop();
    }

    private void vecSign(Vector2 vector)
    {
        if (vector == Vector2.zero)
        {
            Debug.LogError("Windが使用できません");
            return;
        }
        else if (vector.x == vector.y)
        {
            Debug.LogError("ベクトル量が同じなため移動先を決定できません");
        }
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            vector.x = Mathf.Sign(vec.x);
            vector.y = 0;
        }
        else
        {
            vector.y = Mathf.Sign(vector.y);
            vector.x = 0;
        }
        vec = vector;
    }
    private void iceMove()//移動先の決定
    {
        if(stageTypeCheck(StageManager.Instance.GetStageObjectType((int)pos.x,(int)(Mathf.Abs(pos.y - 6) + vec.y))))
        {
            //Debug.Log(stageTypeCheck(StageManager.Instance.GetStageObjectType((int)pos.x, (int)pos.y)));
            dynamicStageObject.ReplaceBaseStageObject(StageObjectType.Ice);
        }
    }
    private bool stageTypeCheck(StageObjectType? stagetype)
    {
        if(stagetype == null)
        {
            Debug.Log("StageType = null");
            return false;
        }
        //Debug.Log(stagetype);
        switch(stagetype)
        {
            case StageObjectType.None:
            case StageObjectType.Key:
            case StageObjectType.Mouse:
                return true;
            
            default:
                return false;
        }
    }

    /// <summary>
    /// ネズミが移動可能なマスか判定して返す
    /// </summary>
    /// <returns></returns>
    public override bool isValidMove()
    {
        return false;
    }
}
