using System.Collections.Generic;
using Map;
using UnityEditor;
using UnityEngine;


/// <summary>
/// ゲーム内のマップを作成するエディター拡張
/// </summary>
public class StageMapEditor : EditorWindow
{
    private Editor _editor;
    public StageMaps StageMaps;

    private int index = 0;
    private Vector2 boxSizeOffset = new Vector2(100, 350);
    private Object stageSelectTexture;
    private Object stageSelectMaskTexture;

    private Vector2 scrollPosition = Vector2.zero;


    [MenuItem("CustomEditor/Map")]
    static void Open()
    {
        var window = GetWindow<StageMapEditor>();
        window.titleContent = new GUIContent("MapEditor");
    }

    void ResetList(int _index = -1)
    {
        if (_index == -1)
        {
            _index = index;
        }

        StageMaps.StageMapEditorList[_index].y.Clear();

        for (int y = 0; y < 6; y++)
        {
            X mapX = new X();
            for (int x = 0; x < 6; x++)
            {
                mapX.x.Add(StageObjectType.None);
            }

            StageMaps.StageMapEditorList[_index].y.Add(mapX);
        }

        StageMaps.StageMapEditorList[_index].StageName = "";
        StageMaps.StageMapEditorList[_index].StageMaxTurn = 4;
        StageMaps.StageMapEditorList[_index].MinutesForTimeOver = 5;
        StageMaps.StageMapEditorList[_index].StageSelectTexture = null;
        StageMaps.StageMapEditorList[_index].StageSelectMaskTexture = null;
    }

