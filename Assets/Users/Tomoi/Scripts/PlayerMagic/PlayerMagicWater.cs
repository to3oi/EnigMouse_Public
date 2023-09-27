public class PlayerMagicWater : BasePlayerMagic
{
    protected override void Awake()
    {
        _magicType = MagicType.Water;
        base.Awake();
    }
 
    #region SE

    /// <summary>
    /// 杖を盤面にかざしたときの音を再生する
    /// </summary>
    protected override SoundHash PlaySEOnHover()
    {
        return SoundManager.Instance.PlaySE(SEType.WaterPrepare);

    }
        
    /// <summary>
    /// 杖を一定時間固定したときの音を再生する
    /// </summary>
    protected override SoundHash PlaySEOnFixedDuration()
    {
        return SoundManager.Instance.PlaySE(SEType.WaterCharge);
    }

    /// <summary>
    /// 魔法が発動したときの音を再生する
    /// </summary>
    protected override SoundHash PlaySEOnRegenComplete()
    {
        return SoundManager.Instance.PlaySE(SEType.WaterActivation);

    }

    #endregion
}
