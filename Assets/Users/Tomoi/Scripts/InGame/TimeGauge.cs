using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class TimeGauge : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> gaugeMeshRenderers = new List<MeshRenderer>();
    private List<Material> gaugeMaterials = new List<Material>();

    private float maxTime;
    private float gaugeTime;
    [SerializeField]
    private bool isSetting;

    private void Start()
    {
        if (!isSetting)
        {
            maxTime = GameManager.Instance.MaxTime;
            gaugeTime = maxTime / 4;
        }

        for (int i = 0; i < gaugeMeshRenderers.Count; i++)
        {
            gaugeMaterials.Add(gaugeMeshRenderers[i].material);
            gaugeMeshRenderers[i].material.SetFloat("_Ratio", !ValueRetention.Instance.PlayedExtraPerformance ? 0 : 1);
        }
        
        //ゲージの更新
        GameManager.Instance.TimeObservable.Subscribe(GaugeUpdate).AddTo(this);
    }

    private void GaugeUpdate(float f)
    {
        var i = 3 - (int)(f / gaugeTime);

        if (i >= 0)
        {
            gaugeMaterials[i].SetFloat("_Ratio",
                (f % gaugeTime) / gaugeTime);
        }
    }
}