using UnityEditor;
using UnityEngine;

public class EffectDataEditor : EditorWindow
{
    private Editor _editor;
    public EffectsData EffectsData;

    private int index = 0;
    private Vector2 _scrollPosition = Vector2.zero;

    [MenuItem("CustomEditor/EffectData")]
    static void Open()
    {
        var window = GetWindow<EffectDataEditor>();
        window.titleContent = new GUIContent("EffectDataEditor");
    }

    /// <Summary>
    /// ウィンドウのパーツを表示する
    /// </Summary>
    void OnGUI()
    {
        //ScriptableObjectの取得
        var guids = UnityEditor.AssetDatabase.FindAssets("t:EffectsData");
        if (guids.Length == 0)
        {
            throw new System.IO.FileNotFoundException("EffectsData does not found");
        }

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        EffectsData = AssetDatabase.LoadAssetAtPath<EffectsData>(path);

        //Editorの要素が更新されたかチェックする
        if (EditorGUI.EndChangeCheck())
        {
            if (EffectsData != null)
            {
                //要素を更新
                _editor = Editor.CreateEditor(EffectsData);
                EditorUtility.SetDirty(EffectsData);
            }
        }

        if (_editor != null)
        {
            //EffectsDataの要素の表示・変更処理
            var database = (EffectsData)_editor.target;
            //スクロールの処理
            EditorGUILayout.Space();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);


            EditorGUILayout.Space();
            for (int i = 0; i < EffectsData.EffectTypeInfoList.Count; i++)
            {
                //カラム
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                var effectType = EffectsData.EffectTypeInfoList[i].MagicType;
                effectType = (EffectType)EditorGUILayout.EnumPopup(effectType);
                EffectsData.EffectTypeInfoList[i].MagicType = effectType;

                //特定のクラスの要素をEditorWindowで指定する
                var baseEffect = EffectsData.EffectTypeInfoList[i].Effect;

                baseEffect = (BaseEffect)EditorGUILayout.ObjectField("", baseEffect,
                    typeof(BaseEffect), true);

                EffectsData.EffectTypeInfoList[i].Effect = baseEffect;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            Undo.RecordObject(database, "");

            //スクロールの処理
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            #region フッダー

            //要素の追加と削除
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("保存", GUILayout.Height(30), GUILayout.Width(60)))
            {
                // ScriptableObjectのパスは 'Assets/' から始まる相対パス
                ScriptableObject obj =
                    AssetDatabase.LoadAssetAtPath("Assets/Resources/EffectsData.asset", typeof(ScriptableObject)) as
                        ScriptableObject;
                // 変更を通知
                EditorUtility.SetDirty(obj);

                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("+", GUILayout.Height(30), GUILayout.Width(30)))
            {
                if (EffectsData != null)
                {
                    database.EffectTypeInfoList.Add(new EffectTypeInfo());
                    _scrollPosition.y += 100f;
                    EditorUtility.SetDirty(EffectsData);
                    AssetDatabase.SaveAssets();
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("-", GUILayout.Height(30), GUILayout.Width(30)))
            {
                if (EffectsData != null)
                {
                    if (database.EffectTypeInfoList.Count <= 1)
                    {
                        Debug.Log($"Listを空にすることはできません");
                    }
                    else
                    {
                        database.EffectTypeInfoList.RemoveAt(database.EffectTypeInfoList.Count - 1);
                        EditorUtility.SetDirty(EffectsData);
                    }
                }
            }

            EditorUtility.SetDirty(EffectsData);
            EditorGUILayout.EndHorizontal();

            #endregion
        }
    }
}