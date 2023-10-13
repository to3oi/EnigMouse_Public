using UnityEngine;

public abstract class IInputDataHandler: MonoBehaviour
{
    /// <summary>
    /// 入力される情報を更新してInputData[]を返す
    /// </summary>
    public abstract InputData[] UpdateInputArrays();

    public abstract void Init();
}