    /// <Summary>
    /// ウィンドウのパーツを表示する
    /// </Summary>
    void OnGUI()
    {
        //ScriptableObjectの取得
        var guids = UnityEditor.AssetDatabase.FindAssets("t:StageMaps");
        if (guids.Length == 0)
        {
            throw new System.IO.FileNotFoundException("StageMaps does not found");
        }

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        StageMaps = AssetDatabase.LoadAssetAtPath<StageMaps>(path);


        //Listを初期化する処理
        if (GUILayout.Button("現在のListを初期化する"))
        {
            if (StageMaps != null)
            {
                ResetList();
            }
        }


        //Editorの要素が更新されたかチェックする
        if (EditorGUI.EndChangeCheck())
        {
            if (StageMaps != null)
            {
                //要素を更新
                _editor = Editor.CreateEditor(StageMaps);
                EditorUtility.SetDirty(StageMaps);
            }
        }

        EditorGUILayout.Space();
        List<string> mapList = new List<string>();
        for (int i = 0; i < StageMaps.StageMapEditorList.Count; i++)
        {
            mapList.Add(i.ToString());
        }


        GUILayout.BeginHorizontal();
        //ポップアップのGUIを作成
        index = EditorGUILayout.Popup("ステージセレクト", index, mapList.ToArray());
        StageMaps.StageMapEditorList[index].isHardMode =
            EditorGUILayout.Toggle("ハードモード", StageMaps.StageMapEditorList[index].isHardMode);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        //ステージのターン数を設定
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ステージの最大ターン数");
        StageMaps.StageMapEditorList[index].StageMaxTurn =
            EditorGUILayout.IntField(StageMaps.StageMapEditorList[index].StageMaxTurn);
        if (StageMaps.StageMapEditorList[index].StageMaxTurn < 4)
        {
            StageMaps.StageMapEditorList[index].StageMaxTurn = 4;
            Debug.LogError("ステージの最大ターン数を4以下の値は設定できません");
        }

        GUILayout.EndHorizontal();

        //ステージの名前を設定
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ステージの名前");
        StageMaps.StageMapEditorList[index].StageName =
            EditorGUILayout.TextField(StageMaps.StageMapEditorList[index].StageName);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("時間制限(分)");
        StageMaps.StageMapEditorList[index].MinutesForTimeOver =
            EditorGUILayout.IntField(StageMaps.StageMapEditorList[index].MinutesForTimeOver);
        if (StageMaps.StageMapEditorList[index].MinutesForTimeOver < 1)
        {
            StageMaps.StageMapEditorList[index].MinutesForTimeOver = 1;
            Debug.LogError("制限時間を1分以下にはできません");
        }

        GUILayout.EndHorizontal();

        //StageSelectTextureの設定
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ステージ選択画面の画像");
        stageSelectTexture = StageMaps.StageMapEditorList[index].StageSelectTexture;
        stageSelectTexture = EditorGUILayout.ObjectField(stageSelectTexture, typeof(Texture), true);
        StageMaps.StageMapEditorList[index].StageSelectTexture = stageSelectTexture as Texture;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //StageSelectTextureの設定
        EditorGUILayout.LabelField("ステージ選択画面のマスク画像");
        stageSelectMaskTexture = StageMaps.StageMapEditorList[index].StageSelectMaskTexture;
        stageSelectMaskTexture = EditorGUILayout.ObjectField(stageSelectMaskTexture, typeof(Texture), true);
        StageMaps.StageMapEditorList[index].StageSelectMaskTexture = stageSelectMaskTexture as Texture;
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();


        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(4));
        {
            if (_editor != null)
            {
                //StageMapsの要素の表示・変更処理
                var database = (StageMaps)_editor.target;
                //スクロールの処理
                EditorGUILayout.Space();

                //StageMapsの要素を表示
                //縦揃え
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.BeginVertical();
                for (int y = 0; y < StageMaps.StageMapEditorList[index].y.Count; y++)
                {
                    //横揃え
                    EditorGUILayout.BeginHorizontal();
                    for (int x = 0; x < StageMaps.StageMapEditorList[index].y[y].x.Count; x++)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        var objectType = StageMaps.StageMapEditorList[index].y[y].x[x];
                        objectType = (StageObjectType)EditorGUILayout.EnumPopup(objectType);
                        StageMaps.StageMapEditorList[index].y[y].x[x] = objectType;
                        //中央揃え
                        GUILayout.FlexibleSpace();
                        Texture texture =
                            AssetDatabase.LoadAssetAtPath($"Assets/Resources/EditorSprite/{objectType}.png",
                                typeof(Texture)) as Texture;
                        GUILayout.Box(texture, GUILayout.Width((position.size.x - boxSizeOffset.x) / 6),
                            GUILayout.Height((position.size.y - boxSizeOffset.y) / 6));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                Undo.RecordObject(database, "");

                //スクロールの処理
                EditorGUILayout.Space();
                EditorGUILayout.EndScrollView();

                //要素の追加と削除
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("保存", GUILayout.Height(30), GUILayout.Width(60)))
                {
                    // ScriptableObjectのパスは 'Assets/' から始まる相対パス
                    ScriptableObject obj =
                        AssetDatabase.LoadAssetAtPath("Assets/Resources/StageMaps.asset", typeof(ScriptableObject)) as
                            ScriptableObject;
                    // 変更を通知
                    EditorUtility.SetDirty(obj);

                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("+", GUILayout.Height(30), GUILayout.Width(30)))
                {
                    if (StageMaps != null)
                    {
                        database.StageMapEditorList.Add(new Map.Map(new List<X>(6)));
                        ResetList(database.StageMapEditorList.Count - 1);
                        index = database.StageMapEditorList.Count - 1;
                        EditorUtility.SetDirty(StageMaps);
                        AssetDatabase.SaveAssets();
                    }
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("-", GUILayout.Height(30), GUILayout.Width(30)))
                {
                    if (StageMaps != null)
                    {
                        if (database.StageMapEditorList.Count <= 1)
                        {
                            Debug.Log($"Listを空にすることはできません");
                        }
                        else
                        {
                            database.StageMapEditorList.RemoveAt(index);
                            if (database.StageMapEditorList.Count - 1 < index)
                            {
                                index = database.StageMapEditorList.Count - 1;
                            }

                            EditorUtility.SetDirty(StageMaps);
                        }
                    }
                }

                EditorUtility.SetDirty(StageMaps);

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}