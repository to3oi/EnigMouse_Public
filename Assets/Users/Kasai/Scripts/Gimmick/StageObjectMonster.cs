using UnityEngine;

public class StageObjectMonster : BaseStageObject
{
    public StageObjectMonster(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {

    }
    // Start is called before the first frame update
    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        if(MagicType.Fire == type) 
        {
            var position = transform.position;
            position.y += 1f;
            
            //Noneに置き換える


            //エフェクトを再生
            //ここでは適当な値StageObject_Noneを使用
            //第二引数、第三引数に座標と回転を入れる
            //回転には基本的にPrefab自身の開店情報を使うQuaternion.identityを入れる
            //もし指定の方向に回転したいときのみQuaternion.Euler()などで計算して使用する
            EffectManager.Instance.PlayEffect(EffectType.StageObject_None, position, Quaternion.identity);
        }
        stageObjectType = StageObjectType.Monster;
        return false;
    }

    public override bool isValidMove()
    {
        return true;
    }
}
