using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    [Tooltip("生成するGameObject")]
    private ParticleSystem createPrefab;

    private void Start()
    {
        GetCanvas.Instance.GameOverCanvas.alpha = 0;
    }

    /// <summary>
    /// カウントダウンの開始
    /// </summary>
    /// <param name="token"></param>
    public async UniTask StartCountDown(CancellationToken token)
    {
        for (int i = 0; i < 30; i++)
        {
            SoundManager.Instance.PlaySE(SEType.CountDown);
            token.ThrowIfCancellationRequested();
            await UniTask.Delay(TimeSpan.FromSeconds(1));
        }
    }

    /// <summary>
    /// タイムオーバーの開始
    /// </summary>
    [ContextMenu("TimeOverStart")]
    public async UniTask TimeOverStart()
    {
        //魔方陣が四ヶ所に生成
        for (int i = 0; i < 4; i++)
        {
            //魔方陣をランダムな場所に生成
            float x = Random.Range(-5.0f, 5.0f);
            float z = Random.Range(-5.0f, 5.0f);
            var pos = new Vector3(x, 2, z);
            var p = Instantiate(createPrefab, pos, Quaternion.identity);
            p.Play();
            GetCanvas.Instance.FadeMask[i].FadeStart(RectTransformUtility.WorldToScreenPoint(Camera.main, pos), 5);

            await UniTask.Delay(TimeSpan.FromSeconds(0.15));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        //ViewGameOverCanvas();
    }

    public void ViewGameOverCanvas()
    {
        DOVirtual.Float(
            0f,
            1.0f,
            1.25f,
            (tweenValue) => { GetCanvas.Instance.GameOverCanvas.alpha = tweenValue; }
        );
    }
}