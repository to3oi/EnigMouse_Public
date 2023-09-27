using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameStart : MonoBehaviour
{
    [SerializeField]
    [Tooltip("生成するGameObject")]
    private ParticleSystem createPrefab;

    private void Start()
    {
        GetCanvas.Instance.GameStartCanvas.alpha = 0;
    }

    /// <summary>
    /// ゲームの開始
    /// </summary>
    [ContextMenu("GameBeginning")]
    public async UniTask GameBeginning()
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
        await UniTask.Delay(TimeSpan.FromSeconds(4));
        ViewGameStartCanvas();
    }

    public void ViewGameStartCanvas()
    {
        DOVirtual.Float(
            0f,
            1.0f,
            1.25f,
            (tweenValue) => { GetCanvas.Instance.GameStartCanvas.alpha = tweenValue; }
        );
    }
}