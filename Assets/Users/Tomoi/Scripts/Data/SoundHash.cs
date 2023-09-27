using System.Security.Cryptography;

public class SoundHash
{
    public SHA1  SoundHashID { get; set; }
    public SoundHash()
    {
        SoundHashID = SHA1.Create();
    }
}
