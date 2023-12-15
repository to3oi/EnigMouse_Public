using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageObjectAbyss : BaseStageObject
{
    /// <summary>
    /// 移動できるようにする場合はtrueに変更
    /// </summary>
    private bool _isPassable = false;
    private int _PassableStartTurn = 0;
    private BaseEffect _effect;
    public StageObjectAbyss(Vector2 position, int stageCreateAnimationIndex) : base(position, stageCreateAnimationIndex)
    {

    }

    public override bool HitMagic(MagicType type, Vector2 direction, out StageObjectType stageObjectType)
    {
        if (MagicType.Wind == type)
        {
            //通れるターンをセット
            _PassableStartTurn = GameManager.Instance.Turn;
            //1ターンのみ通行可能に変更
            _isPassable = true;
            
            var position = transform.position;
            position.y += 0.5f;

            //エフェクトを再生
            //ここでは適当な値StageObject_Noneを使用
            //第二引数、第三引数に座標と回転を入れる
            //回転には基本的にPrefab自身の回転情報を使うQuaternion.identityを入れる
            //もし指定の方向に回転したいときのみQuaternion.Euler()などで計算して使用する
            _effect = EffectManager.Instance.PlayEffect(EffectType.Magic_Wind, position, Quaternion.identity);
            SoundManager.Instance.PlaySE(SEType.SE25);
        }
        stageObjectType = StageObjectType.Abyss;
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
    /// <summary>
    /// ネズミが今移動したら死亡するか判定して返す
    /// </summary>
    /// <returns></returns>
    public override bool isMovedDeath()
    {
        return !_isPassable;
    }

    public override async UniTask EndTurn()
    {
        //ターン終了時にターンが_PassableStartTurnを越していたら通れなくする
        if (_PassableStartTurn < GameManager.Instance.Turn)
        {
            //風のエフェクトを止める
            _effect?.Stop();
            _isPassable = false;
            if(Mouse.Instance.MousePosition == Position)
            {
                Debug.Log("風魔法終了");
                await Mouse.Instance.Death();
            }
        }
        await UniTask.Yield();
    }
    public override async UniTask MoveToCell()
    {
        if (!_isPassable)
        {
            await Mouse.Instance.Death();
        }
    }

    void OnDestroy()
    {
        if (_effect != null)
        {
            _effect.Stop();
        }
    }
}
