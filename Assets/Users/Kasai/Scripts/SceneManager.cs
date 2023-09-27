using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class SceneManager : SingletonMonoBehaviour<SceneManager>
{
    private GameObject canvas;
    [SerializeField] private float _fadeTime = 1;
    private bool _alpha = false;
    private bool _fadeColorToggle = true;
    private SceneList sceneList;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        ResetCanvasComponent();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoad;
    }
    /// <summary>
    /// 指定したシーンを呼び出す
    /// (例)SceneManager.Instance.SceneChange(SceneList.Title,true,true);
    /// </summary>
    /// <param name="isFade">trueならシーン移動時にFadeOutを行う</param>
    /// <param name="isAlpha">trueならシーン移動時にFade用のImageのAlphaを255を設定し、移動後にFadeInを行う</param>
    /// <param name="isWhite">フェードする色を指定</param>
    public async void SceneChange(SceneList list,bool isFade,bool isAlpha,bool isWhite = true)
    {
        FadeImageColor(isWhite);
        SoundManager.Instance.AllStopSE();
        var sceneID = (int)list;
        sceneList = list;
        if (isFade)
        {
            FadeOut();
            await UniTask.Delay(TimeSpan.FromSeconds(_fadeTime));
        }
        _alpha = isAlpha;
        canvas = null;
        DOTween.KillAll();//鍵の回転アニメーションか何かがシーン移動時にも動き続けていたので停止
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID);
    }

    public void FadeIn()
    {
        GetCanvas.Instance.FadeImage.DOFade(0f, _fadeTime);
    }
    public void FadeOut()
    {
        GetCanvas.Instance.FadeImage.DOFade(1f,_fadeTime);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        ResetCanvasComponent();
        InputManager.Instance.ChangeScene(sceneList);
        if (_alpha)
        {
            if(_fadeColorToggle)
            {
                GetCanvas.Instance.FadeImage.color = Color.white;
            }
            else
            {
                GetCanvas.Instance.FadeImage.color = Color.black;
            }
            
            FadeIn();
        }
        else
        {
            GetCanvas.Instance.FadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    private void ResetCanvasComponent()
    {
        //CanvasGroupのAlphaを0に変更
        foreach(var fadeMask in GetCanvas.Instance.FadeMask)
        {
            fadeMask.Reset();
        }
        GetCanvas.Instance.GameOverCanvas.alpha = 0;
        GetCanvas.Instance.GameClearCanvas.alpha = 0;
        GetCanvas.Instance.GameStartCanvas.alpha = 0;
    }
    /// <summary>
    /// trueで白、falseで黒に変更
    /// alphaは0のためFade処理の前に呼んで
    /// </summary>
    /// <param name="colorMode"></param>
    private void FadeImageColor(bool colorMode)
    {
        _fadeColorToggle = colorMode;
        if(colorMode)
        {
            GetCanvas.Instance.FadeImage.color = new Color(255, 255, 255, 0);
        }
        else
        {
            GetCanvas.Instance.FadeImage.color = new Color(0, 0, 0, 0);
        }
    }
}
