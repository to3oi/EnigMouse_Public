using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer))]
public class StageSelectPanel : MonoBehaviour
{
    [SerializeField] private Texture _stageTexture;
    [SerializeField] private Texture _stageMaskTexture;
    [SerializeField] private MeshRenderer _outlineMeshRenderer;
    private Material _outlineMaterial;
    public int MapIndex { get;private set; }
    public Transform RootTransform { get;private set; }
    private Material _material;
    private bool _isHardMode;
    private bool _selectedPanel;
    
    private static readonly int nameID_MainTexture = Shader.PropertyToID("_MainTexture");
    private static readonly int nameID_FireRatio = Shader.PropertyToID("_FireRatio");
    private static readonly int nameID_Rand = Shader.PropertyToID("_Rand");

    
    private static readonly int nameID_MaskTexture = Shader.PropertyToID("_MaskTexture");
    private static readonly int nameID_OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int nameID_OutlineRatio = Shader.PropertyToID("_OutlineRatio");

    
    /// <summary>
    /// コンストラクタの代わり
    /// Instantiateではコンストラクタにアクセスできないため
    /// </summary>
    /// <param name="stageTexture"></param>
    public void Init(Texture stageTexture,Texture stageMaskTexture,int mapIndex,Transform rootTransform,bool isHardMode)
    {
        _stageTexture = stageTexture;
        _stageMaskTexture = stageMaskTexture;
        MapIndex = mapIndex;
        RootTransform = rootTransform;
        _material = GetComponent<MeshRenderer>().material;
        _material.SetFloat(nameID_Rand,Random.Range(3f,6f));
        _material.SetFloat(nameID_FireRatio,1);
        _material.SetTexture(nameID_MainTexture,_stageTexture);
        _isHardMode = isHardMode;

        _outlineMaterial = _outlineMeshRenderer.material;
        _outlineMaterial.SetTexture(nameID_MaskTexture,_stageMaskTexture);
        _outlineMaterial.SetColor(nameID_OutlineColor,Color.white);
        _outlineMaterial.SetFloat(nameID_OutlineRatio,0);
        
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
        if (!_selectedPanel)
        {
            _selectedPanel = true;
            //前回のSetOutlineをキャンセル
            _tweener?.Complete(false);

            await UniTask.Delay(TimeSpan.FromSeconds(waitTimeSeconds));
            SoundManager.Instance.PlaySE(SEType.SE34);
            await DOVirtual.Float(1, 0, 1, value => { _material.SetFloat(nameID_FireRatio, value); }).ToUniTask();
        }
    }

    /// <summary>
    /// このパネルを選択したときの処理
    /// </summary>
    public void SelectedThisPanel()
    {
        if (!_selectedPanel)
        {
            _selectedPanel = true;
            StageSelectManager.Instance.SelectedStageSelectPanel(this);
        }
    }


    private Tweener _tweener;

    public void SetOutline(MagicType magicType)
    {
        if(_selectedPanel){return;}
        
        //前回のSetOutlineをキャンセル
        _tweener?.Complete(false);
        _outlineMaterial.SetColor(nameID_OutlineColor, GetMagicTypeColor(magicType));

        _tweener = DOVirtual.Float(1f, 0f, 0.5f, value => { _outlineMaterial.SetFloat(nameID_OutlineRatio, value); });
    }
    
    private static Color GetMagicTypeColor(MagicType magicType)
    {
        return magicType switch
        {
            MagicType.Fire => Color.red,
            MagicType.Water => Color.blue,
            MagicType.Ice => Color.cyan,
            MagicType.Wind => Color.green,
            _ => Color.white
        };
    }
}