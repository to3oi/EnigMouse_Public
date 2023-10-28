using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;


public class SettingFlow : MonoBehaviour
{
    [SerializeField] private GameObject pf_SettingFlow;
    [SerializeField] private GameObject PC_DebugUI;
    [SerializeField] private GameObject DisplayMask;
    [SerializeField] private GameObject InputCorrection;
    
    private UIDocument _uiDocument;
    private GameObject settingFlow;

    private bool isOption1 = false;
    private bool isOption2 = false;

    private Label textArea;
    private Button option1_Button;
    private Button option2_Button;

    private void Awake()
    {
        DisplayMask.SetActive(false);
        InputCorrection.SetActive(false);
    }

    async void Start()
    {
        //初期化
        InputManager.Instance.SetCorrection(Vector2.zero, Vector2.one);
        settingFlow = Instantiate(pf_SettingFlow);

        SetupUI();

        //実行方法を選択
        //PC or プロジェクター
        textArea.text = "ゲームの投影方法を選択してください";
        option1_Button.text = "PC";
        option2_Button.text = "プロジェクター";
        await UniTask.WaitUntil(() => isOption1 || isOption2);

        //PCならこの段階で設定終了
        //タイトルシーンに移動する
        if (isOption1)
        {
            //選択肢を非表示
            settingFlow.SetActive(false);
            InputManager.Instance.SetUseMouse(true);
            InputManager.Instance.SetCorrection(Vector2.zero, Vector2.one);
            SceneManager.Instance.SceneChange(SceneList.Title, true, true);
            PC_DebugUI.SetActive(true);
            return;
        }


        //プロジェクターなら
        if (isOption2)
        {
            //選択肢を非表示
            settingFlow.SetActive(false);
            //マスク処理を表示
            DisplayMask.SetActive(true);
            await UniTask.WaitUntil(() => DisplayMask == null);
            //選択肢を表示
            settingFlow.SetActive(true);
            //SetActive(false)からtrueにするとUIDocumentが新しく生成され、以前の参照では参照が取れないので再度紐づけ
            SetupUI();
        }

        isOption1 = false;
        isOption2 = false;

        //入力方法を選択
        //EnigMouseSendMaster or マウス
        textArea.text = "入力方法を選択してください";
        option1_Button.text = "EnigMouseSendMaster";
        option2_Button.text = "マウス";
        await UniTask.WaitUntil(() => isOption1 || isOption2);

        //選択肢を非表示
        settingFlow.SetActive(false);
        
        //EnigMouseSendMaster
        if (isOption1)
        {
            InputManager.Instance.SetUseMouse(false);
            InputCorrection.SetActive(true);
            await UniTask.WaitUntil(() => InputCorrection == null);
            //設定が完了したらシーンを移動
            SceneManager.Instance.SceneChange(SceneList.Title, true, true);
            return;
        }

        //マウス
        if (isOption2)
        {

            InputManager.Instance.SetUseMouse(true);
            InputManager.Instance.SetCorrection(Vector2.zero, Vector2.one);
            SceneManager.Instance.SceneChange(SceneList.Title, true, true);
            PC_DebugUI.SetActive(true);
            return;
        }
        
        //追加の設定用
        //選択肢を表示
        settingFlow.SetActive(true);
        //SetActive(false)からtrueにするとUIDocumentが新しく生成され、以前の参照では参照が取れないので再度紐づけ
        SetupUI();
        
        isOption1 = false;
        isOption2 = false;
    }

    /// <summary>
    /// UIの要素の参照を紐づけ
    /// </summary>
    private void SetupUI()
    {
        _uiDocument = settingFlow.GetComponent<UIDocument>();

        textArea = _uiDocument.rootVisualElement.Q<Label>("TextArea");
        option1_Button = _uiDocument.rootVisualElement.Q<Button>("Option1");
        option1_Button.clickable.clicked += GetOption1Down;
        option2_Button = _uiDocument.rootVisualElement.Q<Button>("Option2");
        option2_Button.clickable.clicked += GetOption2Down;
    }

    private void GetOption1Down()
    {
        isOption1 = true;
    }

    private void GetOption2Down()
    {
        isOption2 = true;
    }
}