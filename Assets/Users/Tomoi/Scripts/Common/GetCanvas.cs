using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class GetCanvas : SingletonMonoBehaviour4Manager<GetCanvas>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public Canvas Canvas
    {
        get
        {
            return _canvas;
        }

    }

    [SerializeField] private Canvas _canvas;

    public FadeMask[] FadeMask
    {
        get
        {
            return _fadeMask;
        }
    }

    [SerializeField] private FadeMask[] _fadeMask;

    public CanvasGroup GameOverCanvas
    {
        get
        {
            return _gameOverCanvas;
        }
    }

    [SerializeField] private CanvasGroup _gameOverCanvas; 
    public CanvasGroup GameClearCanvas
    {
        get
        {
            return _gameClearCanvas;
        }
    }

    [SerializeField] private CanvasGroup _gameClearCanvas;

    public CanvasGroup GameStartCanvas
    {
        get
        {
            return _gameStartCanvas;
        }
    }

    [SerializeField] private CanvasGroup _gameStartCanvas;
    
    public Image FadeImage
    {
        get
        {
            return _fadeImage;
        }
    }

    [SerializeField] private Image _fadeImage;
}