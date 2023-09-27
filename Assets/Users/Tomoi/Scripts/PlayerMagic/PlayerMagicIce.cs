public class PlayerMagicIce : BasePlayerMagic
{
    protected override void Awake()
    {
        _magicType = MagicType.Ice;
        base.Awake();
    }
 
    #region SE

    /// <summary>
    /// 杖を盤面にかざしたときの音を再生する
    /// </summary>
    protected override SoundHash PlaySEOnHover()
    {
        return SoundManager.Instance.PlaySE(SEType.IcePrepare);
    }
        
    /// <summary>
    /// 杖を一定時間固定したときの音を再生する
    /// </summary>
    protected override SoundHash PlaySEOnFixedDuration()
    {
        return SoundManager.Instance.PlaySE(SEType.IceCharge);
    }

    /// <summary>
    /// 魔法が発動したときの音を再生する
    /// </summary>
    protected override SoundHash PlaySEOnRegenComplete()
    {
        return SoundManager.Instance.PlaySE(SEType.IceActivation);
    }

    #endregion
}