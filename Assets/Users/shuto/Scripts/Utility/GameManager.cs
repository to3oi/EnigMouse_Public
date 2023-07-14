using UniRx;
using System;

public partial class GameManager : SingletonMonoBehaviour<GameManager>
{

    // 現在の状態
    private GameProgressState _GameProgressState;

    private Subject<GameProgressState> _GameProgressSubject = new Subject<GameProgressState>();

    public IObservable<GameProgressState> GameProgress => _GameProgressSubject;
    public void Subscribe(GameProgressState observer) => this.Subscribe(observer);

    // 外からこのメソッドを使って状態を変更
    public void SetCurrentState(GameProgressState state)
    {
        _GameProgressState = state;
        OnGameStateChanged(_GameProgressState);
    }

    // 状態が変わったら何をするか
    void OnGameStateChanged(GameProgressState state)
    {
        switch (state)
        {
            case GameProgressState.Start:
                StartAction();
                break;
            case GameProgressState.Playing:
                PlayingAction();
                break;
            case GameProgressState.GameClear:
                EndAction();
                break;
            case GameProgressState.GameOver:
                EndAction();
                break;
            default:
                break;
        }
    }

    // Startになったときの処理
    void StartAction()
    {
    }

    // Prepareになったときの処理
    void PrepareCoroutine()
    {
    }
    // Playingになったときの処理
    void PlayingAction()
    {

    }
    // Endになったときの処理
    void EndAction()
    {
    }
}
