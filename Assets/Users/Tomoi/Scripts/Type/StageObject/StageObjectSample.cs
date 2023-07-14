using UnityEngine;

/// <summary>
/// StageObjectのサンプル
/// </summary>
//                              ↓BaseStageObjectを継承する
public class StageObjectSample : BaseStageObject
{
    
    //↓コンストラクタを生成する
    public StageObjectSample(Vector2 position,int stageCreateAnimationIndex) : base(position,stageCreateAnimationIndex)
    {
    }

    /// <summary>
    /// 引数のMagicTypeを参照してこのStageObjectに与える影響を定義する関数
    /// </summary>
    public override bool HitMagic(MagicType type,Vector2 direction, out StageObjectType stageObjectType)
    {
        //以下サンプル
        //使用した魔法がFireのときにEffectを再生したい場合
        if (MagicType.Fire == type)
        {
            //エフェクトを再生する位置を調整
            //今回はCubeを使用しているので中心から1上に上げた値にする
            var position = transform.position;
            position.y += 1f;
            
            
            //エフェクトを再生
            //ここでは適当な値StageObject_Noneを使用
            //第二引数、第三引数に座標と回転を入れる
            //回転には基本的にPrefab自身の開店情報を使うQuaternion.identityを入れる
            //もし指定の方向に回転したいときのみQuaternion.Euler()などで計算して使用する
            EffectManager.Instance.PlayEffect(EffectType.StageObject_None,position,Quaternion.identity);
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
        return true;
    }
}