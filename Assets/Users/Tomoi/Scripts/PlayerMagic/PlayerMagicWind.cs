public class PlayerMagicWind : BasePlayerMagic
{
    protected override void Awake()
    {
        _magicType = MagicType.Wind;
        base.Awake();
    }
 
    #region SE

    /// <summary>
    /// 杖を盤面にかざしたときの音を再生する
    /// </summary>
    protected override SoundHash PlaySEOnHover()
    {
        return SoundManager.Instance.PlaySE(SEType.WindPrepare);
    }
        
    /// <summary>
    /// 杖を一定時間固定したときの音を再生する
    /// </summary>
    protected override SoundHash PlaySEOnFixedDuration()
    {
        return SoundManager.Instance.PlaySE(SEType.WindCharge);
    }

    /// <summary>
    /// 魔法が発動したときの音を再生する
    /// </summary>
    protected override SoundHash PlaySEOnRegenComplete()
    {
        return SoundManager.Instance.PlaySE(SEType.WindActivation);
    }

    #endregion
}
