using UnityEngine;

public class StageObjectIce : BaseStageObject
{
    private bool _isPassable = false;//移動できるようにする場合はtrueに変更
    public StageObjectIce(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {

    }

    public override bool HitMagic(MagicType type, Vector2 direction,out StageObjectType stageObjectType)
    {
        if (MagicType.Wind == type)
        {
            var position = transform.position;
            position.y += 1f;

            //1ターンのみ通行可能に変更※ターン数の概念がまだないかも
            _isPassable = true;

            //エフェクトを再生
            //ここでは適当な値StageObject_Noneを使用
            //第二引数、第三引数に座標と回転を入れる
            //回転には基本的にPrefab自身の回転情報を使うQuaternion.identityを入れる
            //もし指定の方向に回転したいときのみQuaternion.Euler()などで計算して使用する
            EffectManager.Instance.PlayEffect(EffectType.StageObject_None, position, Quaternion.identity);
        }

        stageObjectType = StageObjectType.None;
        return false;
    }

    /// <summary>
    /// ネズミが移動可能なマスか判定して返す
    /// </summary>
    /// <returns></returns>
    public override bool isValidMove()
    {
        if (_isPassable)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
