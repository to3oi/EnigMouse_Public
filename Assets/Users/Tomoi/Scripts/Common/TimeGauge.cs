using System.Collections.Generic;
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
            gaugeMeshRenderers[i].material.SetFloat("_Ratio", 1);
        }
    }

    private void Update()
    {
        if (isSetting) {return;}
        if (GameManager.Instance.Timer.isTimer)
        {
            var i = 3 - (int)(GameManager.Instance.Timer.TimeLimit / gaugeTime);

            if (i >= 0)
            {
                gaugeMaterials[i].SetFloat("_Ratio",
                    (GameManager.Instance.Timer.TimeLimit % gaugeTime) / gaugeTime);
            }
        }
    }
}