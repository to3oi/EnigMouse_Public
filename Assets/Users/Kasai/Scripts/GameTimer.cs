using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private float timeLimit = 0;

    public float TimeLimit => timeLimit;

    private float defaulitime = 0;
    public bool isTimer { get;private set; } = false;
    private bool notice = true;

    public void SetTime(float _timeLimit)
    {
        timeLimit = _timeLimit;
        defaulitime = _timeLimit;
    }

    void Update()
    {
        if (isTimer)
        {
            timeLimit -= Time.deltaTime;
            
            if (notice && timeLimit <= 30)
            {
                GameManager.Instance.CountDown().Forget();
                notice = false;
            }

            if (timeLimit <= 0)
            {
                isTimer = false;
                GameManager.Instance.TimeOver().Forget();
            }
        }
    }

    public void TimerStart()
    {
        isTimer = true;
    }

    public void TimerStop()
    {
        isTimer = false;
    }

    public void TimerReset()
    {
        isTimer = false;
        notice = true;
        timeLimit = defaulitime;
    }
}