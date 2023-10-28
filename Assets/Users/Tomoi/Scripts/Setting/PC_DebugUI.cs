using UnityEngine;
using UnityEngine.UIElements;

public class PC_DebugUI : MonoBehaviour
{
    [SerializeField] private GameObject pf_PC_DebugUI;
    private GameObject pC_DebugUI;
    private UIDocument _uiDocument;

    private Label MagicTypeViewString;
    private VisualElement MagicTypeView;
    [SerializeField] private Sprite[] magicUISprites;

    void Start()
    {
        pC_DebugUI = Instantiate(pf_PC_DebugUI, transform);
        _uiDocument = pC_DebugUI.GetComponent<UIDocument>();
        MagicTypeViewString = _uiDocument.rootVisualElement.Q<Label>("MagicTypeViewString");
        MagicTypeView = _uiDocument.rootVisualElement.Q<VisualElement>("MagicTypeView");

        transform.parent = null;
        gameObject.AddComponent<DontDestroy>();
        
        //初回だけ手動でUIを表示
        MagicTypeViewString.text = InputManager.Instance.InputMouse.MagicType.ToString();
        MagicTypeView.style.backgroundImage = new StyleBackground(magicUISprites[(int)InputManager.Instance.InputMouse.MagicType -1]);
        
        InputManager.Instance.InputMouse.DebugAction += UpdateUI;
    }

    private void UpdateUI(MagicType magicType)
    {
        MagicTypeViewString.text = magicType.ToString();
        MagicTypeView.style.backgroundImage = new StyleBackground(magicUISprites[(int)magicType -1]);
    }
}