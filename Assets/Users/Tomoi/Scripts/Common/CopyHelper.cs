using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class CopyHelper
{
    // DeepCopyメソッド
    public static T DeepCopy<T>(this T src)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, src);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }
}