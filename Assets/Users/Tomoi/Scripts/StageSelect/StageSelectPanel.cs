using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer))]
public class StageSelectPanel : MonoBehaviour
{
    [SerializeField] private Texture _stageTexture;
    public int MapIndex { get;private set; }
    public Transform RootTransform { get;private set; }
    private Material _material;
    private bool _isHardMode;
    private static readonly int nameID_MainTexture = Shader.PropertyToID("_MainTexture");
    private static readonly int nameID_Ratio = Shader.PropertyToID("_Ratio");
    private static readonly int nameID_Rand = Shader.PropertyToID("_Rand");

    /// <summary>
    /// コンストラクタの代わり
    /// Instantiateではコンストラクタにアクセスできないため
    /// </summary>
    /// <param name="stageTexture"></param>
    public void Init(Texture stageTexture,int mapIndex,Transform rootTransform,bool isHardMode)
    {
        _stageTexture = stageTexture;
        MapIndex = mapIndex;
        RootTransform = rootTransform;
        _material = GetComponent<MeshRenderer>().material;
        _material.SetFloat(nameID_Rand,Random.Range(3f,6f));
        _material.SetFloat(nameID_Ratio,1);
        _material.SetTexture(nameID_MainTexture,_stageTexture);
        _isHardMode = isHardMode;
        if (_isHardMode)
        {
            //ハードモードの盤面は選択できないようにレイヤーを変更する
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        //TODO:InitAnimationで必要なら非表示にするなどの処理をする
    }

    /// <summary>
    /// 最初の表示演出
    /// </summary>
    public async UniTask InitAnimation(float waitTimeSeconds)
    {
        RootTransform.localScale = Vector3.zero;
        await UniTask.Delay(TimeSpan.FromSeconds(waitTimeSeconds));
        SoundManager.Instance.PlaySE(SEType.SE35);
        RootTransform.DOScale(1, 0.5f);
    }
    
    /// <summary>
    /// 燃える演出の開始
    /// </summary>
    public async UniTask StartBurn(float waitTimeSeconds)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTimeSeconds));
        SoundManager.Instance.PlaySE(SEType.SE34);
        await DOVirtual.Float(1, 0, 1, value =>
        {
            _material.SetFloat(nameID_Ratio,value);
        }).ToUniTask();
    }

    /// <summary>
    /// このパネルを選択したときの処理
    /// </summary>
    public void SelectedThisPanel()
    {
        StageSelectManager.Instance.SelectedStageSelectPanel(this);
    }
}