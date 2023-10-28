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

    private void Awake()
    {
        if (!isSetting)
        {
            maxTime = GameManager.Instance.MaxTime;
            gaugeTime = maxTime / 4;
        }

        for (int i = 0; i < gaugeMeshRenderers.Count; i++)
        {
            gaugeMaterials.Add(gaugeMeshRenderers[i].material);
            gaugeMeshRenderers[i].material.SetFloat("_Ratio", 0);
        }
        
        //ゲージの更新
        GameManager.Instance.TimeObservable.Subscribe(timeLimit =>
        {
            var i = 3 - (int)(timeLimit / gaugeTime);

            if (i >= 0)
            {
                gaugeMaterials[i].SetFloat("_Ratio",
                    (timeLimit % gaugeTime) / gaugeTime);
            }
        }).AddTo(this);
    }
}