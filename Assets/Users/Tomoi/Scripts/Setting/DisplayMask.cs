using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UIElements.Slider;

/// <summary>
/// プロジェクターで投影する際に対象外の範囲を黒でマスクする処理
/// </summary>
public class DisplayMask : MonoBehaviour
{
    [SerializeField] GameObject pf_DisplayMask;
    private UIDocument _uiDocument;

    [SerializeField] private RectTransform Mask_Top;
    [SerializeField] private RectTransform Mask_Down;
    [SerializeField] private RectTransform Mask_Right;
    [SerializeField] private RectTransform Mask_Left;

    void Start()
    {
        //保存されたマスクのデータを読み込み
        Mask_Top.sizeDelta = new Vector2(Mask_Top.sizeDelta.x ,PlayerPrefs.GetFloat("DisplayMask_Top", 0));
        Mask_Down.sizeDelta = new Vector2(Mask_Down.sizeDelta.x ,PlayerPrefs.GetFloat("DisplayMask_Down", 0));
        Mask_Right.sizeDelta = new Vector2(PlayerPrefs.GetFloat("DisplayMask_Right", 0),Mask_Right.sizeDelta.y);
        Mask_Left.sizeDelta = new Vector2(PlayerPrefs.GetFloat("DisplayMask_Left", 0),Mask_Left.sizeDelta.y);

        // PrefabからUIを生成
        var displayMask = Instantiate(pf_DisplayMask);

        // UIDocumentの参照を保存
        _uiDocument = displayMask.GetComponent<UIDocument>();

        //Top
        var top_Slider = _uiDocument.rootVisualElement.Q<Slider>("Top_Value");

        top_Slider.value = Mask_Top.sizeDelta.y;
        top_Slider.RegisterValueChangedCallback(v =>
        {
            var sizeDelta = Mask_Top.sizeDelta;
            sizeDelta.y = v.newValue;
            Mask_Top.sizeDelta = sizeDelta;
        });

        //Down
        var down_Slider = _uiDocument.rootVisualElement.Q<Slider>("Down_Value");

        down_Slider.value = Mask_Down.sizeDelta.y;
        down_Slider.RegisterValueChangedCallback(v =>
        {
            var sizeDelta = Mask_Down.sizeDelta;
            sizeDelta.y = v.newValue;
            Mask_Down.sizeDelta = sizeDelta;
        });


        //Right
        var right_Slider = _uiDocument.rootVisualElement.Q<Slider>("Right_Value");

        right_Slider.value = Mask_Right.sizeDelta.x;
        right_Slider.RegisterValueChangedCallback(v =>
        {
            var sizeDelta = Mask_Right.sizeDelta;
            sizeDelta.x = v.newValue;
            Mask_Right.sizeDelta = sizeDelta;
        });

        //Left
        var left_Slider = _uiDocument.rootVisualElement.Q<Slider>("Left_Value");

        left_Slider.value = Mask_Left.sizeDelta.x;
        left_Slider.RegisterValueChangedCallback(v =>
        {
            var sizeDelta = Mask_Left.sizeDelta;
            sizeDelta.x = v.newValue;
            Mask_Left.sizeDelta = sizeDelta;
        });
        
        var next_Button = _uiDocument.rootVisualElement.Q<Button>("Next");
        next_Button.clickable.clicked  += () =>
        {
            Destroy(displayMask.gameObject);
            Destroy(gameObject);
        };
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("DisplayMask_Top", Mask_Top.sizeDelta.y);
        PlayerPrefs.SetFloat("DisplayMask_Down", Mask_Down.sizeDelta.y);
        PlayerPrefs.SetFloat("DisplayMask_Right", Mask_Right.sizeDelta.x);
        PlayerPrefs.SetFloat("DisplayMask_Left", Mask_Left.sizeDelta.x);
    }
}