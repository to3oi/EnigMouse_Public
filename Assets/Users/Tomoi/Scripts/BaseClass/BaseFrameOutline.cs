using DG.Tweening;
using UnityEngine;

public class BaseFrameOutline : MonoBehaviour
{
    private Material _outlineMaterial;
    private static readonly int OutlineRatio = Shader.PropertyToID("_OutlineRatio");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

    private void Start()
    {
        _outlineMaterial = GetComponent<MeshRenderer>().material;
    }

    private Tweener _tweener;

    public void SetOutline(MagicType magicType)
    {
        //前回のSetOutlineをキャンセル
        _tweener.Complete(false);
        _outlineMaterial.SetColor(OutlineColor, GetMagicTypeColor(magicType));

        _tweener = DOVirtual.Float(1f, 0f, 0.5f, value => { _outlineMaterial.SetFloat(OutlineRatio, value); });
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